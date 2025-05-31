using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosStore.Data;
using PosStore.DTOs;
using PosStore.Models;
using PosStore.Services.Interfaces;

namespace PosStore.Services.Service
{
    public class WeddingGuestService : IWeddingGuestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public WeddingGuestService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetAllWeddingGuestsAsync()
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<PagedResult<WeddingGuestDto>> GetWeddingGuestsAsync(WeddingGuestQueryDto queryDto)
        {
            IQueryable<WeddingGuest> query = _context.WeddingGuests
                .Where(g => g.IsActive);

            // Apply search filter
            if (!string.IsNullOrEmpty(queryDto.SearchTerm))
            {
                query = query.Where(g =>
                    g.Name.Contains(queryDto.SearchTerm) ||
                    (g.Unit != null && g.Unit.Contains(queryDto.SearchTerm)));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(queryDto.Status))
            {
                if (Enum.TryParse<GuestStatus>(queryDto.Status, true, out var status))
                {
                    query = query.Where(g => g.Status == status);
                }
            }

            // Apply unit filter
            if (!string.IsNullOrEmpty(queryDto.Unit))
            {
                query = query.Where(g => g.Unit == queryDto.Unit);
            }

            // Apply relationship filter
            if (!string.IsNullOrEmpty(queryDto.Relationship))
            {
                if (Enum.TryParse<RelationshipType>(queryDto.Relationship, true, out var relationship))
                {
                    query = query.Where(g => g.Relationship == relationship);
                }
            }

            // Apply sorting
            switch (queryDto.SortBy.ToLower())
            {
                case "name":
                    query = queryDto.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(g => g.Name)
                        : query.OrderBy(g => g.Name);
                    break;
                case "unit":
                    query = queryDto.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(g => g.Unit)
                        : query.OrderBy(g => g.Unit);
                    break;
                case "giftamount":
                    query = queryDto.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(g => g.GiftAmount)
                        : query.OrderBy(g => g.GiftAmount);
                    break;
                case "createddate":
                    query = queryDto.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(g => g.CreatedDate)
                        : query.OrderBy(g => g.CreatedDate);
                    break;
                default:
                    query = query.OrderBy(g => g.Name);
                    break;
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var guests = await query
                .Skip((queryDto.Page - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize)
                .ToListAsync();

            var guestDtos = _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / queryDto.PageSize);

            return new PagedResult<WeddingGuestDto>
            {
                Data = guestDtos,
                Pagination = new PaginationInfo
                {
                    CurrentPage = queryDto.Page,
                    PageSize = queryDto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPrevious = queryDto.Page > 1,
                    HasNext = queryDto.Page < totalPages
                }
            };
        }

        public async Task<WeddingGuestDto?> GetWeddingGuestByIdAsync(Guid id)
        {
            var guest = await _context.WeddingGuests
                .FirstOrDefaultAsync(g => g.Id == id && g.IsActive);

            return guest == null ? null : _mapper.Map<WeddingGuestDto>(guest);
        }

        public async Task<WeddingGuestDto> CreateWeddingGuestAsync(CreateWeddingGuestDto createDto)
        {
            var guest = _mapper.Map<WeddingGuest>(createDto);
            guest.Id = Guid.NewGuid();
            guest.CreatedDate = DateTime.UtcNow;
            guest.UpdatedDate = DateTime.UtcNow;

            _context.WeddingGuests.Add(guest);
            await _context.SaveChangesAsync();

            return _mapper.Map<WeddingGuestDto>(guest);
        }

        public async Task<WeddingGuestDto?> UpdateWeddingGuestAsync(Guid id, UpdateWeddingGuestDto updateDto)
        {
            var guest = await _context.WeddingGuests.FindAsync(id);
            if (guest == null || !guest.IsActive)
                return null;

            _mapper.Map(updateDto, guest);
            guest.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<WeddingGuestDto>(guest);
        }

        public async Task<bool> DeleteWeddingGuestAsync(Guid id)
        {
            var guest = await _context.WeddingGuests.FindAsync(id);
            if (guest == null)
                return false;

            // Soft delete
            guest.IsActive = false;
            guest.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WeddingGuestDto>> SearchWeddingGuestsAsync(string searchTerm)
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive &&
                    (g.Name.Contains(searchTerm) ||
                     (g.Unit != null && g.Unit.Contains(searchTerm))))
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetWeddingGuestsByStatusAsync(string status)
        {
            if (!Enum.TryParse<GuestStatus>(status, true, out var guestStatus))
                return new List<WeddingGuestDto>();

            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive && g.Status == guestStatus)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetWeddingGuestsByUnitAsync(string unit)
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive && g.Unit == unit)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetWeddingGuestsByRelationshipAsync(string relationship)
        {
            if (!Enum.TryParse<RelationshipType>(relationship, true, out var relationshipType))
                return new List<WeddingGuestDto>();

            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive && g.Relationship == relationshipType)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<IEnumerable<WeddingGuestSummaryDto>> GetWeddingGuestsSummaryAsync()
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestSummaryDto>>(guests);
        }

        public async Task<bool> ConfirmGuestAsync(Guid id)
        {
            var guest = await _context.WeddingGuests.FindAsync(id);
            if (guest == null || !guest.IsActive)
                return false;

            guest.Status = GuestStatus.Going;
            guest.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineGuestAsync(Guid id)
        {
            var guest = await _context.WeddingGuests.FindAsync(id);
            if (guest == null || !guest.IsActive)
                return false;

            guest.Status = GuestStatus.NotGoing;
            guest.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetPendingGuestsAsync()
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive && g.Status == GuestStatus.Pending)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetConfirmedGuestsAsync()
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive && g.Status == GuestStatus.Going)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<IEnumerable<WeddingGuestDto>> GetRecentlyAddedGuestsAsync(int days = 7)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive && g.CreatedDate >= cutoffDate)
                .OrderByDescending(g => g.CreatedDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WeddingGuestDto>>(guests);
        }

        public async Task<WeddingGuestStatisticsDto> GetWeddingGuestStatisticsAsync()
        {
            var guests = await _context.WeddingGuests
                .Where(g => g.IsActive)
                .ToListAsync();

            var totalGuests = guests.Count;
            var confirmedGuests = guests.Count(g => g.Status == GuestStatus.Going);
            var pendingGuests = guests.Count(g => g.Status == GuestStatus.Pending);
            var declinedGuests = guests.Count(g => g.Status == GuestStatus.NotGoing);
            var totalPeople = guests.Sum(g => g.NumberOfPeople);
            var totalGiftAmount = guests.Sum(g => g.GiftAmount);
            var averageGiftAmount = totalGuests > 0 ? totalGiftAmount / totalGuests : 0;

            var guestsByRelationship = guests
                .Where(g => g.Relationship.HasValue)
                .GroupBy(g => g.Relationship!.Value.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var guestsByUnit = guests
                .Where(g => !string.IsNullOrEmpty(g.Unit))
                .GroupBy(g => g.Unit!)
                .Select(g => new UnitStatisticsDto
                {
                    Unit = g.Key,
                    Count = g.Count(),
                    TotalGiftAmount = g.Sum(x => x.GiftAmount)
                })
                .OrderByDescending(u => u.Count)
                .ToList();

            return new WeddingGuestStatisticsDto
            {
                TotalGuests = totalGuests,
                ConfirmedGuests = confirmedGuests,
                PendingGuests = pendingGuests,
                DeclinedGuests = declinedGuests,
                TotalPeople = totalPeople,
                TotalGiftAmount = totalGiftAmount,
                AverageGiftAmount = averageGiftAmount,
                GuestsByRelationship = guestsByRelationship,
                GuestsByUnit = guestsByUnit
            };
        }
    }
}
