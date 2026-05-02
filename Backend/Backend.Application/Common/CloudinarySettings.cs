namespace Backend.Application.Common;

public class CloudinarySettings
{
    public string CloudName { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public CloudinaryTransformationOptions TransformationOptions { get; set; } = null!;
}

public class CloudinaryTransformationOptions
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Crop { get; set; } = null!;
    public int Quality { get; set; }
    public string Format { get; set; } = null!;
}
