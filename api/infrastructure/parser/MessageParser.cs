using System.Drawing;
using System.Text.RegularExpressions;
using api.Models;

namespace api.infrastructure.parser
{
    public class MessageParser
    {
        private readonly Dictionary<string, string> parsedMessage;
        private string messageType { get; set; } = "";
        private readonly string rawMessage;

        public MessageParser(string message)
        {
            this.rawMessage = message;
            this.parsedMessage = message.Split(';')
                .Where(msg => msg.Contains("="))
                .Select(msg => msg.Split('='))
                .Where(args => args.Length == 2)
                .ToDictionary(args => args[0], args => args[1]);

            this.messageType = this.DetectMessageType(message);
        }
        public TwitchMessage? ToTwitchMessage()
        {
            if (this.messageType != "PRIVMSG") return null;

            var (channel, messageContent) = ExtractChannelAndMessage();

            return new TwitchMessage
            {
                Date = parsedMessage.GetValueOrDefault("tmi-sent-ts", ""),
                Channel = channel,
                ChannelId = parsedMessage.GetValueOrDefault("room-id", ""),
                FirstMsg = parsedMessage.GetValueOrDefault("first-msg", "0") == "1",
                UserInfo = ExtractUserInfo(messageContent)
            };
        }
        private string DetectMessageType(string message) => Regex.Match(message, @"tmi.twitch.tv\s*(\S+)").Groups[1].Value ?? "UNKNOWN";

        private (string channel, string messageContent) ExtractChannelAndMessage()
        {
            var match = Regex.Match(this.rawMessage, @"PRIVMSG #(\S+) :(.*)");
            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value);
            }
            return ("", "");
        }
        private UserInfo ExtractUserInfo(string messageContent)
        {
            parsedMessage.TryGetValue("display-name", out string? displayName);
            parsedMessage.TryGetValue("color", out string? color);
            parsedMessage.TryGetValue("user-id", out string? userId);
            parsedMessage.TryGetValue("@badge-info", out string? subscriberInfo);

            return new UserInfo
            {
                DisplayName = displayName ?? "",
                Color = color ?? "",
                UserId = userId ?? "",
                IsSubscriber = !String.IsNullOrEmpty(subscriberInfo) && subscriberInfo.Contains("subscriber"),
                SubscriberMonths = !string.IsNullOrEmpty(subscriberInfo) && subscriberInfo.Contains("/")
                                    ? subscriberInfo.Split("/").ElementAtOrDefault(1) ?? ""
                                    : "",
                Message = messageContent,
            };
        }
    }
}
