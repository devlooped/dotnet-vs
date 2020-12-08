using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace vswhere
{
    public class VisualStudioInstance
    {
        public string InstanceId { get; set; }
        public DateTimeOffset InstallDate { get; set; }
        public string InstallationName { get; set; }
        public string InstallationPath { get; set; }
        [JsonConverter(typeof(JsonConverterVersion))]
        public Version InstallationVersion { get; set; }
        public string ProductId { get; set; }
        public string ProductPath { get; set; }
        public bool IsComplete { get; set; }
        public bool IsLaunchable { get; set; }
        public bool IsPrerelease { get; set; }
        public bool IsRebootRequired { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ChannelId { get; set; }
        public string ChannelUri { get; set; }
        public string EnginePath { get; set; }
        public string ReleaseNotes { get; set; }
        public string ThirdPartyNotices { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public VisualStudioCatalog Catalog { get; set; }
        public VisualStudioProperties Properties { get; set; }
    }

    public class VisualStudioCatalog
    {
        public string BuildBranch { get; set; }
        [JsonConverter(typeof(JsonConverterVersion))]
        public Version BuildVersion { get; set; }
        public string Id { get; set; }
        public string LocalBuild { get; set; }
        public string ManifestName { get; set; }
        public string ManifestType { get; set; }
        public string ProductDisplayVersion { get; set; }
        public string ProductMilestone { get; set; }
        [JsonConverter(typeof(JsonConverterBooleanString))]
        public bool ProductMilestoneIsPreRelease { get; set; }
        public string ProductName { get; set; }
        [JsonConverter(typeof(JsonConverterInt32String))]
        public int ProductPatchVersion { get; set; }
        public string ProductPreReleaseMilestoneSuffix { get; set; }
        public string ProductSemanticVersion { get; set; }
        [JsonConverter(typeof(JsonConverterVersion))]
        public Version RequiredEngineVersion { get; set; }
    }

    public class VisualStudioProperties
    {
        public string CampaignId { get; set; }
        public string ChannelManifestId { get; set; }
        public string Nickname { get; set; }
        public string SetupEngineFilePath { get; set; }
    }

    internal sealed class JsonConverterVersion : JsonConverter<Version>
    {
        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Version.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal sealed class JsonConverterBooleanString : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                return reader.GetBoolean();

            return bool.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLowerInvariant());
        }
    }

    internal sealed class JsonConverterInt32String : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt32();

            return int.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLowerInvariant());
        }
    }
}
