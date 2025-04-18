using Newtonsoft.Json;

namespace Marshrutka.Hockey
{
    internal class HockeyParser
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.ticketpro.by/bilety-na-sportivnye-meropriyatiya/bilety-na-xokkej/";
        public HockeyParser()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<EventModel>> ParseTickets()
        {
            List<EventModel> events = new List<EventModel>();
            try
            {
                var html = await _httpClient.GetStringAsync(BaseUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Находим все блоки с информацией о матчах
                var ticketNodes = doc.DocumentNode.SelectNodes("//script[contains(@type, 'application/ld+json')]");

                if (ticketNodes != null)
                {
                    foreach (var node in ticketNodes)
                    {
                        var eventData = JsonConvert.DeserializeObject<EventModel>(node.InnerText);
                        if (eventData != null) events.Add(eventData);

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return events;
        }

        public string GetUrl()
        {
            return BaseUrl;
        }
    }

    public class EventModel
    {
        public string Url { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
