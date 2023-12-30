using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using HoldemOddsAPI.Models;
using HoldemOddsAPI.Services;

namespace HoldemOddsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddScoped<DeckService>();

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
