
using CommandService.Bootstrapping;

namespace CommandService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //configure services
            builder.ConfigureServices();

            var app = builder.Build();

            //configure to build
            app.Configure();

            app.Run();
        }
    }
}
