using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Masters;
using backend.Services;

namespace backend.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMN,DDO")]
    public class SecretariesController : ControllerBase
    {
        private readonly ISecretaryService _secretaryService;

        public SecretariesController(ISecretaryService secretaryService)
        {
            _secretaryService = secretaryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SecretaryDto>>> GetAllSecretaries()
        {
            var secretaries = await _secretaryService.GetAllSecretariesAsync();
            return Ok(secretaries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SecretaryDto>> GetSecretaryById(int id)
        {
            var secretary = await _secretaryService.GetSecretaryByIdAsync(id);
            if (secretary == null) return NotFound();
            return Ok(secretary);
        }

        [HttpPost]
        public async Task<ActionResult<SecretaryDto>> CreateSecretary(CreateSecretaryDto dto)
        {
            var created = await _secretaryService.CreateSecretaryAsync(dto);
            return CreatedAtAction(nameof(GetSecretaryById), new { id = created.SecretaryId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SecretaryDto>> UpdateSecretary(int id, UpdateSecretaryDto dto)
        {
            var updated = await _secretaryService.UpdateSecretaryAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSecretary(int id)
        {
            var result = await _secretaryService.DeleteSecretaryAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleSecretaryStatus(int id)
        {
            var result = await _secretaryService.ToggleSecretaryStatusAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Status toggled successfully." });
        }
    }
}
