using Marshrutka.Hockey;
using Telegram.Bot.Types;

namespace Marshrutka
{
    internal static class Program
    {
        const string telegramBotToken = "7616635650:AAFY0nfaX0VAYnMn7w4YFOG9H4F4-aKslgA";
        const string telegramChatIdMe = "774957441";
        const string telegramChatIdIgor = "419157003";
        static List<string> telegramChatIds = new() { "774957441", "419157003" };

        [STAThread]
        public static async Task Main()
        {
            Console.WriteLine("Мониторинг билетов на хоккей.");

            var notifier = new TicketNotifier(telegramBotToken);

            var monitor = new HockeyMonitor(TimeSpan.FromSeconds(20), "Минск", telegramChatIds);
            await monitor.StartMonitoring(notifier);
        }

    }


}