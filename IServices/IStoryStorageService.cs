namespace Story_Teller.IServices;

public interface IStoryStorageService
{
    Task<List<Models.Story>> LoadStoriesAsync();

    Task SaveStoriesAsync(IEnumerable<Models.Story> stories);
}
