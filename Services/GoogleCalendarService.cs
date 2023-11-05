using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using googleCalendar.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace googleCalendar.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly CalendarService _calendarService;
        public GoogleCalendarService()
        {
            _calendarService = InitializeCalendarService().Result;
        }

        private async Task<CalendarService> InitializeCalendarService()
        {
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Calendar";

            string credPath = "token.json";
            UserCredential credential;

            using FileStream stream = new("credentials.json", FileMode.Open, FileAccess.Read);
            GoogleClientSecrets secrets = GoogleClientSecrets.FromStream(stream);
            
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets.Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)
            );
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            return service;
        }

        public async Task<Event> CreateEvent(GoogleEvent googleEvent)
        {
            Event newEvent = new()
            {
                Summary = googleEvent.Summary,
                Description = googleEvent.Description,
                Start = new EventDateTime { DateTimeDateTimeOffset = googleEvent.StartDateTime, TimeZone = "UTC" },
                End = new EventDateTime { DateTimeDateTimeOffset = googleEvent.EndDateTime, TimeZone = "UTC" }
            };

            var createdEvent = await _calendarService.Events.Insert(newEvent, "primary").ExecuteAsync();

            createdEvent.End.DateTimeDateTimeOffset = googleEvent.EndDateTime;
            createdEvent.Start.DateTimeDateTimeOffset = googleEvent.StartDateTime;
            return createdEvent;
        }

        public  async Task<List<Event>> GetEvents(DateTime? startDate = null, DateTime? endDate = null, string searchQuery =null)
        {
            EventsResource.ListRequest request = _calendarService.Events.List("primary");
            request.TimeMin = startDate ?? DateTime.Now;
            request.TimeMax = endDate ?? DateTime.Now.AddMonths(1); 
            request.Q = searchQuery;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10; 
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            return await GetEventsPagedAsync(request);
        }
        private async Task<List<Event>> GetEventsPagedAsync(EventsResource.ListRequest request)
        {
            var allEvents = new List<Event>();

            while (request != null)
            {
                Events events = await request.ExecuteAsync();
                foreach(var x in events.Items)
                {
                    x.Start.DateTimeDateTimeOffset = x.Start.DateTime;
                    x.End.DateTimeDateTimeOffset = x.End.DateTime;
                }
                if (events.Items != null)
                {
                    allEvents.AddRange(events.Items);
                }

                request.PageToken = events.NextPageToken;

                if (String.IsNullOrEmpty(request.PageToken))
                {
                    request = null; // No more pages, exit the loop
                }
                
            }
            return allEvents;
        }
        public async Task<bool> DeleteEvent(string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                {
                    return false; // eventId is required
                }

                EventsResource.DeleteRequest deleteRequest = _calendarService.Events.Delete("primary", eventId);
                await deleteRequest.ExecuteAsync();

                // If the delete request didn't throw an exception, consider the event deleted.
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as required
                Console.WriteLine($"Error deleting event: {ex.Message}");
                return false;
            }
        }
    }
}
