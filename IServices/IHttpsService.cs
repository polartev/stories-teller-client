namespace Story_Teller.IServices;

public interface IHttpsService
{
    /// <summary>
    /// HTTP client used for making requests.
    /// </summary>
    HttpClient HttpClient { get; set; }

    /// <summary>
    /// Sends a POST request to the specified URL with the given data.
    /// </summary>
    /// <param name="url">The URL to send the POST request to.</param>
    /// <param name="data">The data to be sent in the POST request.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
    Task<string> PostAsync(string url, object data);

    /// <summary>
    /// Sends a GET request to the specified URL with the given data.
    /// </summary>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
    Task<string> GetAsync(string url);
}