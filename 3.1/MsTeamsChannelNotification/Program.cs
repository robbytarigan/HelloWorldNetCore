using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MsTeamsChannelNotification
{
    class Program
    {
        private const string Colour = "FADA5E"; // Stil De Grain Yellow
        private const string Title = "My System Warning";
        private static readonly string SubTitle = $"{DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}";
        private const string Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
        private const string MicrosoftTeamsImageBaseUrl = "https://example.cloudfront.net";
        private const string MicrosoftTeamsNotificationChannelUrl = "https://outlook.office.com/webhook/example";

        static async Task Main(string[] args)
        {
            var potentialActions = new MSTeamsAction[]
            {
                new MSTeamsAction
                {
                    Type = "OpenUri",
                    Name = "View Logs",
                    Targets = new List<MSTeamsActionTarget>
                    {
                        new MSTeamsActionTarget
                        {
                            OS = "default",
                            Uri = "http://localhost:8080"
                        }
                    }
                }
            };

            var image = $"{MicrosoftTeamsImageBaseUrl}/elastalert/redshift-failed.png";

            var facts = new MSTeamsFact[]
            {
                new MSTeamsFact
                {
                    Name = "Table",
                    Value = @"aircraft\_all\_history"
                },
                new MSTeamsFact
                {
                    Name = "Load Date",
                    Value = "2020-12-08 03:00:00"
                }
            };

            var sections = new MSTeamsSection[]
            {
                new MSTeamsSection
                {
                    StartGroup = true,
                    Markdown = true,
                    ActivityImage = image,
                    ActivityTitle = Title,
                    ActivitySubtitle = SubTitle,
                    Facts = facts,
                    Text = Summary
                }
            };

            var body = new MSTeamsBody
            {
                Type = "MessageCard",
                Context = "http://schema.org/extensions",
                ThemeColor = Colour,
                Summary = Summary,
                Sections = sections,
                PotentialAction = potentialActions
            };


            using var bodyStream = new MemoryStream();

            await JsonSerializer.SerializeAsync<MSTeamsBody>(bodyStream, body);

            bodyStream.Seek(0, SeekOrigin.Begin);

            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, MicrosoftTeamsNotificationChannelUrl) { Content = new StreamContent(bodyStream) };

            var response = await client.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response status {response.StatusCode}, Content = {responseString}");
        }
    }

    public sealed class MSTeamsBody
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        [JsonPropertyName("@context")]
        public string Context { get; set; }

        [JsonPropertyName("themeColor")]
        public string ThemeColor { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("sections")]
        public IEnumerable<MSTeamsSection> Sections { get; set; }

        [JsonPropertyName("potentialAction")]
        public IEnumerable<MSTeamsAction> PotentialAction { get; set; }
    }

    public sealed class MSTeamsSection
    {
        [JsonPropertyName("startGroup")]
        public bool StartGroup { get; set; }

        [JsonPropertyName("markdown")]
        public bool Markdown { get; set; }

        [JsonPropertyName("activityImage")]
        public string ActivityImage { get; set; }

        [JsonPropertyName("activitytitle")]
        public string ActivityTitle { get; set; }

        [JsonPropertyName("activitySubtitle")]
        public string ActivitySubtitle { get; set; }

        [JsonPropertyName("facts")]
        public IEnumerable<MSTeamsFact> Facts { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public sealed class MSTeamsAction
    {
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("targets")]
        public IEnumerable<MSTeamsActionTarget> Targets { get; set; }
    }

    public sealed class MSTeamsActionTarget
    {
        [JsonPropertyName("os")]
        public string OS { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

    public sealed class MSTeamsFact
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
