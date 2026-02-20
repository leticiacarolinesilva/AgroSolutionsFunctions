using AgroSolutionsFunctions.Contracts;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AgroSolutionsFunctions.EntryPoints;

public class IngestionEntryPoint
{
    private readonly ILogger<IngestionEntryPoint> _logger;
    private readonly HttpClient _httpClient;

    public IngestionEntryPoint(
        ILogger<IngestionEntryPoint> logger,
        IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(httpClientFactory, nameof(httpClientFactory));

        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    [Function("Ingestion-Queue")]
    public async Task Run(
        [ServiceBusTrigger("ingestion-queue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        string apiAgroUrlrl = Environment.GetEnvironmentVariable("ApiAgroUrl");

        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        var messageBody = message.Body.ToString();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GenereteToken());

        var response = await _httpClient.PostAsync(
            $"{apiAgroUrlrl}/api/ingestion/single",
            new StringContent(messageBody, System.Text.Encoding.UTF8, "application/json"));

        _logger.LogInformation("Response Status Code: {statusCode}", response.StatusCode);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    [Function("Ingestion-Timer-Run")]
    public async Task TimerRun(
        [TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo)
    {
        string apiAgroUrlrl = Environment.GetEnvironmentVariable("ApiAgroUrl");

        _logger.LogInformation("Timer trigger executed at: {time}", DateTime.Now);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GenereteToken());

        var content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{apiAgroUrlrl}/api/alerts", content);

        _logger.LogInformation("Response Status Code: {statusCode}", response.StatusCode);
    }

    [Function("Ingestion-Timer-Disable")]
    public async Task TimerDisable(
        [TimerTrigger("0 0 */1 * * *")] TimerInfo timerInfo)
    {
        string apiAgroUrlrl = Environment.GetEnvironmentVariable("ApiAgroUrl");

        _logger.LogInformation("Timer trigger executed at: {time}", DateTime.Now);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GenereteToken());

        var content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{apiAgroUrlrl}/api/alerts/update", content);

        _logger.LogInformation("Response Status Code: {statusCode}", response.StatusCode);
    }

    private async Task<string> GenereteToken()
    {
        string login = Environment.GetEnvironmentVariable("Login");
        string password = Environment.GetEnvironmentVariable("Password");
        string apiAgroUrlrl = Environment.GetEnvironmentVariable("ApiAgroUrl");

        var credentials = new
        {
            email = login,
            password
        };

        var credentialsJson = JsonSerializer.Serialize(credentials);

        var content = new StringContent(credentialsJson, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{apiAgroUrlrl}/api/auth/login", content);

        var responseTokenStr = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<TokenResponse>(responseTokenStr);

        return token.Token;
    }
}