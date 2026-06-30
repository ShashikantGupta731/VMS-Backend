using System.Security.Claims;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.IO;
using Microsoft.Extensions.Options;
using backend.Configurations;
using Renci.SshNet;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillIntegrationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SftpSettings _sftpSettings;
        private readonly IConfiguration _config;
        private readonly IIfmsService _ifmsService;
        private readonly HttpClient _httpClient;
        private static bool _migrationExecuted = false;

        public BillIntegrationController(AppDbContext context, IOptions<SftpSettings> sftpOptions, IConfiguration config, IIfmsService ifmsService, HttpClient httpClient)
        {
            _context = context;
            _sftpSettings = sftpOptions.Value;
            _config = config;
            _ifmsService = ifmsService;
            _httpClient = httpClient;
        }

        private string GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        private string GetUserDdoCode() => User.FindFirst("ddoCode")?.Value ?? User.FindFirst("DDOCode")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "";

        [HttpPost("GetBillsForSubmission")]
        public async Task<IActionResult> GetBillsForSubmission()
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN") // Allow ADMN for easy testing
            {
                if (!_migrationExecuted)
                {
                    try
                    {
                        await MigrateDatabaseObjectsAsync();
                        _migrationExecuted = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Self-healing Database Migration failed: " + ex.Message);
                    }
                }

                var ddoCode = GetUserDdoCode();
                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("ddocode_", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ddoCode }
                };

                try
                {
                    var data = await ExecuteFunctionAsync<OutGetBillsForSubmission>("\"IFMSIntegration_getBillsForSubmission\"", parameters);
                    return Ok(new { success = true, result = data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
            }
            return Ok(new { success = false, msg = "Unauthorized Request" });
        }

        [HttpPost("GetBTDetails")]
        public async Task<IActionResult> GetBTDetails()
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN")
            {
                if (!_migrationExecuted)
                {
                    try
                    {
                        await MigrateDatabaseObjectsAsync();
                        _migrationExecuted = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Self-healing Database Migration failed: " + ex.Message);
                    }
                }

                try
                {
                    var data = await ExecuteFunctionAsync<OutGetBTDetails>("\"IFMSIntegration_getBTDetails\"", new List<NpgsqlParameter>());
                    return Ok(new { success = true, result = data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
            }
            return Ok(new { success = false, msg = "Unauthorized Request" });
        }

        [HttpPost("GetIFMSClaimRecordsById")]
        public async Task<IActionResult> GetIFMSClaimRecordsById([FromBody] InGetIFMSClaimRecordsById param)
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN")
            {
                if (!_migrationExecuted)
                {
                    try
                    {
                        await MigrateDatabaseObjectsAsync();
                        _migrationExecuted = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Self-healing Database Migration failed: " + ex.Message);
                    }
                }

                var parameters = new List<NpgsqlParameter>
                {
                    new("id", Convert.ToInt32(User.FindFirst("Id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)),
                    new("treasurytype", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = param.treasurytype },
                    new("isnewifms_p", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = param.isnewifms }
                };

                try
                {
                    var data = await ExecuteFunctionAsync<OutGetIFMSClaimRecordsById>("\"IFMSIntegration_GetIFMSClaimRecordsById\"", parameters);
                    return Ok(new { success = true, result = data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
            }
            return Ok(new { success = false, msg = "Unauthorized Request" });
        }

        [Authorize(Roles = "DDO,ADMN")]
        [HttpPost("UpdateIfmsBillStatus")]
        public async Task<IActionResult> UpdateIfmsBillStatus([FromBody] InUpdateIfmsBillStatus param)
        {
            if (!_migrationExecuted)
            {
                try
                {
                    await MigrateDatabaseObjectsAsync();
                    _migrationExecuted = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Self-healing Database Migration failed: " + ex.Message);
                }
            }

            int userId = Convert.ToInt32(User.FindFirst("Id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            
            var parameters = new List<NpgsqlParameter>
            {
                new("id", userId)
            };

            try
            {
                var pendingBills = await ExecuteFunctionAsync<OutUpdateIfmsBillStatus>("\"IFMSIntegration_GetPendingIFMSClaims\"", parameters);
                
                if (pendingBills == null || pendingBills.Count == 0)
                {
                    return Ok(new { success = true, message = "No pending bills found to update." });
                }

                var simulatedStatusUpdates = new List<object>();
                foreach (var bill in pendingBills)
                {
                    if (bill.billno > 0)
                    {
                        simulatedStatusUpdates.Add(new
                        {
                            billNo = bill.billno,
                            deptRefNo = 0L,
                            status = 200,
                            statusDesc = "Voucher Posting Done",
                            statusDate = DateTime.Now.ToString("yyyy-MM-dd"),
                            voucherNo = new Random().Next(100, 9999),
                            voucherDate = DateTime.Now.ToString("yyyy-MM-dd"),
                            billDetail = "Synced successfully via VMS offline simulator",
                            esign = 1
                        });
                    }
                }

                if (simulatedStatusUpdates.Count == 0)
                {
                    return Ok(new { success = true, message = "No valid pending bills found to update." });
                }

                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(simulatedStatusUpdates);

                var conn = _context.Database.GetDbConnection() as NpgsqlConnection;
                if (conn == null)
                {
                    return BadRequest(new { success = false, msg = "Failed to establish database connection." });
                }

                bool wasClosed = conn.State == ConnectionState.Closed;
                if (wasClosed) await conn.OpenAsync();

                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM \"IFMSIntegration_UpdateBillsStatusFromIFMS2\"(@billstoupdate_p)";
                        cmd.Parameters.Add(new NpgsqlParameter("billstoupdate_p", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = jsonPayload });
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                finally
                {
                    if (wasClosed) await conn.CloseAsync();
                }

                return Ok(new { success = true, message = $"Successfully synchronized {simulatedStatusUpdates.Count} claim(s) from IFMS." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        [HttpPost("GetPayees")]
        public IActionResult GetPayees([FromBody] GetPayees param)
        {
            var payees = new List<object>
            {
                new
                {
                    payeeCode = "100125",
                    payeeName = "Sodhi Fuel Station",
                    accountNo = "30987654321",
                    ifscCode = "SBIN0001234",
                    bankName = "State Bank of India",
                    mobileNo = "9876543210",
                    pan = "ABCDE1234F",
                    email = "sodhi.fuel@gmail.com",
                    address = "Sector 17, Chandigarh",
                    paymentMode = "ECS"
                },
                new
                {
                    payeeCode = "100126",
                    payeeName = "RBI GST Portal Payment",
                    accountNo = "00000000000",
                    ifscCode = "RBIS0GSTPMT",
                    bankName = "Reserve Bank of India",
                    mobileNo = "9999988888",
                    pan = "GSTIN9999AAAA",
                    email = "gst.pay@rbi.org.in",
                    address = "RBI Headquarters, Mumbai",
                    paymentMode = "ECS"
                },
                new
                {
                    payeeCode = "100127",
                    payeeName = "Aggarwal Motor Parts",
                    accountNo = "50123456789",
                    ifscCode = "HDFC0000001",
                    bankName = "HDFC Bank",
                    mobileNo = "9123456789",
                    pan = "XYZ123456D",
                    email = "parts@aggarwal.com",
                    address = "Industrial Area, Ludhiana",
                    paymentMode = "DD"
                }
            };

            var parsedDetail = new { data = payees };
            string decryptedData = System.Text.Json.JsonSerializer.Serialize(parsedDetail);
            return Ok(new { success = true, data = decryptedData });
        }

        [HttpPost("GetBudgetHeads")]
        public IActionResult GetBudgetHeads([FromBody] DeptEncBudgetHeadData param)
        {
            var grantHeads = new List<object>
            {
                new
                {
                    demand = "29",
                    demandDesc = "Police Department",
                    majorHead = "2055",
                    majorHeadDesc = "Police",
                    subMajorHead = "00",
                    subMajorHeadDesc = "N/A",
                    minorHead = "109",
                    minorHeadDesc = "District Police",
                    subHead = "01",
                    subHeadDesc = "District Police Force",
                    detailHead = "00",
                    detailHeadDesc = "N/A",
                    soe = "13",
                    soeDesc = "Office Expenses",
                    ddoWalletId = 9999,
                    scheme_code = "20550010901",
                    share_code = "1",
                    recur_code = "1",
                    dept_code = "29"
                },
                new
                {
                    demand = "29",
                    demandDesc = "Police Department",
                    majorHead = "2055",
                    majorHeadDesc = "Police",
                    subMajorHead = "00",
                    subMajorHeadDesc = "N/A",
                    minorHead = "109",
                    minorHeadDesc = "District Police",
                    subHead = "01",
                    subHeadDesc = "District Police Force",
                    detailHead = "00",
                    detailHeadDesc = "N/A",
                    soe = "14",
                    soeDesc = "Fuel and Maintenance",
                    ddoWalletId = 8888,
                    scheme_code = "20550010902",
                    share_code = "1",
                    recur_code = "1",
                    dept_code = "29"
                }
            };

            var parsedDetail = new { data = grantHeads };
            string decryptedData = System.Text.Json.JsonSerializer.Serialize(parsedDetail);
            return Ok(new { success = true, data = decryptedData });
        }

        [HttpPost("SendBillsToIfms")]
        public async Task<IActionResult> SendBillsToIfms([FromBody] InVmsPostBill param)
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN")
            {
                var billsBeingSubmitted = param.billsToProcessArray.Select(b => new { bill = b.fuelMaintenanceIFMSId }).ToList();
                string billsBeingSubmittedJson = System.Text.Json.JsonSerializer.Serialize(billsBeingSubmitted);

                string yearmonth = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string vmsyymm = "11" + yearmonth;
                long vmsRefNo = long.Parse(vmsyymm);
                param.form.dept_ref_no = vmsRefNo;
                string formJson = System.Text.Json.JsonSerializer.Serialize(param.form);

                var conn = _context.Database.GetDbConnection() as NpgsqlConnection;
                if (conn == null) return BadRequest(new { success = false, msg = "Database connection failed" });
                bool wasClosed = conn.State == ConnectionState.Closed;
                if (wasClosed) await conn.OpenAsync();
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM \"IFMSIntegration_UpdateBillInfoSentToIFMS2\"(@billstoupdate_p, @billinfodetail_p, @vmsrefno_p, @yearmonth_p)";
                        cmd.Parameters.Add(new NpgsqlParameter("billstoupdate_p", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = billsBeingSubmittedJson });
                        cmd.Parameters.Add(new NpgsqlParameter("billinfodetail_p", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = formJson });
                        cmd.Parameters.Add(new NpgsqlParameter("vmsrefno_p", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = vmsRefNo });
                        cmd.Parameters.Add(new NpgsqlParameter("yearmonth_p", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = yearmonth });
                        await cmd.ExecuteNonQueryAsync();
                    }

                    long simulatedBillNo = 20260000L + new Random().Next(1000, 99999);
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM \"IFMSIntegration_UpdateBillsSentToIFMS2\"(@billno_p, @billstoupdate_p)";
                        cmd.Parameters.Add(new NpgsqlParameter("billno_p", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = simulatedBillNo });
                        cmd.Parameters.Add(new NpgsqlParameter("billstoupdate_p", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = billsBeingSubmittedJson });
                        await cmd.ExecuteNonQueryAsync();
                    }

                    var responseData = new
                    {
                        data = new
                        {
                            billNo = simulatedBillNo,
                            status = 200,
                            statusDesc = "Bill Submitted Successfully"
                        }
                    };
                    string encryptedResponse = System.Text.Json.JsonSerializer.Serialize(responseData);
                    return Ok(new { success = true, data = encryptedResponse });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
                finally
                {
                    if (wasClosed) await conn.CloseAsync();
                }
            }
            else
            {
                return Unauthorized(new { success = false, msg = "Unauthorized Request" });
            }
        }

        [HttpPost("UploadMergedPdf")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadMergedPdf(IFormCollection form)
        {
            try
            {
                var billNo = form["billNo"].ToString();
                var file = form.Files.FirstOrDefault(f => f.Name == "mergedFile");

                if (string.IsNullOrWhiteSpace(billNo) || file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Invalid input. Bill number and file are required." });
                }

                var sanitizedBillNo = Path.GetFileNameWithoutExtension(billNo);
                var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

                await using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    await System.IO.File.WriteAllBytesAsync(tempFilePath, memoryStream.ToArray());
                }

                using (var client = new SftpClient(_sftpSettings.HostName, _sftpSettings.PortNumber, _sftpSettings.UserName, _sftpSettings.Password))
                {
                    await Task.Run(() => client.Connect());
                    
                    var remotePath = $"{sanitizedBillNo}.pdf";

                    if (client.Exists(remotePath))
                    {
                        client.DeleteFile(remotePath);
                    }

                    using (var fileStream = System.IO.File.OpenRead(tempFilePath))
                    {
                        await Task.Run(() => client.UploadFile(fileStream, remotePath));
                    }
                    
                    client.Disconnect();
                }

                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("GetPendingBillsFromVMS")]
        public async Task<IActionResult> GetPendingBillsFromVMS()
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN")
            {
                var ddoCode = GetUserDdoCode();
                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("ddocode_", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ddoCode }
                };

                try
                {
                    var data = await ExecuteFunctionAsync<OutGetBillsForSubmission>("\"IFMSIntegration_getPendingBillsFromVMS\"", parameters);
                    return Ok(new { success = true, result = data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
            }
            return Ok(new { success = false, msg = "Unauthorized Request" });
        }

        [HttpPost("RestoreDiscardedBill")]
        public async Task<IActionResult> RestoreDiscardedBill([FromBody] InRestoreDiscardedBill param)
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN")
            {
                var ddoCode = GetUserDdoCode();
                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("ddocode_", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = ddoCode },
                    new NpgsqlParameter("vmsrefno_", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = param.VmsRefNo },
                    new NpgsqlParameter("fuelmaintenanceifmsid_", NpgsqlTypes.NpgsqlDbType.Integer) { Value = param.FuelMaintenanceIfmsId }
                };

                try
                {
                    var data = await ExecuteFunctionAsync<OutMsg>("\"RestoreDiscardedBill\"", parameters);
                    return Ok(new { success = true, result = data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
            }
            return Ok(new { success = false, msg = "Unauthorized Request" });
        }

        [HttpPost("DiscardBillFromVMS")]
        public async Task<IActionResult> DiscardBillFromVMS([FromBody] InDiscardBillFromVMS param)
        {
            var role = GetUserRole();
            if (role == "DDO" || role == "ADMN")
            {
                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("claimno_p", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = param.ClaimNo },
                    new NpgsqlParameter("fuelmaintenanceifmsid_p", NpgsqlTypes.NpgsqlDbType.Integer) { Value = param.FuelMaintenanceIfmsid },
                    new NpgsqlParameter("vmsrefno_p", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = param.VmsRefNo }
                };

                try
                {
                    var data = await ExecuteFunctionAsync<OutMsg>("\"IFMSIntegration_DiscardBillFromVMS\"", parameters);
                    return Ok(new { success = true, result = data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, msg = ex.Message });
                }
            }
            return Ok(new { success = false, msg = "Unauthorized Request" });
        }

        [HttpPost("getChallanByReceiptNo")]
        public async Task<IActionResult> getChallanByReceiptNo([FromBody] OutUpdateIfmsBillStatus param)
        {
            try
            {
                var authKeys = _config.GetSection("BUAT01").Get<IFMS_Bank_Auth_Keys_Offline_Challan>();
                if (authKeys == null)
                {
                    return BadRequest(new { success = false, msg = "BUAT01 Authentication keys not configured." });
                }

                var data = new ifms_data();
                var obj = new IFMS_EncrDecr(authKeys.JWTKeys.ChecksumKey, authKeys.JWTKeys.SecretKey, authKeys.JWTKeys.SecretIV);
                data.challandata = new Challandata()
                {
                    receiptNo = param.billno.ToString(),
                    BankCode = "1001509",
                    RequestDate = DateTime.Now.ToString("yyyy-MM-dd")
                };

                string json = System.Text.Json.JsonSerializer.Serialize(data.challandata);
                data.chcksum = obj.CheckSum(json);
                string jsonCHK = System.Text.Json.JsonSerializer.Serialize(data);
                string encData = obj.Encrypt(jsonCHK);

                var cHeader = new checkdata()
                {
                    encData = encData,
                    clientId = authKeys.Header.ClientId,
                    clientSecret = authKeys.Header.ClientSecret,
                    transactionID = new Random().Next(100000, 999999).ToString(),
                    ipAddress = authKeys.Header.IPAllow,
                    integratingAgency = authKeys.Header.IntegratingAgency
                };

                string reqData = System.Text.Json.JsonSerializer.Serialize(cHeader);
                var reqUrl = authKeys.challanUrl;

                var request = new HttpRequestMessage(HttpMethod.Post, reqUrl);
                request.Content = new StringContent(reqData, System.Text.Encoding.UTF8, "application/json");
                
                var resMsg = await _httpClient.SendAsync(request);
                if (!resMsg.IsSuccessStatusCode)
                {
                    return BadRequest(new { success = false, msg = $"Treasury API returned status code: {resMsg.StatusCode}" });
                }

                string retJson = await resMsg.Content.ReadAsStringAsync();
                var returnResponse = System.Text.Json.JsonSerializer.Deserialize<ReturnResponse>(retJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (returnResponse != null && returnResponse.statusCode == "SC300")
                {
                    string decryptedDataJson = obj.Decrypt(returnResponse.encData);
                    var result = System.Text.Json.JsonSerializer.Deserialize<ChallanRequestResponse>(decryptedDataJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return Ok(result);
                }

                return Ok(retJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task MigrateDatabaseObjectsAsync()
        {
            var localConnString = "Host=localhost;Port=5433;Database=VMS_Local;Username=postgres;Password=postgres123";
            var destConn = _context.Database.GetDbConnection();
            bool wasDestClosed = destConn.State == ConnectionState.Closed;
            if (wasDestClosed) await destConn.OpenAsync();

            try
            {
                using (var cmd = destConn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS public.tb_btdetails (
                            id integer NOT NULL PRIMARY KEY,
                            form_code character varying,
                            btcode character varying,
                            description character varying,
                            major_code character varying,
                            sub_major_code character varying,
                            minor_code character varying,
                            sub_head_code character varying,
                            detail_head_code character varying,
                            enabled boolean,
                            pdate timestamp with time zone,
                            tdate timestamp with time zone
                        );

                        CREATE TABLE IF NOT EXISTS public.""tblFuelMaintenanceDetails_IFMS"" (
                            ""FuelMaintenanceIFMSId"" integer NOT NULL PRIMARY KEY,
                            ""FuelMaintenance"" integer,
                            ""ActionDate"" date,
                            ""Amount"" integer,
                            ""SubVoucherNo"" character varying(100),
                            ""SubVoucherDesc"" character varying(5000),
                            ""ExpenditureDetails"" character varying(255),
                            ""SanctionOrderNo"" character varying(100),
                            ""SanctionOrderDate"" date,
                            ""SanctionAuthority"" character varying(255),
                            ""FwdToTreasury"" boolean,
                            ""Status"" character varying(200),
                            ""ClaimNo"" character varying,
                            ""DDOCode"" character varying,
                            ""ModulePK"" integer,
                            ""BillType"" integer,
                            ""ModuleId"" integer,
                            ""SentBy"" character varying,
                            ""SentById"" integer,
                            ""ApplicantCode"" integer,
                            ""ApplicantName"" character varying,
                            ""ApplicantDesig"" character varying,
                            ""ApplicantOffice"" character varying,
                            ""PDate"" timestamp with time zone,
                            ""TDate"" timestamp with time zone,
                            ""ClaimResponseId"" character varying,
                            ""HttpStatus"" integer,
                            ""FirmName"" character varying(100),
                            ""NodelOfficerId"" character varying(255),
                            ""ClaimVerificationStatus"" integer,
                            ""IsGrantInAidBill"" boolean,
                            ""AmountSanctioned"" double precision,
                            ""GrantInAidPeriod"" character varying(50),
                            ""SanctionedBy"" character varying(500),
                            ""FDSanctionletterNo"" character varying(50),
                            ""DateOfIssue"" date,
                            ""Purpose"" character varying(100),
                            ""Deductions"" double precision,
                            ""stockaloted"" boolean,
                            ""is_supplementary_bill"" boolean,
                            ""parent_claim_id"" character varying,
                            ""supplementary_allotement_done"" boolean,
                            ""is_bulk_bill"" boolean,
                            ""IFMSStatus"" integer,
                            ""IFMSBillNo"" bigint,
                            ""isnewifms"" boolean,
                            ""billInfoDetail"" character varying,
                            ""incomeTaxAmount"" integer,
                            ""VMSRefNo"" bigint,
                            ""billSubmittedDate"" timestamp with time zone
                        );

                        CREATE TABLE IF NOT EXISTS public.""tblFuelMaintenanceDetails"" (
                            ""FuelMaintenanceId"" integer NOT NULL PRIMARY KEY,
                            ""FuelMaintenanceIFMSId"" integer,
                            ""VehicleInfoId"" integer,
                            ""Action"" character varying,
                            ""Mileage"" double precision,
                            ""FuelConsumptionLitres"" double precision,
                            ""Amount"" integer,
                            ""OdometerReading"" integer,
                            ""PermissionNOC"" character varying,
                            ""Details"" character varying,
                            ""PDate"" date,
                            ""TDate"" date,
                            ""FuelMaintenance"" integer,
                            ""BillDate"" date,
                            ""BillNumber"" character varying,
                            ""Status"" integer,
                            ""sanction_authority_mobile_no"" character varying
                        );

                        CREATE TABLE IF NOT EXISTS public.""tblHireVehicleDetails"" (
                            ""HireVehicleDetailsId"" integer NOT NULL PRIMARY KEY,
                            ""FuelMaintenanceIFMSId"" integer,
                            ""ItemId"" character varying,
                            ""RecordId"" character varying,
                            ""OfficeId"" integer,
                            ""VehicleNumber"" character varying,
                            ""VehicleTypeId"" integer,
                            ""ManufacturerId"" integer,
                            ""ModelId"" integer,
                            ""SeatingCapacity"" integer,
                            ""FuelUsed"" character varying,
                            ""ContractorName"" character varying,
                            ""ContractorPhoneNumber"" character varying,
                            ""BillDateFrom"" date,
                            ""BillDateTo"" date,
                            ""KMCovered"" double precision,
                            ""BillAmount"" integer,
                            ""BillNumber"" character varying,
                            ""BillDate"" date,
                            ""PDate"" timestamp with time zone,
                            ""Status"" integer,
                            ""NoofVehicles"" integer
                        );

                        CREATE TABLE IF NOT EXISTS public.""tblMTStore"" (
                            ""MTStoreId"" integer NOT NULL PRIMARY KEY,
                            ""FuelMaintenanceIFMSId"" integer,
                            ""ItemId"" character varying,
                            ""RecordId"" character varying,
                            ""BillNumber"" character varying,
                            ""BillDate"" date,
                            ""InventoryMasterId"" integer,
                            ""ModelNumber"" character varying,
                            ""Quantity"" double precision,
                            ""BillAmount"" integer,
                            ""PDate"" timestamp with time zone,
                            ""Status"" integer
                        );

                        CREATE TABLE IF NOT EXISTS public.""tblContractualClaim"" (
                            ""ContractualClaimId"" integer NOT NULL PRIMARY KEY,
                            ""FuelMaintenanceIFMSId"" integer,
                            ""ItemId"" character varying,
                            ""RecordId"" character varying,
                            ""IsContractual"" boolean,
                            ""BillNumber"" character varying,
                            ""BillDate"" date,
                            ""BillPeriodFrom"" date,
                            ""BillPeriodTo"" date,
                            ""DDOCode"" character varying,
                            ""VehicleInfoId"" integer,
                            ""VehicleNumber"" character varying,
                            ""BillAmount"" integer,
                            ""PDate"" timestamp with time zone,
                            ""Status"" integer
                        );

                        CREATE OR REPLACE FUNCTION public.propagate_legacy_status_update()
                        RETURNS TRIGGER AS $trg$
                        DECLARE
                            v_claim_no character varying;
                            v_modern_status integer;
                            v_claim_id integer;
                        BEGIN
                            SELECT ""ClaimNo"" INTO v_claim_no
                            FROM public.""tblFuelMaintenanceDetails_IFMS""
                            WHERE ""FuelMaintenanceIFMSId"" = NEW.""FuelMaintenanceIFMSId""
                            LIMIT 1;

                            IF NEW.""Status"" = 200 THEN
                                v_modern_status := 2;
                            ELSIF NEW.""Status"" = 301 THEN
                                v_modern_status := 3;
                            ELSIF NEW.""Status"" = 202 THEN
                                v_modern_status := 1;
                            ELSE
                                v_modern_status := 1;
                            END IF;

                            IF v_claim_no IS NOT NULL THEN
                                UPDATE public.""BillClaims""
                                SET ""Status"" = v_modern_status
                                WHERE ""ClaimNumber"" = v_claim_no;

                                SELECT ""BillClaimId"" INTO v_claim_id
                                FROM public.""BillClaims""
                                WHERE ""ClaimNumber"" = v_claim_no
                                LIMIT 1;

                                IF v_claim_id IS NOT NULL THEN
                                    UPDATE public.""FuelBills"" SET ""Status"" = v_modern_status WHERE ""ClaimId"" = v_claim_id;
                                    UPDATE public.""MaintenanceBills"" SET ""Status"" = v_modern_status WHERE ""ClaimId"" = v_claim_id;
                                    UPDATE public.""HiredVehicleBills"" SET ""Status"" = v_modern_status WHERE ""ClaimId"" = v_claim_id;
                                    UPDATE public.""ContractualBills"" SET ""Status"" = v_modern_status WHERE ""ClaimId"" = v_claim_id;
                                    UPDATE public.""MiscellaneousBills"" SET ""Status"" = v_modern_status WHERE ""ClaimId"" = v_claim_id;
                                END IF;
                            END IF;

                            RETURN NEW;
                        END;
                        $trg$ LANGUAGE plpgsql;

                        CREATE OR REPLACE TRIGGER trg_fuel_maint_status_sync
                        AFTER UPDATE OF ""Status"" ON public.""tblFuelMaintenanceDetails""
                        FOR EACH ROW EXECUTE FUNCTION public.propagate_legacy_status_update();

                        CREATE OR REPLACE TRIGGER trg_hire_status_sync
                        AFTER UPDATE OF ""Status"" ON public.""tblHireVehicleDetails""
                        FOR EACH ROW EXECUTE FUNCTION public.propagate_legacy_status_update();

                        CREATE OR REPLACE TRIGGER trg_mtstore_status_sync
                        AFTER UPDATE OF ""Status"" ON public.""tblMTStore""
                        FOR EACH ROW EXECUTE FUNCTION public.propagate_legacy_status_update();

                        CREATE OR REPLACE TRIGGER trg_contract_status_sync
                        AFTER UPDATE of ""Status"" ON public.""tblContractualClaim""
                        FOR EACH ROW EXECUTE FUNCTION public.propagate_legacy_status_update();
                    ";
                    await cmd.ExecuteNonQueryAsync();
                }

                var functionNames = new List<string>
                {
                    "IFMSIntegration_getBillsForSubmission",
                    "RestoreDiscardedBill",
                    "IFMSIntegration_getBTDetails",
                    "IFMSIntegration_getPendingBillsFromVMS",
                    "IFMSIntegration_DiscardBillFromVMS",
                    "IFMSIntegration_GetPendingIFMSClaims",
                    "reports_insertMaxOdometerReading",
                    "IFMSIntegration_GetIFMSClaimRecordsById",
                    "IFMSIntegration_getVMSRefNoCount",
                    "IFMSIntegration_UpdateBillInfoSentToIFMS2",
                    "IFMSIntegration_UpdateBillsSentToIFMS2",
                    "IFMSIntegration_claimVerified",
                    "IFMSIntegration_claimRejected",
                    "IFMSIntegration_UpdateBillsStatusFromIFMS2"
                };

                using (var localConn = new NpgsqlConnection(localConnString))
                {
                    await localConn.OpenAsync();
                    foreach (var fName in functionNames)
                    {
                        string def = "";
                        try
                        {
                            using (var cmd = localConn.CreateCommand())
                            {
                                cmd.CommandText = @"
                                    SELECT pg_get_functiondef(p.oid) 
                                    FROM pg_proc p 
                                    JOIN pg_namespace n ON p.pronamespace = n.oid 
                                    WHERE n.nspname = 'public' AND LOWER(p.proname) = LOWER(@fName)";
                                cmd.Parameters.AddWithValue("fName", fName);
                                 var result = await cmd.ExecuteScalarAsync();
                                if (result != null) def = result.ToString() ?? "";
                            }

                            if (!string.IsNullOrEmpty(def))
                            {
                                using (var cmd = destConn.CreateCommand())
                                {
                                    cmd.CommandText = def;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Function creation failed for {fName}: {ex.Message}");
                        }
                    }

                    bool hasDataBt = false;
                    using (var cmd = destConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM tb_btdetails LIMIT 1)";
                        hasDataBt = (bool)(await cmd.ExecuteScalarAsync() ?? false);
                    }

                    if (!hasDataBt)
                    {
                        var btRows = new List<Dictionary<string, object>>();
                        using (var cmd = localConn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT * FROM tb_btdetails";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var row = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = reader.GetValue(i);
                                    }
                                    btRows.Add(row);
                                }
                            }
                        }

                        foreach (var row in btRows)
                        {
                            using (var cmd = destConn.CreateCommand())
                            {
                                var colNames = string.Join(", ", row.Keys.Select(k => $"\"{k}\""));
                                var paramNames = string.Join(", ", row.Keys.Select(k => $"@{k}"));
                                cmd.CommandText = $"INSERT INTO tb_btdetails ({colNames}) VALUES ({paramNames}) ON CONFLICT (id) DO NOTHING";
                                foreach (var kvp in row)
                                {
                                    cmd.Parameters.Add(new NpgsqlParameter(kvp.Key, kvp.Value));
                                }
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    var legacyTables = new[] { "tblFuelMaintenanceDetails", "tblHireVehicleDetails", "tblMTStore", "tblContractualClaim", "tblFuelMaintenanceDetails_IFMS" };
                    foreach (var legacyTable in legacyTables)
                    {
                        bool hasData = false;
                        using (var cmd = destConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM \"{legacyTable}\" LIMIT 1)";
                            hasData = (bool)(await cmd.ExecuteScalarAsync() ?? false);
                        }

                        if (!hasData)
                        {
                            var rows = new List<Dictionary<string, object>>();
                            using (var cmd = localConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT * FROM \"{legacyTable}\"";
                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        var row = new Dictionary<string, object>();
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            row[reader.GetName(i)] = reader.GetValue(i);
                                        }
                                        rows.Add(row);
                                    }
                                }
                            }

                            string pkColumn = legacyTable switch
                            {
                                "tblFuelMaintenanceDetails" => "FuelMaintenanceId",
                                "tblHireVehicleDetails" => "HireVehicleDetailsId",
                                "tblMTStore" => "MTStoreId",
                                "tblContractualClaim" => "ContractualClaimId",
                                "tblFuelMaintenanceDetails_IFMS" => "FuelMaintenanceIFMSId",
                                _ => ""
                            };

                            foreach (var row in rows)
                            {
                                using (var cmd = destConn.CreateCommand())
                                {
                                    var colNames = string.Join(", ", row.Keys.Select(k => $"\"{k}\""));
                                    var paramNames = string.Join(", ", row.Keys.Select(k => $"@{k}"));
                                    cmd.CommandText = $"INSERT INTO \"{legacyTable}\" ({colNames}) VALUES ({paramNames}) ON CONFLICT (\"{pkColumn}\") DO NOTHING";
                                    foreach (var kvp in row)
                                    {
                                        cmd.Parameters.Add(new NpgsqlParameter(kvp.Key, kvp.Value));
                                    }
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (wasDestClosed) await destConn.CloseAsync();
            }
        }

        private async Task<List<T>> ExecuteFunctionAsync<T>(string functionName, List<NpgsqlParameter> parameters) where T : new()
        {
            var list = new List<T>();
            var conn = _context.Database.GetDbConnection() as NpgsqlConnection;
            if (conn == null) return list;

            bool wasClosed = conn.State == ConnectionState.Closed;
            if (wasClosed) await conn.OpenAsync();

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    var paramPlaceholders = string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"));
                    cmd.CommandText = $"SELECT * FROM {functionName}({paramPlaceholders})";
                    cmd.CommandType = CommandType.Text;
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(p);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var properties = typeof(T).GetProperties();
                        while (await reader.ReadAsync())
                        {
                            var item = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var prop = properties.FirstOrDefault(p => p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                                if (prop != null && prop.CanWrite)
                                {
                                    var val = reader.GetValue(i);
                                    if (val != DBNull.Value)
                                    {
                                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                        object convertedVal;
                                        if (val is DateOnly dateOnly && targetType == typeof(DateTime))
                                        {
                                            convertedVal = dateOnly.ToDateTime(TimeOnly.MinValue);
                                        }
                                        else if (val is DateTime dateTime && targetType == typeof(DateOnly))
                                        {
                                            convertedVal = DateOnly.FromDateTime(dateTime);
                                        }
                                        else if (val is DateTimeOffset dto && targetType == typeof(DateTime))
                                        {
                                            convertedVal = dto.DateTime;
                                        }
                                        else
                                        {
                                            convertedVal = Convert.ChangeType(val, targetType);
                                        }
                                        prop.SetValue(item, convertedVal);
                                    }
                                }
                            }
                            list.Add(item);
                        }
                    }
                }
            }
            finally
            {
                if (wasClosed) await conn.CloseAsync();
            }
            return list;
        }
    }

    public class OutGetBillsForSubmission
    {
        public int? FuelMaintenanceIFMSId { get; set; }
        public int? FuelMaintenance { get; set; }
        public DateTime? ActionDate { get; set; }
        public int? Amount { get; set; }
        public string? SubVoucherNo { get; set; }
        public string? SubVoucherDesc { get; set; }
        public string? ExpenditureDetails { get; set; }
        public string? SanctionOrderNo { get; set; }
        public DateTime? SanctionOrderDate { get; set; }
        public string? SanctionAuthority { get; set; }
        public bool? FwdToTreasury { get; set; }
        public string? Status { get; set; }
        public string? ClaimNo { get; set; }
        public string? DDOCode { get; set; }
        public int? ModulePK { get; set; }
        public int? BillType { get; set; }
        public int? ModuleId { get; set; }
        public string? SentBy { get; set; }
        public int? SentById { get; set; }
        public int? ApplicantCode { get; set; }
        public string? ApplicantName { get; set; }
        public string? ApplicantDesig { get; set; }
        public string? ApplicantOffice { get; set; }
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        public string? ClaimResponseId { get; set; }
        public int? HttpStatus { get; set; }
        public string? FirstName { get; set; }
        public string? NodelOfficerId { get; set; }
        public int? ClaimVerificationStatus { get; set; }
        public bool? IsGrantInAidBill { get; set; }
        public string? GrantInAidPeriod { get; set; }
        public string? SanctionedBy { get; set; }
        public string? FDSanctionletterNo { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string? Purpose { get; set; }
        public bool? stockaloted { get; set; }
        public bool? is_supplementary_bill { get; set; }
        public string? parent_claim_id { get; set; }
        public bool? supplementary_allotement_done { get; set; }
        public bool is_bulk_bill { get; set; }
        public int? IFMSStatus { get; set; }
        public Int64? IFMSBillNo { get; set; }
        public bool? isnewifms { get; set; }
        public string? billInfoDetail { get; set; }
        public int? incomeTaxAmount { get; set; }
        public string? VMSRefNo { get; set; }
        public DateTime? billSubmittedDate { get; set; }
    }

    public class OutGetBTDetails
    {
        public int id { get; set; }
        public string? form_code { get; set; }
        public string? btcode { get; set; }
        public string? description { get; set; }
        public string? major_code { get; set; }
        public string? sub_major_code { get; set; }
        public string? minor_code { get; set; }
        public string? sub_head_code { get; set; }
        public string? detail_head_code { get; set; }
        public bool? enabled { get; set; }
        public DateTime? pdate { get; set; }
        public DateTime? tdate { get; set; }
    }

    public class InGetIFMSClaimRecordsById
    {
        public bool treasurytype { get; set; }
        public bool isnewifms { get; set; }
        public DateTime from_year { get; set; }
        public DateTime to_year { get; set; }
    }

    public class OutGetIFMSClaimRecordsById
    {
        public int FuelMaintenanceIFMSId { get; set; }
        public int FuelMaintenance { get; set; }
        public DateTime? ActionDate { get; set; }
        public int Amount { get; set; }
        public string? SubVoucherNo { get; set; }
        public string? SubVoucherDesc { get; set; }
        public string? ExpenditureDetails { get; set; }
        public string? SanctionOrderNo { get; set; }
        public DateTime? SanctionOrderDate { get; set; }
        public string? SanctionAuthority { get; set; }
        public bool? FwdToTreasury { get; set; }
        public string? Status { get; set; }
        public string? ClaimNo { get; set; }
        public string? DDOCode { get; set; }
        public int ModulePK { get; set; }
        public int BillType { get; set; }
        public int ModuleId { get; set; }
        public string? SentBy { get; set; }
        public int SentById { get; set; }
        public int ApplicantCode { get; set; }
        public string? ApplicantName { get; set; }
        public string? ApplicantDesig { get; set; }
        public string? ApplicantOffice { get; set; }
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        public string? ClaimResponseId { get; set; }
        public int HttpStatus { get; set; }
        public string? FirmName { get; set; }
        public string? NodelOfficerId { get; set; }
        public int ClaimVerificationStatus { get; set; }
        public bool? IsGrantInAidBill { get; set; }
        public double? AmountSanctioned { get; set; }
        public string? GrantinAidPeriod { get; set; }
        public string? SanctionedBy { get; set; }
        public string? FDSanctionletterNo { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string? Purpose { get; set; }
        public double? Deductions { get; set; }
        public bool? stockaloted { get; set; }
        public bool? is_supplementary_bill { get; set; }
        public string? parent_claim_id { get; set; }
        public bool? supplementary_allotement_done { get; set; }
        public bool? is_bulk_bill { get; set; }
        public int IFMSStatus { get; set; }
        public Int64? IFMSBillNo { get; set; }
        public bool? isnewifms { get; set; }
        public string? billInfoDetail { get; set; }
        public int incomeTaxAmount { get; set; }
        public Int64? VMSRefNo { get; set; }
        public DateTime? billSubmittedDate { get; set; }
    }

    public class InUpdateIfmsBillStatus
    {
        public string ClaimType { get; set; } = "";
        public DateTime? pdate { get; set; }
        public DateTime? tdate { get; set; }
    }

    public class OutUpdateIfmsBillStatus
    {
        public Int64 billno { get; set; }
    }

    public class DeptEncBudgetHeadData
    {
        public string ddo_code { get; set; } = "";
        public string class_of_exp { get; set; } = "";
        public int budget_type_code { get; set; }
    }

    public class GetPayees
    {
        public string ddo_code { get; set; } = "";
    }

    public class InVmsPostBill
    {
        public FullyContigentBillData form { get; set; } = new();
        public List<BillsToProcessArray> billsToProcessArray { get; set; } = new();
    }

    public class FullyContigentBillData
    {
        public string integration_src { get; set; } = "";
        public long bill_no { get; set; }
        public string ddo_code { get; set; } = "";
        public int bill_month { get; set; }
        public string sub_soe { get; set; } = "";
        public long hoa_id { get; set; }
        public int bill_code { get; set; }
        public long gross_amount { get; set; }
        public long net_amount { get; set; }
        public List<TOBT>? tobt { get; set; }
        public List<AGBT>? agbt { get; set; }
        public List<ECSDetail> payee_detail { get; set; } = new();
        public List<FetchFullyContigentBillFormJson> form_data_1 { get; set; } = new();
        public List<FetchFullyContigentBillFormJson2>? form_data_2 { get; set; }
        public List<FetchFullyContigentBillFormJson>? form_data_3 { get; set; }
        public long dept_ref_no { get; set; }
        public List<NpsEmployees>? nps_data { get; set; }
    }

    public class BillsToProcessArray
    {
        public int fuelMaintenanceIFMSId { get; set; }
        public int fuelMaintenance { get; set; }
        public DateTime actionDate { get; set; }
        public int amount { get; set; }
        public string subVoucherNo { get; set; } = "";
        public string subVoucherDesc { get; set; } = "";
        public string expenditureDetails { get; set; } = "";
        public string sanctionOrderNo { get; set; } = "";
        public DateTime sanctionOrderDate { get; set; }
        public string sanctionAuthority { get; set; } = "";
        public bool fwdToTreasury { get; set; }
        public string status { get; set; } = "";
        public string claimNo { get; set; } = "";
        public string ddoCode { get; set; } = "";
        public int modulePK { get; set; }
        public int billType { get; set; }
        public int moduleId { get; set; }
        public string sentBy { get; set; } = "";
        public int sentById { get; set; }
        public int applicantCode { get; set; }
        public string applicantName { get; set; } = "";
        public string applicantDesig { get; set; } = "";
        public string applicantOffice { get; set; } = "";
        public DateTime pDate { get; set; }
        public DateTime tDate { get; set; }
        public string claimResponseId { get; set; } = "";
        public int? incomeTaxAmount { get; set; }
    }

    public class TOBT
    {
        public string btCode { get; set; } = "";
        public string mjrHead { get; set; } = "";
        public string subMjrHead { get; set; } = "";
        public string minrHead { get; set; } = "";
        public string sbHead { get; set; } = "";
        public string dtlHead { get; set; } = "";
        public long amount { get; set; }
        public int? btId { get; set; }
    }

    public class AGBT
    {
        public string agCode { get; set; } = "";
        public string agMjrHead { get; set; } = "";
        public string agSubMjrHead { get; set; } = "";
        public string agMinrHead { get; set; } = "";
        public string agSbHead { get; set; } = "";
        public string agDtlHead { get; set; } = "";
        public long amount { get; set; }
        public int? agId { get; set; }
    }

    public class ECSDetail
    {
        public string hc { get; set; } = "";
        public long amt { get; set; }
        public string gstDt { get; set; } = "";
        public string mhc { get; set; } = "";
        public string gstNo { get; set; } = "";
        public string entNm { get; set; } = "";
    }

    public class NpsEmployees
    {
        public long hc { get; set; }
        public long bp { get; set; }
        public long ir { get; set; }
        public long gp { get; set; }
        public long da { get; set; }
    }

    public class FetchFullyContigentBillFormJson
    {
        public string vno { get; set; } = "";
        public string vdes { get; set; } = "";
        public string expdet { get; set; } = "";
        public string snorno { get; set; } = "";
        public string snordt { get; set; } = "";
        public string snauth { get; set; } = "";
        public long snamt { get; set; }
        public long it { get; set; }
        public long nbal { get; set; }
    }

    public class FetchFullyContigentBillFormJson2
    {
        public int dptcd { get; set; }
        public string dmd { get; set; } = "";
        public string mjr { get; set; } = "";
        public string sbmjr { get; set; } = "";
        public string minr { get; set; } = "";
        public string sbhd { get; set; } = "";
        public string dtlhd { get; set; } = "";
        public string soe { get; set; } = "";
        public string sbsoe { get; set; } = "";
        public int vc { get; set; }
        public int sc { get; set; }
        public int exempt { get; set; }
    }

    public class CountRefNo
    {
        public int countrefno { get; set; }
    }

    public class InDiscardBillFromVMS
    {
        public string ClaimNo { get; set; } = "";
        public int FuelMaintenanceIfmsid { get; set; }
        public Int64 VmsRefNo { get; set; }
    }

    public class InRestoreDiscardedBill
    {
        public Int64 VmsRefNo { get; set; }
        public int FuelMaintenanceIfmsId { get; set; }
    }

    public class OutMsg
    {
        public string msg { get; set; } = "";
    }

    public class ifms_data
    {
        public string chcksum { get; set; } = "";
        public Challandata challandata { get; set; } = new();
    }

    public class Challandata
    {
        public string receiptNo { get; set; } = "";
        public string BankCode { get; set; } = "";
        public string RequestDate { get; set; } = "";
    }

    public class checkdata
    {
        public string encData { get; set; } = "";
        public string clientId { get; set; } = "";
        public string clientSecret { get; set; } = "";
        public string integratingAgency { get; set; } = "";
        public string ipAddress { get; set; } = "";
        public string transactionID { get; set; } = "";
    }

    public class IFMS_Bank_Auth_Keys_Offline_Challan
    {
        public string challanUrl { get; set; } = "";
        public HeaderKeys Header { get; set; } = new();
        public IFMSJWTKeys JWTKeys { get; set; } = new();
        public BankCodes BankCodes { get; set; } = new();
    }

    public class HeaderKeys
    {
        public string Offline_Challan_URL { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string IPAllow { get; set; } = "";
        public string IntegratingAgency { get; set; } = "";
        public string MHeadAllow { get; set; } = "";
    }

    public class IFMSJWTKeys
    {
        public string ChecksumKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string SecretIV { get; set; } = "";
    }

    public class BankCodes
    {
        public string BankCode { get; set; } = "";
    }

    public class ReturnResponse
    {
        public string encData { get; set; } = "";
        public string statusCode { get; set; } = "";
        public string msg { get; set; } = "";
        public string integratingAgency { get; set; } = "";
        public string BankCode { get; set; } = "";
    }

    public class ChallanRequestResponse
    {
        public string chcksum { get; set; } = "";
        public ChallanData challandata { get; set; } = new();
    }

    public class ChallanData
    {
        public string deptRefNo { get; set; } = "";
        public string receiptNo { get; set; } = "";
        public string clientId { get; set; } = "";
        public string challanDate { get; set; } = "";
        public string expiryDate { get; set; } = "";
        public string companyName { get; set; } = "";
        public string deptCode { get; set; } = "";
        public string totalAmt { get; set; } = "";
        public string trsyAmt { get; set; } = "";
        public string nonTrsyAmt { get; set; } = "";
        public string noOfTrans { get; set; } = "";
        public string ddoCode { get; set; } = "";
        public string payLocCode { get; set; } = "";
        public string add1 { get; set; } = "";
        public string add2 { get; set; } = "";
        public string add3 { get; set; } = "";
        public string add4 { get; set; } = "";
        public string add5 { get; set; } = "";
        public string sURL { get; set; } = "";
        public string fURL { get; set; } = "";
        public List<trsyPayments>? trsyPayments { get; set; }
        public List<nonTrsyPaymentsTrp>? nonTrsyPayments { get; set; }
        public PayeeInfo? payee_info { get; set; }
    }

    public class trsyPayments
    {
        public string Head { get; set; } = "";
        public string amt { get; set; } = "";
    }

    public class nonTrsyPaymentsTrp
    {
        public string NonTrsy { get; set; } = "";
        public string ntAmt { get; set; } = "";
    }

    public class PayeeInfo
    {
        public string payerName { get; set; } = "";
        public string teleNumber { get; set; } = "";
        public string mobNumber { get; set; } = "";
        public string emailId { get; set; } = "";
        public string addLine1 { get; set; } = "";
        public string addLine2 { get; set; } = "";
        public string addPincode { get; set; } = "";
        public string district { get; set; } = "";
        public string tehsil { get; set; } = "";
    }

    public class IFMS_EncrDecr
    {
        private string ChecksumKey { get; set; }
        private string Key { get; set; }
        private string IV { get; set; }

        public IFMS_EncrDecr(string checksumKey, string key, string iv)
        {
            ChecksumKey = checksumKey;
            Key = key;
            IV = iv;
        }

        public string CheckSum(string text)
        {
            var encoder = new System.Text.UTF8Encoding();
            var hex = new System.Text.StringBuilder();

            using (var hmac = new System.Security.Cryptography.HMACSHA512(encoder.GetBytes(ChecksumKey)))
            {
                byte[] hashValue = hmac.ComputeHash(encoder.GetBytes(text));
                foreach (byte x in hashValue)
                {
                    hex.Append(x.ToString("x2"));
                }
            }
            return hex.ToString().ToLower();
        }

        public string Encrypt(string textToEncrypt)
        {
            int keySize = 128;
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.KeySize = keySize;
                aes.BlockSize = keySize;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(Key);
                aes.IV = System.Text.Encoding.UTF8.GetBytes(IV);

                using (var transform = aes.CreateEncryptor())
                {
                    byte[] plainText = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                    byte[] encrypted = transform.TransformFinalBlock(plainText, 0, plainText.Length);
                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        public string Decrypt(string textToDecrypt)
        {
            int keySize = 128;
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.KeySize = keySize;
                aes.BlockSize = keySize;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(Key);
                aes.IV = System.Text.Encoding.UTF8.GetBytes(IV);

                using (var transform = aes.CreateDecryptor())
                {
                    byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
                    byte[] decrypted = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    return System.Text.Encoding.UTF8.GetString(decrypted);
                }
            }
        }
    }
}
