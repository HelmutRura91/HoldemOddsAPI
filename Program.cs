using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using HoldemOddsAPI.Models;
using HoldemOddsAPI.Services;
using System.Text.Json;

namespace HoldemOddsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Add services to the Dependency Injection container
            builder.Services.AddControllers();
            builder.Services.AddScoped<DeckService>();
            builder.Services.AddScoped<PokerTableService>();
            builder.Services.AddScoped<GameStateService>();
            builder.Services.AddSingleton<GameState>();
            builder.Services.AddTransient<PokerHandEvaluator>();
            builder.Services.AddSingleton<JsonLogger>(provider => new JsonLogger(@"C:\Users\npotu\source\repos\HoldemOddsAPI\Logs\logfile.json"));

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

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
