namespace Story_Teller.Models;

public record Story(string Sid = "", string Title = "", string Content = "");

public class StoryImage
{
    public string path { get; set; }
}