using PlatformService2.DTOs;
using System.Text;
using System.Text.Json;

namespace PlatformService2.SyncDataService.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task SendPlatformToCommand(PlatformReadDTO platform)
        {
            var httpContent = new StringContent(

                JsonSerializer.Serialize(platform),

                Encoding.UTF8,
                "application/json");
            Console.WriteLine($"Content send to command: {httpContent.ToString()}");
            var response = await _httpClient.PostAsync($"{_configuration["CommandServiceConnectionString"]}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
            }
        }
    }
}
