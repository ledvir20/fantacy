
using System.Text.Json;
using Fantasy.Frontend.Repositories;

namespace Fantacy.Frontend.Repositories;

public class Repository(HttpClient httpClient) : IRepository
{
    private readonly HttpClient _httpClient = httpClient;

    private JsonSerializerOptions _jsonSerializerOptions =>
        new() { PropertyNameCaseInsensitive = true };

    public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
    {
        var responseHttp = await _httpClient.GetAsync(url);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<T>(responseHttp);
            return new HttpResponseWrapper<T>(response, false, responseHttp);
        }
        return new HttpResponseWrapper<T>(default, true, responseHttp);
    }

    public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T entity)
    {
        var messageJSON = JsonSerializer.Serialize(entity);
        var messageContent = new StringContent(messageJSON, System.Text.Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PostAsync(url, messageContent);

        return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }


    public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T entity)
    {
        var messageJSON = JsonSerializer.Serialize(entity);
        var messageContent = new StringContent(messageJSON, System.Text.Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PostAsync(url, messageContent);

        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<TActionResponse>(responseHttp);
            return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
        }

        return new HttpResponseWrapper<TActionResponse>(default, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    private async Task<T> UnserializeAnswer<T>(HttpResponseMessage httpResponseMessage)
    {
        var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseString, _jsonSerializerOptions)!;
    }
}
