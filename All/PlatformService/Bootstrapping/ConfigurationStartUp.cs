using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Repository;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Bootstrapping
{
    public static class ConfigurationStartUp
    {
        public static void ConfigureServices(this IHostApplicationBuilder builder)
        {
            if(builder.Environment.IsProduction())
            {
                var connectionString = builder.Configuration.GetConnectionString("PlatformsConn");
                Console.WriteLine($"--> Using SqlServer Db with connection string: {connectionString}");

                // Register DbContext with SQL Server
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                Console.WriteLine("--> Using InMem Db");
                builder.Services.AddDbContext<AppDbContext>(opt =>
                    opt.UseInMemoryDatabase("InMem"));
            }

            builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

            builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();


            builder.Services.AddSingleton<RabbitMqConnectionProvider>(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var client = new RabbitMqConnectionProvider(config);
                client.GetConnectionAsync().GetAwaiter().GetResult();
                Console.WriteLine("--> RabbitMqConnectionProvider created successfully.");
                return client;
            });

            builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

           
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
            }

            app.UseSwagger();
            app.UseSwaggerUI(); // Generates the classic UI page

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.PrepPopulation(); // Seed the database with initial data
        }
    }
}

