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

    private void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button &&
            button.BindingContext is ViewModels.StoryViewModel vm)
        {
            var parent = button.Parent as Grid;
            if (parent?.FindByName<Entry>("TitleEntry") is Entry entry)
            {
                entry.IsVisible = true;
                entry.Focus();
            }
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