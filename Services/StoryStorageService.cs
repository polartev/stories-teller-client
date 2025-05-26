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

    public async Task<List<Story>> LoadAsync()
    {
        if (!File.Exists(filePath))
        {
            var defaultStories = Enumerable.Range(1, 2)
                .Select(i => new Story(Guid.NewGuid().ToString(), $"Story {i}", ""))
                .ToList();
            await SaveAsync(defaultStories);
            return defaultStories;
        }

        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Story>>(json) ?? new List<Story>();
    }

    public async Task SaveAsync(IEnumerable<Story> stories)
    {
        string json = JsonSerializer.Serialize(stories, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task DeleteAsync(Story story)
    {
        var stories = await LoadAsync();
        stories.RemoveAll(s => s.Sid == story.Sid);
        await SaveAsync(stories);
    }
}