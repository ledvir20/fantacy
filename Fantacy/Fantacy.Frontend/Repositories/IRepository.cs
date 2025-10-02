using Fantasy.Frontend.Repositories;

namespace Fantacy.Frontend.Repositories;

public interface IRepository
{
    Task<HttpResponseWrapper<T>> GetAsync<T>(string url);
    Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T entity);
    Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T entity);
}
