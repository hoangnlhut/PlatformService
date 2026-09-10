using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Repository;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Bootstrapping
{
    public static class ConfigurationStartUp
    {
        public static void ConfigureServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("InMem"));

            builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

            builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

            // Add services to the container.
            builder.Services.AddControllers();

            //add automapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //Register Swagger generator services
            builder.Services.AddSwaggerGen();

            Console.WriteLine($"--> Configuration CommandService Endpoint: {builder.Configuration["CommandService"]}");
        }

        public static void Configure(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.PrepPopulation(); // Seed the database with initial data
            }

            app.UseSwagger();
            app.UseSwaggerUI(); // Generates the classic UI page

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
        }
    }
}

