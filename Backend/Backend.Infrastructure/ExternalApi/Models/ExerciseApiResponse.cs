using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend.Infrastructure.ExternalApi.Models
{
    public class ExerciseApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("meta")]
        public ApiMeta Meta { get; set; } = null!;

        [JsonPropertyName("data")]
        public List<ExternalExerciseDto> Data { get; set; } = new();
    }

    public class ApiMeta
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonPropertyName("nextCursor")]
        public string? NextCursor { get; set; } // Тот самый идентификатор для сидинга

        [JsonPropertyName("previousCursor")]
        public string? PreviousCursor { get; set; }
    }

    public class ExternalExerciseDto
    {
        [JsonPropertyName("exerciseId")]
        public string ExerciseId { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = null!;

        [JsonPropertyName("bodyParts")]
        public List<string> BodyParts { get; set; } = new();

        [JsonPropertyName("equipments")]
        public List<string> Equipments { get; set; } = new();

        [JsonPropertyName("exerciseType")]
        public string ExerciseType { get; set; } = null!;

        [JsonPropertyName("targetMuscles")]
        public List<string> TargetMuscles { get; set; } = new();

        [JsonPropertyName("secondaryMuscles")]
        public List<string> SecondaryMuscles { get; set; } = new();

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new();
    }
}
