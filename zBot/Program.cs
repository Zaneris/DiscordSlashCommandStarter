using Microsoft.Extensions.Hosting;
using zBot.Extensions;

namespace zBot
{
    public class Program
    {
        public static void Main()
        {
            CreateHostBuilder().Build().Run();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .CreateBotHostDefaults(botBuilder =>
                {
                    botBuilder.UseStartup<Startup>();
                });
    }
}
