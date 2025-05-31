using PosStore.DTOs;

namespace PosStore.Services.Interfaces
{
    public interface IWeddingGuestService
    {
        Task<IEnumerable<WeddingGuestDto>> GetAllWeddingGuestsAsync();
        Task<PagedResult<WeddingGuestDto>> GetWeddingGuestsAsync(WeddingGuestQueryDto queryDto);
        Task<WeddingGuestDto?> GetWeddingGuestByIdAsync(long id);
        Task<WeddingGuestDto> CreateWeddingGuestAsync(CreateWeddingGuestDto createDto);
        Task<WeddingGuestDto?> UpdateWeddingGuestAsync(long id, UpdateWeddingGuestDto updateDto);
        Task<bool> DeleteWeddingGuestAsync(long id);
        Task<IEnumerable<WeddingGuestDto>> SearchWeddingGuestsAsync(string searchTerm);
        Task<IEnumerable<WeddingGuestDto>> GetWeddingGuestsByStatusAsync(string status);
        Task<IEnumerable<WeddingGuestDto>> GetWeddingGuestsByUnitAsync(string unit);
        Task<IEnumerable<WeddingGuestDto>> GetWeddingGuestsByRelationshipAsync(string relationship);
        Task<IEnumerable<WeddingGuestSummaryDto>> GetWeddingGuestsSummaryAsync();
        Task<WeddingGuestStatisticsDto> GetWeddingGuestStatisticsAsync();
        Task<bool> ConfirmGuestAsync(long id);
        Task<bool> DeclineGuestAsync(long id);
        Task<IEnumerable<WeddingGuestDto>> GetPendingGuestsAsync();
        Task<IEnumerable<WeddingGuestDto>> GetConfirmedGuestsAsync();
        Task<IEnumerable<WeddingGuestDto>> GetRecentlyAddedGuestsAsync(int days = 7);
    }
}
