using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Story_Teller.ViewModels;

public partial class BookshelfViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<StoryViewModel> stories = new();

    public IEnumerable<StoryViewModel> StoriesReversed => Stories.Reverse();

    partial void OnStoriesChanged(ObservableCollection<StoryViewModel> value)
    {
        OnPropertyChanged(nameof(StoriesReversed));
    }

    private IServices.IStoryStorageService storyStorageService;

    public BookshelfViewModel(IServices.IStoryStorageService storyStorageService)
    {
        this.storyStorageService = storyStorageService;

        LoadAsync();
    }

    private async void LoadAsync()
    {
        var loaded = await storyStorageService.LoadAsync();
        Stories.Clear();
        foreach (var story in loaded)
        {
            Stories.Add(new StoryViewModel(story, SaveAllAsync));
        }

        OnPropertyChanged(nameof(StoriesReversed));
    }

    private async void SaveAllAsync()
    {
        var models = Stories.Select(story => story.ToModel());
        await storyStorageService.SaveAsync(models);
    }

    public async Task DeleteBookAsync(StoryViewModel storyVM)
    {
        Stories.Remove(storyVM);
        await storyStorageService.DeleteAsync(storyVM.ToModel());

        OnPropertyChanged(nameof(StoriesReversed));
    }

    [RelayCommand]
    private Task OnAddStoryTapped()
    {
        var story = new StoryViewModel(new Models.Story(Guid.NewGuid().ToString(), "New Story", ""), SaveAllAsync);
        Stories.Add(story);
        SaveAllAsync();

        OnPropertyChanged(nameof(StoriesReversed));

        return Shell.Current.GoToAsync(nameof(Views.EditorPage), true, new Dictionary<string, object>
        {
            ["Story"] = story
        });
    }

    [RelayCommand]
    private async Task OnOpenStoryTappedAsync(StoryViewModel story)
    {
        if (story != null)
        {
            await Shell.Current.GoToAsync(nameof(Views.EditorPage), true, new Dictionary<string, object>
            {
                ["Story"] = story
            });
        }
    }
}
