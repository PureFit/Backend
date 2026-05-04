namespace Backend.Infrastructure.Services;

/// <summary>
/// Maps API muscle names (uppercase) to SVG region IDs.
///
/// Collision strategy:
///   Percentage  — Max(percentages) of all muscles sharing one SVG region is used.
///   Target/Syn  — Target wins over Synergist; if any mapped muscle is Target → region is Target.
/// </summary>
public static class MuscleSvgMapping
{
    /// <summary>
    /// Key   = muscle name as returned by the exercise API (case-insensitive).
    /// Value = SVG element ID from SvgMuscleIds.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> Map =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // ── Adductors ────────────────────────────────────────────────
            ["ADDUCTOR LONGUS"]              = SvgMuscleIds.Adductors,
            ["ADDUCTOR BREVIS"]              = SvgMuscleIds.Adductors,
            ["ADDUCTOR MAGNUS"]              = SvgMuscleIds.Adductors,
            ["GRACILIS"]                     = SvgMuscleIds.Adductors,
            ["PECTINEUS"]                    = SvgMuscleIds.Adductors,

            // ── Abductors ────────────────────────────────────────────────
            ["TENSOR FASCIAE LATAE"]         = SvgMuscleIds.Abductors,

            // ── Biceps ───────────────────────────────────────────────────
            ["BICEPS BRACHII"]               = SvgMuscleIds.Biceps,
            ["BRACHIALIS"]                   = SvgMuscleIds.Biceps,

            // ── Forearms ─────────────────────────────────────────────────
            ["BRACHIORADIALIS"]              = SvgMuscleIds.Forearms,
            ["WRIST EXTENSORS"]              = SvgMuscleIds.Forearms,
            ["WRIST FLEXORS"]                = SvgMuscleIds.Forearms,

            // ── Calves ───────────────────────────────────────────────────
            ["GASTROCNEMIUS"]                = SvgMuscleIds.Calves,
            ["SOLEUS"]                       = SvgMuscleIds.Calves,

            // ── Chest ────────────────────────────────────────────────────
            ["PECTORALIS MAJOR CLAVICULAR HEAD"] = SvgMuscleIds.Chest,
            ["PECTORALIS MAJOR STERNAL HEAD"]    = SvgMuscleIds.Chest,

            // ── Delts ────────────────────────────────────────────────────
            ["ANTERIOR DELTOID"]             = SvgMuscleIds.FrontDelts,
            ["LATERAL DELTOID"]              = SvgMuscleIds.SideDelts,
            ["POSTERIOR DELTOID"]            = SvgMuscleIds.RearDelts,

            // ── Glutes ───────────────────────────────────────────────────
            ["GLUTEUS MAXIMUS"]              = SvgMuscleIds.Glutes,
            ["GLUTEUS MEDIUS"]               = SvgMuscleIds.Glutes,
            ["GLUTEUS MINIMUS"]              = SvgMuscleIds.Glutes,
            ["DEEP HIP EXTERNAL ROTATORS"]   = SvgMuscleIds.Glutes,

            // ── Hamstrings ───────────────────────────────────────────────
            ["HAMSTRINGS"]                   = SvgMuscleIds.Hamstrings,
            ["POPLITEUS"]                    = SvgMuscleIds.Hamstrings,

            // ── Lats ─────────────────────────────────────────────────────
            ["LATISSIMUS DORSI"]             = SvgMuscleIds.Lats,
            ["TERES MAJOR"]                  = SvgMuscleIds.Lats,

            // ── Lower back ───────────────────────────────────────────────
            ["ERECTOR SPINAE"]               = SvgMuscleIds.LowerBack,

            // ── Neck ─────────────────────────────────────────────────────
            ["STERNOCLEIDOMASTOID"]          = SvgMuscleIds.Neck,
            ["LEVATOR SCAPULAE"]             = SvgMuscleIds.Neck,
            ["SPLENIUS"]                     = SvgMuscleIds.Neck,

            // ── Abs ──────────────────────────────────────────────────────
            ["RECTUS ABDOMINIS"]             = SvgMuscleIds.Abs,
            ["TRANSVERSUS ABDOMINIS"]        = SvgMuscleIds.Abs,
            ["ILIOPSOAS"]                    = SvgMuscleIds.Abs,    // hip flexor; closest region

            // ── Obliques ─────────────────────────────────────────────────
            ["OBLIQUES"]                     = SvgMuscleIds.Obliques,
            ["SERRATUS ANTERIOR"]            = SvgMuscleIds.Obliques,
            ["SERRATUS ANTE"]                = SvgMuscleIds.Obliques,

            // ── Quads ────────────────────────────────────────────────────
            ["QUADRICEPS"]                   = SvgMuscleIds.Quads,
            ["SARTORIUS"]                    = SvgMuscleIds.Quads,

            // ── Rotator cuffs ────────────────────────────────────────────
            ["INFRASPINATUS"]                = SvgMuscleIds.RotatorCuffs,
            ["SUBSCAPULARIS"]                = SvgMuscleIds.RotatorCuffs,
            ["TERES MINOR"]                  = SvgMuscleIds.RotatorCuffs,

            // ── Shins ────────────────────────────────────────────────────
            ["TIBIALIS ANTERIOR"]            = SvgMuscleIds.Shins,

            // ── Traps ────────────────────────────────────────────────────
            ["TRAPEZIUS UPPER FIBERS"]       = SvgMuscleIds.Traps,
            ["TRAPEZIUS MIDDLE FIBERS"]      = SvgMuscleIds.Traps,
            ["TRAPEZIUS LOWER FIBERS"]       = SvgMuscleIds.Traps,

            // ── Triceps ──────────────────────────────────────────────────
            ["TRICEPS BRACHII"]              = SvgMuscleIds.Triceps,
        };
}
