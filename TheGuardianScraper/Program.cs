using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace TheGuardianScraper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddRequired(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/scraping/start", async ([FromQuery]DateOnly startDate, [FromQuery]DateOnly endDate, HttpContext httpContext, GlobalArticlesProcessor processor) =>
            {
                await processor.StartProcessing(startDate, endDate);
            })
            .WithName("StartScraping")
            .WithOpenApi();

            //app.MapGet("/scraping/day/retry", async ([FromQuery]string date, HttpContext httpContext, GlobalArticlesProcessor processor) =>
            //{
            //    await processor.RetryFailedDays(date);
            //})
            //.WithName("RetryFailedDays")
            //.WithOpenApi();


            //app.MapGet("/scraping/single/retry", async (HttpContext httpContext, GlobalArticlesProcessor processor) =>
            //{
            //    await processor.RetryFailedArticles();
            //})
            //.WithName("RetryFailedArticles")
            //.WithOpenApi();

            //app.MapGet("/content/clear", async (HttpContext httpContext, GlobalArticlesProcessor processor) =>
            //{
            //    await processor.ClearContent();
            //})
            //.WithName("ClearContent")
            //.WithOpenApi();

            app.Run();
        }
    }
}
