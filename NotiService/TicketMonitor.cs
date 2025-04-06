using System.Globalization;

namespace Marshrutka;

internal class TicketMonitor
{
    private readonly BaranovichiTicketParser _parser;
    private readonly TimeSpan _checkInterval;
    private readonly string _chatId;
    private const string departurePoint = "ИНСТИТУТ КУЛЬТУРЫ";


    public TicketMonitor(TimeSpan checkInterval, string chatId)
    {
        _parser = new BaranovichiTicketParser();
        _checkInterval = checkInterval;
        _chatId = chatId;
    }
    public async Task StartMonitoring(TicketNotifier notifier, DateTime date, 
        string departureTimeStartString, string departureTimeEndString)
    {
        DateTime departureTimeStart = DateTime.ParseExact(departureTimeStartString, "H:mm", CultureInfo.InvariantCulture);
        DateTime departureTimeEnd = DateTime.ParseExact(departureTimeEndString, "H:mm", CultureInfo.InvariantCulture);

        while (true)
        {
            var tickets = await _parser.ParseTickets(date);

            foreach (var ticket in tickets)
            {
                if (ticket.DeparturePoint == departurePoint &&
                    ticket.DepartureTime.TimeOfDay >= departureTimeStart.TimeOfDay &&
                    ticket.DepartureTime.TimeOfDay <= departureTimeEnd.TimeOfDay &&
                    ticket.AvailableSeats > 0)
                {
                    await notifier.SendNotification($"Свободное место в {ticket.DepartureTime:H:mm}", _chatId);
                }
            }

            await Task.Delay(_checkInterval);
        }
    }
}
