using HtmlAgilityPack;
using System.Globalization;
namespace Marshrutka;

public class BaranovichiTicketParser
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://tickets.baranovichi-express.by/tickets/search";

    public BaranovichiTicketParser()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<BusTicket>> ParseTickets(DateTime date)
    {
        var tickets = new List<BusTicket>();
        bool needStop = false;
        int pageNum = 1;

        try
        {
            while (!needStop)
            {
                // Формируем URL с параметрами
                var url = $"{BaseUrl}?pickup=2&destination=1&seats_limit=1&date_of_journey={date:dd.MM.yyyy}&page={pageNum}";

                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Находим все блоки с информацией о рейсах
                var ticketNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'tickets-item__body')]");

                if (ticketNodes == null)
                {
                    needStop = true;
                    continue;
                }

                if (ticketNodes != null)
                {
                    foreach (var node in ticketNodes)
                    {
                        var ticket = new BusTicket
                        {
                            DepartureTime = DateTime.ParseExact(node.SelectSingleNode(".//div[contains(@class, 'tickets-way__point-time')]")?.InnerText.Trim()!, "H:mm", CultureInfo.InvariantCulture),
                            DeparturePoint = node.SelectSingleNode(".//span[contains(@style, 'vertical-align: middle;')]")?.InnerText.Trim()!
                        };

                        // Парсинг количества свободных мест
                        var seatsText = node.SelectSingleNode(".//div[contains(text(), 'Свободно мест:')]")?.InnerText;
                        if (seatsText != null)
                        {
                            ticket.AvailableSeats = int.Parse(seatsText.Replace("Свободно мест:", "").Trim());
                        }

                        tickets.Add(ticket);
                    }
                }

                pageNum++;
            }
        }
        catch (Exception ex)
        {
            //
        }

        return tickets;
    }

    public class BusTicket
    {

        public DateTime DepartureTime { get; set; }
        public string? DeparturePoint { get; set; }
        public int AvailableSeats { get; set; }
    }
}
