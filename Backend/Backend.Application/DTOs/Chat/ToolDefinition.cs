using System.Text.Json.Nodes;

namespace Backend.Application.DTOs.Chat;

/// <summary>
/// Описание инструмента, доступного AI в чате (tool calling / MCP).
/// </summary>
public class ToolDefinition
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    /// <summary>JSON Schema параметров инструмента.</summary>
    public JsonObject Parameters { get; set; } = new();
}
