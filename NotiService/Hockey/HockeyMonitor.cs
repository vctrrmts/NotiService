namespace Marshrutka.Hockey;

internal class HockeyMonitor
{
    private readonly HockeyParser _parser;
    private readonly TimeSpan _checkInterval;
    private readonly string _teamName;
    private DateTime _lastMessageSended;
    private readonly List<string> _chatIds;


    public HockeyMonitor(TimeSpan checkInterval, string teamName, List<string> chatIds)
    {
        _parser = new HockeyParser();
        _teamName = teamName;
        _checkInterval = checkInterval;
        _chatIds = chatIds;
    }
    public async Task StartMonitoring(TicketNotifier notifier)
    {
        while (true)
        {
            var events = await _parser.ParseTickets(_teamName);

            var lookingEvent = events.Where(x=>x.Name.Contains(_teamName)).FirstOrDefault();

            if (lookingEvent != null) 
            {
                if ((DateTime.Now - _lastMessageSended).TotalMinutes > 30 )
                {
                    foreach (var id in _chatIds)
                    {
                        await notifier.SendNotification($"Появился билет на {lookingEvent.Name}. \n{lookingEvent.Url}", id);
                        Console.WriteLine(DateTime.Now + " Уведомление отправлено.");
                    }
                    _lastMessageSended = DateTime.Now;

                }
            }
            else
            {
                Console.WriteLine(DateTime.Now + " Событие не найдено.");
            }

            await Task.Delay(_checkInterval);
        }
    }
}
