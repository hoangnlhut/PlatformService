using Microsoft.EntityFrameworkCore;

namespace CommandService.Bootstrapping
{
    public static class ConfigurationStartUp
    {
        public static void ConfigureServices(this IHostApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();

            //add automapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //Register Swagger generator services
            builder.Services.AddSwaggerGen();
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
        }
    }
}
