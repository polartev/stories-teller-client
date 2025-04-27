namespace Story_Teller.Services;

internal class DataBaseService : IServices.IOnlineService
{
    public string ServiceName { get; set; } = "DataBaseService";

    public bool? IsOnline { get; set; } = false;

    public Task ConnectAsync()
    {
        IsOnline = true;
        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        IsOnline = false;
        return Task.CompletedTask;
    }
}
