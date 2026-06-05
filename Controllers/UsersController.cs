using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Core;
using backend.DTOs.User;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public UsersController(IUserService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var deptIdClaim = User.FindFirst("departmentId")?.Value;
            var distIdClaim = User.FindFirst("districtId")?.Value;

            if (userIdClaim == null || roleClaim == null) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);
            string currentRole = roleClaim;
            int? deptId = !string.IsNullOrEmpty(deptIdClaim) ? int.Parse(deptIdClaim) : null;
            int? distId = !string.IsNullOrEmpty(distIdClaim) ? int.Parse(distIdClaim) : null;

            var users = await _userService.GetUsersAsync(currentUserId, currentRole, deptId, distId);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("ddo-list")]
        public async Task<ActionResult<List<DdoResponseDto>>> GetDdoList([FromQuery] int? deptId, [FromQuery] int? districtId, [FromQuery] int? userId, [FromQuery] string? role)
        {
            var ddos = await _userService.GetAllDdoInformationAsync(deptId, districtId, userId, role);
            return Ok(ddos);
        }
        
        [HttpGet("check-username")]
        public async Task<ActionResult> CheckUsername([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { available = false, message = "Username is required" });
            var available = await _userService.CheckUsernameAvailabilityAsync(username);
            return Ok(new { available });
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = "ADMN")]
        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            var result = await _userService.AdminResetPasswordAsync(id, newPassword);
            if (!result) return NotFound();
            return Ok(new { success = true, message = "Password reset successful" });
        }

        [Authorize(Roles = "ADMN")]
        [HttpPost("{id}/toggle-status")]
        public async Task<ActionResult> ToggleStatus(int id)
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            if (!result) return NotFound();
            return Ok(new { success = true, message = "User status updated" });
        }
        [Authorize(Roles = "ADMN")]
        [HttpGet("activity-logs")]
        public async Task<ActionResult> GetActivityLogs([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.UserActivityLogs.AsQueryable();

            if (fromDate.HasValue)
            {
                var utcFrom = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(x => x.CreatedOn >= utcFrom);
            }
            
            if (toDate.HasValue)
            {
                var utcTo = DateTime.SpecifyKind(toDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(x => x.CreatedOn <= utcTo);
            }

            var totalCount = await query.CountAsync();
            var logs = await query.OrderByDescending(x => x.CreatedOn)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return Ok(new { data = logs, totalCount, page, pageSize });
        }

        [Authorize(Roles = "ADMN")]
        [HttpGet("error-logs")]
        public async Task<ActionResult> GetErrorLogs([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.ErrorLogs.AsQueryable();

            if (fromDate.HasValue)
            {
                var utcFrom = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(x => x.ErrDate >= utcFrom);
            }
            
            if (toDate.HasValue)
            {
                var utcTo = DateTime.SpecifyKind(toDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(x => x.ErrDate <= utcTo);
            }

            var totalCount = await query.CountAsync();
            var logs = await query.OrderByDescending(x => x.ErrDate)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return Ok(new { data = logs, totalCount, page, pageSize });
        }
    }
}

