using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;

        public CaptchaController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        // GET /api/captcha/generate
        [HttpGet("generate")]
        public IActionResult GenerateCaptcha()
        {
            var (captchaId, imageBytes) = _captchaService.GenerateCaptcha();

            // Return captcha ID in header and image in body
            Response.Headers.Append("X-Captcha-Id", captchaId);
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Captcha-Id");

            return File(imageBytes, "image/png");
        }

        // POST /api/captcha/validate
        [HttpPost("validate")]
        public IActionResult ValidateCaptcha([FromBody] ValidateCaptchaRequest request)
        {
            var isValid = _captchaService.ValidateCaptcha(request.CaptchaId, request.UserInput);

            if (!isValid)
            {
                return BadRequest(new { message = "Invalid or expired captcha" });
            }

            return Ok(new { message = "Captcha validated successfully" });
        }
    }

    public record ValidateCaptchaRequest(string CaptchaId, string UserInput);
}
