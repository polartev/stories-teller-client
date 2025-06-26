using System.Linq;
using System.Text.Json;

namespace Story_Teller.Services;

public class ImageStorageService : IServices.IImageStorageService
{
    private readonly string imagesJson = StoragePaths.ImagesFilePath;
    private readonly string imagesDirectory = StoragePaths.ImagesDirectory;

    public ImageStorageService()
    {
        if (!File.Exists(imagesDirectory))
        {
            Directory.CreateDirectory(imagesDirectory);
        }
    }

    public async Task<List<Models.ImageItem>> LoadImagesAsync(string sid)
    {
        if (!File.Exists(imagesJson))
            return new();

        var json = await File.ReadAllTextAsync(imagesJson);
        var dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.ImagePath>>>(json)
                   ?? new();

        if (!dict.TryGetValue(sid, out var imagePaths))
            return new();

        return imagePaths
            .Select(p => new Models.ImageItem
            {
                Path = p.path,
                Source = ImageSource.FromFile(p.path)
            })
            .ToList();
    }

    public async Task AddImageAsync(string sid, ImageSource imageSource)
    {
        var fileName = $"img_{Guid.NewGuid()}.png";
        var fullPath = Path.Combine(imagesDirectory, fileName);

        if (imageSource is FileImageSource fileSource)
        {
            var sourcePath = fileSource.File;
            File.Copy(sourcePath, fullPath);
        }
        else if (imageSource is StreamImageSource streamSource)
        {
            var stream = await streamSource.Stream(CancellationToken.None);
            using var fileStream = File.Create(fullPath);
            await stream.CopyToAsync(fileStream);
        }
        else
        {
            throw new NotSupportedException("Unsupported ImageSource type");
        }

        Dictionary<string, List<Models.ImagePath>> dict;

        if (File.Exists(imagesJson))
        {
            var json = await File.ReadAllTextAsync(imagesJson);
            dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.ImagePath>>>(json) ?? new();
        }
        else
        {
            dict = new();
        }

        if (!dict.ContainsKey(sid))
            dict[sid] = new();

        dict[sid].Add(new Models.ImagePath { path = fullPath });

        var updatedJson = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(imagesJson, updatedJson);
    }

    public async Task DeleteImageAsync(string sid, string path)
    {
        if (!File.Exists(imagesJson))
            return;

        var json = await File.ReadAllTextAsync(imagesJson);
        var dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.ImagePath>>>(json)
                   ?? new();

        if (!dict.TryGetValue(sid, out var imageList))
            return;

        var itemToRemove = imageList.FirstOrDefault(img => img.path == path);
        if (itemToRemove != null)
        {
            imageList.Remove(itemToRemove);

            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }

            if (imageList.Count == 0)
                dict.Remove(sid);

            var updatedJson = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(imagesJson, updatedJson);
        }
    }

    public async Task DeleteImagesAsync(string sid)
    {
        if (!File.Exists(imagesJson))
            return;

        var json = await File.ReadAllTextAsync(imagesJson);
        var dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.ImagePath>>>(json)
                   ?? new();

        if (!dict.TryGetValue(sid, out var images))
            return;

        foreach (var img in images)
        {
            try
            {
                if (File.Exists(img.path))
                    File.Delete(img.path);
            }
            catch { }
        }

        dict.Remove(sid);

        var updatedJson = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(imagesJson, updatedJson);
    }
}
