
using Microsoft.EntityFrameworkCore;
using PlatformService2.AsynDataService;
using PlatformService2.Data;
using PlatformService2.Models;
using PlatformService2.SyncDataService.Grpc;
using PlatformService2.SyncDataService.Http;

namespace PlatformService2
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
            if (builder.Environment.IsProduction())
            {
                Console.WriteLine("--> Using SqlServer database");
                builder.Services.AddDbContext<PlatformDbContext>(
                    opt => opt.UseSqlServer(
                        builder.Configuration.GetConnectionString("PlatformConn")));
            }
            else
            {
                Console.WriteLine("--> Using local sql database");
                builder.Services.AddDbContext<PlatformDbContext>(opt => opt.UseSqlServer(
                    builder.Configuration.GetConnectionString("PlatformConnLocal")));

            }

            builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            builder.Services.AddSingleton<IMessageBus, MessageBus>();
            builder.Services.AddGrpc();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                //add seed data to the in-memory database
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<PlatformDbContext>();
                if (builder.Environment.IsProduction())
                {
                    //Try migration
                    try
                    {
                        Console.WriteLine("Migrating database...");
                        
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not run migrations: " + ex.Message);
                    }
                }
                SeedData(context);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            Console.WriteLine("--> CommandService endpoint : " + app.Configuration["CommandServiceConnectionString"]);
            app.UseHttpsRedirection();

            app.UseAuthorization();

            //Configure the gRPC service
            app.MapGrpcService<GrpcPlatformServicecs>();
            app.MapGet("/Proto/platform.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Proto/platform.proto"));
            });
            app.MapControllers();

            app.Run();
        }
        private static void SeedData(PlatformDbContext context)
        {
            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data...");
                context.Platforms.AddRange(
                    new Platform { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );
                context.SaveChanges();
            }
        }
    }
}
