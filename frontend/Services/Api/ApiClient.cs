using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace insureflow_portal.Services.Api;

public sealed class ApiClient(HttpClient httpClient) : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            endpoint,
            cancellationToken);

        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            endpoint,
            request,
            JsonOptions,
            cancellationToken);

        return await ReadResponseAsync<TResponse>(
            response,
            cancellationToken);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            endpoint,
            request,
            JsonOptions,
            cancellationToken);

        return await ReadResponseAsync<TResponse>(
            response,
            cancellationToken);
    }

    public async Task DeleteAsync(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(
            endpoint,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task<T?> ReadResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent ||
            response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        var content = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync(
            cancellationToken);

        throw new ApiException(
            $"API request failed with status code {(int)response.StatusCode}.",
            response.StatusCode,
            content);
    }
}