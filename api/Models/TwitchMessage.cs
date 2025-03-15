namespace api.Models;

public record TwitchMessage(
    string Date,
    string Channel,
    string ChannelId,
    bool FirstMsg,
    UserInfo UserInfo
);

public record UserInfo(
    string UserName,
    string Color,
    string UserId,
    bool IsSubscriber,
    string SubscriberMonths,
    bool IsVip,
    string Message
);
