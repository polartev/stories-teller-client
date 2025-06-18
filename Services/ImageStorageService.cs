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

    public async Task<List<ImageSource>> LoadImagesAsync(string sid)
    {
        if (!File.Exists(imagesJson))
            return new();

        var json = await File.ReadAllTextAsync(imagesJson);
        var dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.StoryImage>>>(json)
                   ?? new();

        if (!dict.TryGetValue(sid, out var images))
            return new();

        return images
            .Select(img => ImageSource.FromFile(img.path))
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

        Dictionary<string, List<Models.StoryImage>> dict;

        if (File.Exists(imagesJson))
        {
            var json = await File.ReadAllTextAsync(imagesJson);
            dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.StoryImage>>>(json) ?? new();
        }
        else
        {
            dict = new();
        }

        if (!dict.ContainsKey(sid))
            dict[sid] = new();

        dict[sid].Add(new Models.StoryImage { path = fullPath });

        var updatedJson = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(imagesJson, updatedJson);
    }

    public async Task DeleteImagesAsync(string sid)
    {
        if (!File.Exists(imagesJson))
            return;

        var json = await File.ReadAllTextAsync(imagesJson);
        var dict = JsonSerializer.Deserialize<Dictionary<string, List<Models.StoryImage>>>(json)
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
