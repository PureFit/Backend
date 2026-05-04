using System.Xml.Linq;
using Backend.Application.DTOs.Muscles;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class SvgManipulationService : ISvgManipulationService
{
    // Target/Synergist colors
    private const string ColorTarget     = "#FF4444";
    private const string ColorSynergist  = "#FFA726";

    public string ApplyPercentages(string svgContent, IEnumerable<MusclePercentageItem> muscles)
    {
        var svgActivations = new Dictionary<string, float>(StringComparer.Ordinal);
        foreach (var m in muscles)
        {
            if (!MuscleSvgMapping.Map.TryGetValue(m.MuscleName, out var svgMuscleName)) continue;
            var pct = Math.Clamp(m.Percentage, 0f, 100f);
            if (!svgActivations.TryGetValue(svgMuscleName, out var current) || pct > current)
                svgActivations[svgMuscleName] = pct;
        }

        var doc = XDocument.Parse(svgContent);
        foreach (var (svgMuscleName, pct) in svgActivations)
        {
            if (pct < 1f) continue; // skip zero/near-zero — leave default color

            var element = FindById(doc, svgMuscleName);
            element?.SetAttributeValue("fill", PercentageToColor(pct));
        }

        return doc.Root!.ToString(SaveOptions.DisableFormatting);
    }

    public string ApplyTargetSynergist(string svgContent, IEnumerable<string> targetMuscleNames, IEnumerable<string> synergistMuscleNames)
    {
        // Resolve to SVG IDs; Target wins over Synergist for the same region
        var svgColors = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var name in synergistMuscleNames)
        {
            if (!MuscleSvgMapping.Map.TryGetValue(name, out var svgId)) continue;
            svgColors[svgId] = ColorSynergist;
        }

        foreach (var name in targetMuscleNames)
        {
            if (!MuscleSvgMapping.Map.TryGetValue(name, out var svgId)) continue;
            svgColors[svgId] = ColorTarget; // overwrite synergist if same region
        }

        var doc = XDocument.Parse(svgContent);
        foreach (var (svgId, color) in svgColors)
        {
            var element = FindById(doc, svgId);
            element?.SetAttributeValue("fill", color);
        }

        return doc.Root!.ToString(SaveOptions.DisableFormatting);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private static XElement? FindById(XDocument doc, string id) =>
        doc.Descendants().FirstOrDefault(e => (string?)e.Attribute("id") == id);

    /// <summary>
    /// 3-stop gradient:
    ///   1–50%  : #FFEB3B (yellow)  → #FF9800 (orange)
    ///   50–100%: #FF9800 (orange)  → #FF1744 (red)
    /// </summary>
    private static string PercentageToColor(float pct)
    {
        var t = pct / 100f;
        int r, g, b;

        if (t <= 0.5f)
        {
            var u = t / 0.5f;
            r = 255;
            g = Lerp(235, 152, u);
            b = Lerp(59,  0,   u);
        }
        else
        {
            var u = (t - 0.5f) / 0.5f;
            r = 255;
            g = Lerp(152, 23, u);
            b = Lerp(0,   68, u);
        }

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static int Lerp(int from, int to, float t) =>
        (int)Math.Round(from + (to - from) * t);
}
