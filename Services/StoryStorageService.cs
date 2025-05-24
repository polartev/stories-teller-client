using Story_Teller.Models;
using System.Text.Json;

namespace Story_Teller.Services;

public static class StoragePaths
{
    public static string StoriesFilePath =>
        Path.Combine(FileSystem.AppDataDirectory, "stories.json");
}

public class StoryStorageService : IServices.IStoryStorageService
{
    private readonly string filePath = StoragePaths.StoriesFilePath;

    public async Task<List<Story>> LoadStoriesAsync()
    {
        if (!File.Exists(filePath))
        {
            var defaultStories = Enumerable.Range(1, 2)
                .Select(i => new Story($"Story {i}", ""))
                .ToList();
            await SaveStoriesAsync(defaultStories);
            return defaultStories;
        }

        using FileStream fs = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<List<Story>>(fs) ?? new();
    }

    public async Task SaveStoriesAsync(IEnumerable<Story> stories)
    {
        using FileStream fs = File.Create(filePath);
        await JsonSerializer.SerializeAsync(fs, stories, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}