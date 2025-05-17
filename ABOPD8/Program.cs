using ABOPD8.Repositories;
using ABOPD8.Services;

namespace ABOPD8;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        builder.Services.AddScoped<ITripsServices, TripsServices>();
        builder.Services.AddScoped<IClientsServices, ClientsServices>();
        builder.Services.AddScoped<ITripsRepositories, TripsRepositories>();
        builder.Services.AddScoped<IClientsRepositories, ClientsRepositories>();
        

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();
        
        app.Run();
    }
}