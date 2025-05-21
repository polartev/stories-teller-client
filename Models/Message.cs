using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Story_Teller.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    info,
    task,
    error,
    success
}

public class BasePayload
{
    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; } = new();
}

public class Message
{
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("payload")]
    public BasePayload Payload { get; set; }

    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }
}