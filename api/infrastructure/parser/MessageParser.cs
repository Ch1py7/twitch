using System.Text.RegularExpressions;
using api.Domain.Entities;

namespace api.Infrastructure.Parser
{
    public class MessageParser
    {
        public string messageType { get; set; } = "";
        private readonly string rawMessage;

        public MessageParser(string message)
        {
            rawMessage = message;
            var parsedMessage = ParseTwitchMessage();

            messageType = DetectMessageType(message);
        }
        public TwitchMessage ParseTwitchMessage()
        {
            var parsedTags = new Dictionary<string, string>();

            int spaceIndex = rawMessage.IndexOf(' ');
            string tagsPart = spaceIndex != -1 ? rawMessage.Substring(0, spaceIndex) : rawMessage;
            string messagePart = spaceIndex != -1 ? rawMessage.Substring(spaceIndex + 1) : "";

            foreach (var part in tagsPart.Split(';'))
            {
                var keyValue = part.Split(new[] { '=' }, 2);
                string key = keyValue[0];
                string value = keyValue.Length > 1 ? keyValue[1] : "";

                parsedTags[key] = value;
            }

            var (channel, messageContent) = ExtractChannelAndMessage(messagePart);

            return new TwitchMessage(
                Date: parsedTags.GetValueOrDefault("tmi-sent-ts", ""),
                Channel: channel,
                ChannelId: parsedTags.GetValueOrDefault("room-id", ""),
                FirstMsg: parsedTags.TryGetValue("first-msg", out var firstMsg) && firstMsg == "1",
                UserInfo: new UserInfo(
                    UserName: parsedTags.GetValueOrDefault("display-name", ""),
                    Color: parsedTags.GetValueOrDefault("color", ""),
                    UserId: parsedTags.GetValueOrDefault("user-id", ""),
                    IsSubscriber: parsedTags.TryGetValue("subscriber", out var sub) && sub == "1",
                    SubscriberMonths: parsedTags.TryGetValue("badge-info", out var badgeInfo) ? badgeInfo.Split('/')[1] : "0",
                    IsVip: parsedTags.TryGetValue("vip", out var vip) && vip == "1",
                    Message: messageContent
                )
            );
        }
        private string DetectMessageType(string message) => Regex.Match(message, @"tmi.twitch.tv\s*(\S+)").Groups[1].Value ?? "UNKNOWN";

        private (string channel, string messageContent) ExtractChannelAndMessage(string message)
        {
            var match = Regex.Match(message, @"PRIVMSG #(\S+) :(.*)");
            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value);
            }
            return ("", "");
        }
    }
}
