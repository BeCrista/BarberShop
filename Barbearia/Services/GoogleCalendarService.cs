using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace Barbearia.Services
{
    public class GoogleCalendarService
    {
        private readonly string ServiceAccountCredentialsPath = "Google/credentials.json"; // caminho do JSON da conta de serviço
        private readonly string ApplicationName = "BarbeariaApp"; // o nome tem que ser igual ao nome do projeto que criou no Google Cloud

        private CalendarService GetCalendarService()
        {
            GoogleCredential credential;
            using (var stream = new FileStream(ServiceAccountCredentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(CalendarService.Scope.Calendar);
            }

            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public async Task<string> CreateEventAsync(DateTime start, DateTime end, string title, string description, string calendarId)
        {
            var service = GetCalendarService();

            var newEvent = new Event()
            {
                Summary = title,
                Description = description,
                Start = new EventDateTime() { DateTime = start, TimeZone = "Europe/Lisbon" },
                End = new EventDateTime() { DateTime = end, TimeZone = "Europe/Lisbon" },
            };

            var request = service.Events.Insert(newEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();

            return createdEvent.Id;
        }
    }
}
