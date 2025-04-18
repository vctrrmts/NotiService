namespace Marshrutka.Hockey;

internal class HockeyMonitor
{
    private readonly HockeyParser _parser;
    private readonly TimeSpan _checkInterval;
    private readonly List<string> _chatIds;
    private List<EventModel> _events;

    public HockeyMonitor(TimeSpan checkInterval, List<string> chatIds)
    {
        _parser = new HockeyParser();
        _checkInterval = checkInterval;
        _chatIds = chatIds;
        _events = new List<EventModel>();
    }

    public async Task StartMonitoring(TicketNotifier notifier)
    {
        while (true)
        {
            var events = await _parser.ParseTickets();

            foreach (var eventt in events)
            {
                if (!_events.Any(x => x.Name == eventt.Name))
                {
                    foreach (var id in _chatIds)
                    {
                        await notifier.SendNotification($"Появилось событие {eventt.Name}. \n{eventt.Url}", id);
                        Console.WriteLine(DateTime.Now + " Уведомление отправлено.");
                    }

                    _events.Add(eventt);
                }
            }
            Console.WriteLine(DateTime.Now + " Проведена проверка событий.");

            await Task.Delay(_checkInterval);
        }
    }
}
