
using CommandService.AsyncDataService;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SyncDataService.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<CommandDBContextcs>(opt =>
                opt.UseInMemoryDatabase("InMem"));
            builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
            builder.Services.AddScoped<ICommandRepo, CommandRepo>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHostedService<MessageBusSubscriber>();
            builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            PrepDb.PrepPopulation(app);

            app.MapControllers();

            app.Run();
        }
    }
}
