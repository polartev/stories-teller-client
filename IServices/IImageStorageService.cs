namespace Story_Teller.IServices;

public interface IImageStorageService
{
    Task<List<Models.ImageItem>> LoadImagesAsync(string sid);

    Task AddImageAsync(string sid, ImageSource imageSource);

    Task DeleteImageAsync(string sid, string path);

    Task DeleteImagesAsync(string sid);
}
