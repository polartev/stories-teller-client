using CommunityToolkit.Maui.Views;

namespace Story_Teller.Views;

public partial class BookshelfPage : ContentPage
{
    ViewModels.BookshelfViewModel bookshelfViewModel;

    public BookshelfPage(ViewModels.BookshelfViewModel bookshelfViewModel)
	{
		InitializeComponent();
        BindingContext = bookshelfViewModel;
        this.bookshelfViewModel = bookshelfViewModel;
    }

    string GetCurrentWidthState(double width, string name)
        => width >= 1200 ? $"LargeWidth{name}"
        : width >= 800 ? $"MediumWidth{name}"
                   : $"SmallWidth{name}";

    protected override void OnAppearing()
    {
        base.OnAppearing();

        VisualStateManager.GoToState(main, GetCurrentWidthState(Width, "Main"));
    }

    private async void EditClicked(object sender, EventArgs e)
    {
        var result = await this.ShowPopupAsync(new Popups.EditBookPopup());

        ImageButton? button = sender as ImageButton;

        if (sender is not ImageButton &&
            button?.BindingContext is not ViewModels.StoryViewModel)
        {
            return;
        }

        switch (result as string)
        {
            case "Edit":
                var parent = button.Parent as Grid;
                if (parent?.FindByName<Entry>("TitleEntry") is Entry entry)
                {
                    entry.IsVisible = true;
                    entry.Focus();
                }
                break;
            case "Delete":
                if (button?.BindingContext is ViewModels.StoryViewModel vm) 
                {
                    await bookshelfViewModel.DeleteBookAsync(vm);
                }
                break;
        }
    }

    private void OnTitleEditCompleted(object sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            entry.IsVisible = false;
        }
    }
}