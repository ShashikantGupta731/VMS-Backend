using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Masters;
using backend.Models.Masters;

namespace backend.Services
{
    public class SecretaryService : ISecretaryService
    {
        private readonly AppDbContext _context;

        public SecretaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SecretaryDto>> GetAllSecretariesAsync()
        {
            return await _context.Secretaries
                .Include(s => s.Department)
                .Select(s => new SecretaryDto
                {
                    SecretaryId = s.SecretaryId,
                    Title = s.Title,
                    EmailId = s.EmailId,
                    DeptId = s.DeptId,
                    DepartmentName = s.Department != null ? s.Department.DeptName : string.Empty,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<SecretaryDto?> GetSecretaryByIdAsync(int id)
        {
            var s = await _context.Secretaries
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SecretaryId == id);

            if (s == null) return null;

            return new SecretaryDto
            {
                SecretaryId = s.SecretaryId,
                Title = s.Title,
                EmailId = s.EmailId,
                DeptId = s.DeptId,
                DepartmentName = s.Department != null ? s.Department.DeptName : string.Empty,
                IsActive = s.IsActive
            };
        }

        public async Task<SecretaryDto> CreateSecretaryAsync(CreateSecretaryDto dto)
        {
            var secretary = new Secretary
            {
                Title = dto.Title,
                EmailId = dto.EmailId,
                DeptId = dto.DeptId,
                IsActive = dto.IsActive
            };

            _context.Secretaries.Add(secretary);
            await _context.SaveChangesAsync();

            return await GetSecretaryByIdAsync(secretary.SecretaryId) ?? throw new System.Exception("Failed to load created secretary.");
        }

        public async Task<SecretaryDto?> UpdateSecretaryAsync(int id, UpdateSecretaryDto dto)
        {
            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary == null) return null;

            secretary.Title = dto.Title;
            secretary.EmailId = dto.EmailId;
            secretary.DeptId = dto.DeptId;
            secretary.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return await GetSecretaryByIdAsync(id);
        }

        public async Task<bool> DeleteSecretaryAsync(int id)
        {
            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary == null) return false;

            _context.Secretaries.Remove(secretary);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleSecretaryStatusAsync(int id)
        {
            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary == null) return false;

            secretary.IsActive = !secretary.IsActive;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
