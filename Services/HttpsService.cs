namespace Story_Teller.Services;

public class HttpsService : IServices.IHttpsService
{
    public HttpClient HttpClient { get; set; }

    public HttpsService()
    {
        HttpClient = new HttpClient();
    }

    public async Task<string> PostAsync(string url, object data)
    {
        var response = await HttpClient.PostAsync(url, (HttpContent?)data);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAsync(string url)
    {
        var response = await HttpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}