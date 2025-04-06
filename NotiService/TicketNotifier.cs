using Telegram.Bot;

namespace Marshrutka
{
    public class TicketNotifier
    {
        private readonly TelegramBotClient _botClient;

        public TicketNotifier(string token)
        {
            _botClient = new TelegramBotClient(token);
        }

        public async Task SendNotification(string message, string chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, message);
        }

        public async Task MonitorUpdates(TimeSpan interval)
        {
            while (true)
            {
                var updates = await _botClient.GetUpdatesAsync();
                foreach (var update in updates)
                {
                    Console.WriteLine(update.Message.Chat.Id);
                }

                await Task.Delay(interval);
            }
        }
    }
}
