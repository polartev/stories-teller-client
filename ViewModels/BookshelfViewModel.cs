using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Story_Teller.ViewModels;

public partial class BookshelfViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<StoryViewModel> stories = new();

    private IServices.IStoryStorageService storyStorageService;

    public BookshelfViewModel(IServices.IStoryStorageService storyStorageService)
    {
        this.storyStorageService = storyStorageService;

        LoadAsync();
    }

    private async void LoadAsync()
    {
        var loaded = await storyStorageService.LoadStoriesAsync();
        Stories.Clear();
        foreach (var story in loaded)
        {
            Stories.Add(new StoryViewModel(story, SaveAllAsync));
        }
    }

    private async void SaveAllAsync()
    {
        var models = Stories.Select(story => story.ToModel());
        await storyStorageService.SaveStoriesAsync(models);
    }

    public ICommand AddStoryCommand => new Command(() =>
    {
        var newStory = new StoryViewModel(new Models.Story("New Story", ""), SaveAllAsync);
        Stories.Add(newStory);
        SaveAllAsync();
    });

    public ICommand OpenStoryCommand => new Command<StoryViewModel>(async (story) =>
    {
        if (story != null)
        {
            await Shell.Current.GoToAsync(nameof(Views.EditorPage), true, new Dictionary<string, object>
            {
                ["Story"] = story
            });
        }
    });
}
