using Backend.Application.DTOs.Chat;
using Backend.Application.Services;
using System.Text.Json.Nodes;

namespace Backend.Infrastructure.Services;

public class ChatToolProvider : IChatToolProvider
{
    public IReadOnlyList<ToolDefinition> GetTools() =>
    [
        new()
        {
            Name        = "get_user_profile",
            Description = "Returns the user's profile: sex, age, weight, height, and fitness level. Call this when the question requires knowledge of the user's physical characteristics.",
            Parameters  = new JsonObject
            {
                ["type"]       = "object",
                ["properties"] = new JsonObject(),
                ["required"]   = new JsonArray()
            }
        },
        new()
        {
            Name        = "get_active_plan",
            Description = "Returns the user's active training plan: type, duration in weeks, current week, and list of upcoming trainings. Call this when the question is about the user's schedule or training plan.",
            Parameters  = new JsonObject
            {
                ["type"]       = "object",
                ["properties"] = new JsonObject(),
                ["required"]   = new JsonArray()
            }
        }
    ];
}
