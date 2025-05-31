using Microsoft.AspNetCore.Mvc;
using PosStore.DTOs;
using PosStore.Services.Interfaces;

namespace PosStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeddingGuestsController : ControllerBase
    {
        private readonly IWeddingGuestService _weddingGuestService;

        public WeddingGuestsController(IWeddingGuestService weddingGuestService)
        {
            _weddingGuestService = weddingGuestService;
        }

        /// <summary>
        /// Get all wedding guests with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<WeddingGuestDto>>> GetWeddingGuests([FromQuery] WeddingGuestQueryDto queryDto)
        {
            var result = await _weddingGuestService.GetWeddingGuestsAsync(queryDto);
            return Ok(result);
        }

        /// <summary>
        /// Get all wedding guests without pagination
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetAllWeddingGuests()
        {
            var guests = await _weddingGuestService.GetAllWeddingGuestsAsync();
            return Ok(guests);
        }

        /// <summary>
        /// Get wedding guest by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<WeddingGuestDto>> GetWeddingGuest(long id)
        {
            var guest = await _weddingGuestService.GetWeddingGuestByIdAsync(id);

            if (guest == null)
            {
                return NotFound($"Wedding guest with ID {id} not found.");
            }

            return Ok(guest);
        }

        /// <summary>
        /// Create a new wedding guest
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WeddingGuestDto>> CreateWeddingGuest(CreateWeddingGuestDto dto)
        {
            var guest = await _weddingGuestService.CreateWeddingGuestAsync(dto);
            return CreatedAtAction(nameof(GetWeddingGuest), new { id = guest.Id }, guest);
        }

        /// <summary>
        /// Update an existing wedding guest
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<WeddingGuestDto>> UpdateWeddingGuest(long id, UpdateWeddingGuestDto dto)
        {
            var guest = await _weddingGuestService.UpdateWeddingGuestAsync(id, dto);

            if (guest == null)
            {
                return NotFound($"Wedding guest with ID {id} not found.");
            }

            return Ok(guest);
        }

        /// <summary>
        /// Delete a wedding guest (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWeddingGuest(long id)
        {
            var result = await _weddingGuestService.DeleteWeddingGuestAsync(id);

            if (!result)
            {
                return NotFound($"Wedding guest with ID {id} not found.");
            }

            return NoContent();
        }

        /// <summary>
        /// Search wedding guests by term
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> SearchWeddingGuests([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required.");
            }

            var guests = await _weddingGuestService.SearchWeddingGuestsAsync(searchTerm);
            return Ok(guests);
        }

        /// <summary>
        /// Get wedding guests by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetWeddingGuestsByStatus(string status)
        {
            var guests = await _weddingGuestService.GetWeddingGuestsByStatusAsync(status);
            return Ok(guests);
        }

        /// <summary>
        /// Get wedding guests by unit
        /// </summary>
        [HttpGet("unit/{unit}")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetWeddingGuestsByUnit(string unit)
        {
            var guests = await _weddingGuestService.GetWeddingGuestsByUnitAsync(unit);
            return Ok(guests);
        }

        /// <summary>
        /// Get wedding guests by relationship
        /// </summary>
        [HttpGet("relationship/{relationship}")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetWeddingGuestsByRelationship(string relationship)
        {
            var guests = await _weddingGuestService.GetWeddingGuestsByRelationshipAsync(relationship);
            return Ok(guests);
        }

        /// <summary>
        /// Get wedding guests summary
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<WeddingGuestSummaryDto>>> GetWeddingGuestsSummary()
        {
            var guests = await _weddingGuestService.GetWeddingGuestsSummaryAsync();
            return Ok(guests);
        }

        /// <summary>
        /// Get wedding guest statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<WeddingGuestStatisticsDto>> GetWeddingGuestStatistics()
        {
            var statistics = await _weddingGuestService.GetWeddingGuestStatisticsAsync();
            return Ok(statistics);
        }

        /// <summary>
        /// Get pending wedding guests
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetPendingGuests()
        {
            var guests = await _weddingGuestService.GetPendingGuestsAsync();
            return Ok(guests);
        }

        /// <summary>
        /// Get confirmed wedding guests
        /// </summary>
        [HttpGet("confirmed")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetConfirmedGuests()
        {
            var guests = await _weddingGuestService.GetConfirmedGuestsAsync();
            return Ok(guests);
        }

        /// <summary>
        /// Get recently added wedding guests
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<WeddingGuestDto>>> GetRecentlyAddedGuests([FromQuery] int days = 7)
        {
            var guests = await _weddingGuestService.GetRecentlyAddedGuestsAsync(days);
            return Ok(guests);
        }

        /// <summary>
        /// Confirm a wedding guest
        /// </summary>
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmGuest(long id)
        {
            var result = await _weddingGuestService.ConfirmGuestAsync(id);

            if (!result)
            {
                return NotFound($"Wedding guest with ID {id} not found.");
            }

            return Ok(new { message = "Guest confirmed successfully." });
        }

        /// <summary>
        /// Decline a wedding guest
        /// </summary>
        [HttpPost("{id}/decline")]
        public async Task<ActionResult> DeclineGuest(long id)
        {
            var result = await _weddingGuestService.DeclineGuestAsync(id);

            if (!result)
            {
                return NotFound($"Wedding guest with ID {id} not found.");
            }

            return Ok(new { message = "Guest declined successfully." });
        }
    }
}
