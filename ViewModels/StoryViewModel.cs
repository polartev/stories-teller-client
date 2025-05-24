using CommunityToolkit.Mvvm.ComponentModel;

namespace Story_Teller.ViewModels;

public partial class StoryViewModel : ObservableObject
{
    private readonly Action onChanged;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? content;

    public StoryViewModel(Models.Story story, Action onChanged)
    {
        Title = story.Title;
        Content = story.Content;
        this.onChanged = onChanged;
    }

    partial void OnTitleChanged(string? oldValue, string? newValue)
    {
        onChanged?.Invoke();
    }

    partial void OnContentChanged(string? oldValue, string? newValue)
    {
        onChanged?.Invoke();
    }

    public Models.Story ToModel() => new(Title ?? "", Content ?? "");
}