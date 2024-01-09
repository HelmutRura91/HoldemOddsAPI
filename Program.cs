using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using HoldemOddsAPI.Models;
using HoldemOddsAPI.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;


namespace HoldemOddsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Add services to the Dependency Injection container
            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.AllowTrailingCommas = true;
            });
            builder.Services.AddScoped<DeckService>();
            builder.Services.AddScoped<PokerTableService>();
            builder.Services.AddScoped<GameStateService>();
            builder.Services.AddSingleton<GameState>();
            builder.Services.AddTransient<PokerHandEvaluator>();
            builder.Services.AddSingleton<JsonLogger>(provider => new JsonLogger(@"C:\Users\npotu\source\repos\HoldemOddsAPI\Logs\logfile.json"));

            builder.Services.AddHttpClient();
            

            // Build the intermediate service provider to access configured services
            //var intermediateProvider = builder.Services.BuildServiceProvider();
            //// Retrieve the configured JsonSerializerOptions
            //var jsonSerializerOptions = intermediateProvider.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

            //// Register the JsonSerializerOptions instance
            //builder.Services.AddSingleton(jsonSerializerOptions);
            //{
            //    PropertyNameCaseInsensitive = true,
            //});

            var app = builder.Build();

            //Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

    }
} 
