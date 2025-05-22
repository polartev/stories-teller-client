using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Story_Teller.IServices;
using System.Windows.Input;

namespace Story_Teller.ViewModels;

public partial class FlyoutFooterViewModel : ObservableObject
{
    private readonly ILanguageService languageService;

    [ObservableProperty]
    private string currentLanguage;

    public ICommand SetLanguageCommand { get; }

    public FlyoutFooterViewModel(ILanguageService languageService)
    {
        this.languageService = languageService;
        CurrentLanguage = this.languageService.GetLanguage();
        SetLanguageCommand = new AsyncRelayCommand<string>(SetLanguageAsync);
    }

    private async Task SetLanguageAsync(string? language)
    {
        if (string.IsNullOrEmpty(language) || language == CurrentLanguage)
        {
            return;
        }

        languageService.SetLanguage(language);
        CurrentLanguage = language;
    }
}