namespace api.Models
{
    public class TwitchMessage
    {
        public string Date { get; set; } = String.Empty;
        public string Channel { get; set; } = String.Empty;
        public string ChannelId { get; set; } = String.Empty;
        public bool FirstMsg { get; set; }
        public UserInfo UserInfo { get; set; } = new UserInfo();
    }

}

public class UserInfo
{
    public string UserName { get; set; } = String.Empty;
    public string DisplayName { get; set; } = String.Empty;
    public string Color { get; set; } = String.Empty;
    public string UserId { get; set; } = String.Empty;
    public bool IsSubscriber { get; set; }
    public string SubscriberMonths { get; set; } = String.Empty;
    public string IsVip { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
}