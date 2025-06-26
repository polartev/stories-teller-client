namespace Story_Teller.Models;

public record Story(string Sid = "", string Title = "", string Content = "");

public class ImagePath
{
    public string path { get; set; }
}

public class ImageItem
{
    public string Path { get; set; }

    public ImageSource Source { get; set; }
}