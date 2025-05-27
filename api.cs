// ISteamIntegrationService.cs
public interface ISteamIntegrationService
{
    Task<IEnumerable<Skin>> GetUserInventoryFromSteamAsync(int userId);
    Task<bool> TransferSkinAsync(string steamItemId, int toUserId);
}

// SteamIntegrationService.cs
public class SteamIntegrationService : ISteamIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SteamIntegrationService> _logger;
    private readonly string _apiKey;

    public SteamIntegrationService(
        IHttpClientFactory httpClientFactory,
        ILogger<SteamIntegrationService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiKey = configuration["Steam:ApiKey"];
    }

    public async Task<IEnumerable<Skin>> GetUserInventoryFromSteamAsync(int userId)
    {
        var httpClient = _httpClientFactory.CreateClient("SteamAPI");

        try
        {
            // Здесь должен быть реальный запрос к Steam API
            // Это упрощенная имитация
            var response = await httpClient.GetAsync($"some/steam/api/endpoint?key={_apiKey}&user={userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Steam API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            // Парсинг ответа и преобразование в список скинов
            return new List<Skin>(); // Заглушка
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Steam inventory for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> TransferSkinAsync(string steamItemId, int toUserId)
    {
        var httpClient = _httpClientFactory.CreateClient("SteamAPI");

        try
        {
            // Имитация запроса на передачу предмета
            var response = await httpClient.PostAsync(
                "some/steam/api/transfer",
                new StringContent($"item={steamItemId}&to={toUserId}&key={_apiKey}"));

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring Steam item: {ItemId} to user: {UserId}", steamItemId, toUserId);
            throw;
        }
    }
}