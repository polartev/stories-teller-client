namespace Story_Teller.IServices;

public interface IImageStorageService
{
    Task<List<ImageSource>> LoadImagesAsync(string sid);

    Task AddImageAsync(string sid, ImageSource imageSource);

    Task DeleteImagesAsync(string sid);
}
