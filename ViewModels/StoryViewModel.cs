using CommunityToolkit.Mvvm.ComponentModel;

namespace Story_Teller.ViewModels;

public partial class StoryViewModel : ObservableObject
{
    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? content;

    public StoryViewModel(Models.Story story)
    {
        Title = story.Title;
        Content = story.Content;
    }
}
