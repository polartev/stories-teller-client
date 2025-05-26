using CommunityToolkit.Maui.Views;

namespace Story_Teller.Views.Popups;

public partial class EditBookPopup : Popup
{
	public EditBookPopup()
	{
		InitializeComponent();
	}

    private void OnVariantButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            Close(button.Text);
        }
        else
        {
            Close(null);
        }
    }
}