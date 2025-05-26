namespace Story_Teller.IServices;

public interface IStoryStorageService
{
    Task<List<Models.Story>> LoadAsync();

    Task SaveAsync(IEnumerable<Models.Story> stories);

    Task DeleteAsync(Models.Story story);
}
