using googleCalendar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using googleCalendar.Services;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace googleCalendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IGoogleCalendarService _calendarService;
        public EventsController(IGoogleCalendarService calendarService)
        {
            _calendarService = calendarService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GoogleEvent googleEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (googleEvent.StartDateTime.DayOfWeek == DayOfWeek.Friday || googleEvent.StartDateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                return BadRequest("Events cannot be created on Fridays or Saturdays.");
            }

            if (googleEvent.StartDateTime < DateTime.Today)
            {
                return BadRequest("Cannot create events in the past.");
            }
            try
            {
                Event createdEvent = await _calendarService.CreateEvent(googleEvent);
                return Created("", createdEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to create event: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetEvents(DateTime? startDate = null, DateTime? endDate = null, string searchQuery = null)
        {
            try
            {
                var events = await _calendarService.GetEvents(startDate, endDate, searchQuery);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving events: {ex.Message}");
            }
        }
        [HttpDelete("{eventId}")]
        public async Task<ActionResult> DeleteEvent(string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                {
                    return BadRequest("Event ID is required.");
                }

                bool isEventDeleted = await _calendarService.DeleteEvent(eventId);

                if (isEventDeleted)
                {
                    return NoContent(); // 204 No Content
                }
                else
                {
                    return NotFound("Event not found or could not be deleted.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting event: {ex.Message}");
            }
        }
    }
}
