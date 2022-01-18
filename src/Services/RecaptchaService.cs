using System.Text.Json.Serialization;

namespace GigLocal.Services;

record RecaptchaResponse
(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("challenge_ts")] DateTime ChallengeTimestamp,
    [property: JsonPropertyName("hostname")] string Hostname,
    [property: JsonPropertyName("error-codes")] IEnumerable<string> ErrorCodes
);

public interface IRecaptchaService
{
    string RecaptchaSiteKey { get; }

    Task<bool> ValidateAsync(string response);
}

public class RecaptchaService : IRecaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly RecaptchaOptions _options;

    public string RecaptchaSiteKey { get => _options.SiteKey; }

    public RecaptchaService(HttpClient httpClient, IOptions<RecaptchaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<bool> ValidateAsync(string response)
    {
        var formData = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new ("secret", _options.SecretKey),
            new ("response", response)
        });

        var result = await _httpClient.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            formData
        );

        var recaptchaResponse = await result.Content.ReadFromJsonAsync<RecaptchaResponse>();
        return recaptchaResponse.Success;
    }
}
