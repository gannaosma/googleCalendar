using Google.Apis.Calendar.v3.Data;
using googleCalendar.Models;

namespace googleCalendar.Services
{
    public interface IGoogleCalendarService
    {
        Task<Event> CreateEvent(GoogleEvent googleEvent);
        Task<List<Event>> GetEvents(DateTime? startDate = null, DateTime? endDate = null, string searchQuery = null);
        Task<bool> DeleteEvent(string eventId);
    }
}
