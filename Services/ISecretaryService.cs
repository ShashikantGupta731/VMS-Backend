using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs.Masters;

namespace backend.Services
{
    public interface ISecretaryService
    {
        Task<IEnumerable<SecretaryDto>> GetAllSecretariesAsync();
        Task<SecretaryDto?> GetSecretaryByIdAsync(int id);
        Task<SecretaryDto> CreateSecretaryAsync(CreateSecretaryDto dto);
        Task<SecretaryDto?> UpdateSecretaryAsync(int id, UpdateSecretaryDto dto);
        Task<bool> DeleteSecretaryAsync(int id);
        Task<bool> ToggleSecretaryStatusAsync(int id);
    }
}
