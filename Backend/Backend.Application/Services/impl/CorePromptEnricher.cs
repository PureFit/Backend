using Backend.Application.DTOs.Plan;
using System.Text;
using System.Text.Json;

namespace Backend.Application.Services.impl;

public class CorePromptEnricher : ICorePromptEnricher
{
    public AIPrompt Enrich(AIPrompt prompt, GeneratePlanRequest request)
    {
        var sb = new StringBuilder(prompt.UserMessage);

        sb.AppendLine();
        sb.AppendLine("=== COACHING INSTRUCTIONS ===");

        AppendWeightGuidelines(sb, request);
        AppendSubTypeInstructions(sb, request);
        AppendProgressionRules(sb, request);

        prompt.UserMessage = sb.ToString();
        return prompt;
    }

    // ─── Weight & Intensity Guidelines ───────────────────────────────────────

    private static void AppendWeightGuidelines(StringBuilder sb, GeneratePlanRequest request)
    {
        sb.AppendLine();
        sb.AppendLine("--- WEIGHT & INTENSITY GUIDELINES ---");

        var level = request.FitnessLevel?.ToLower();
        var weight = request.WeightKg;

        // Base intensity by level
        var (minPct, maxPct, rirNote) = level switch
        {
            "beginner"     => (40, 60, "Leave 3-4 reps in reserve (RIR 3-4). Focus on technique."),
            "returning"    => (50, 65, "Leave 2-3 reps in reserve (RIR 2-3). Rebuild movement patterns."),
            "intermediate" => (65, 80, "Leave 1-2 reps in reserve (RIR 1-2). Progressive overload each week."),
            "advanced"     => (75, 90, "Train close to failure (RIR 0-1). Include periodization."),
            "elite"        => (80, 95, "Train at competition intensity. Autoregulate based on daily readiness."),
            _              => (60, 75, "Moderate intensity. Adjust based on performance.")
        };

        sb.AppendLine($"Working weight: {minPct}-{maxPct}% of 1RM");
        sb.AppendLine($"Effort level: {rirNote}");

        if (weight.HasValue)
        {
            sb.AppendLine($"User bodyweight: {weight} kg — use this for bodyweight exercise scaling and relative strength estimates.");
            sb.AppendLine($"Estimated strength baselines (intermediate natural athlete ~{weight} kg):");
            sb.AppendLine($"  Squat ~{(int)(weight.Value * 1.2)}kg | Bench ~{(int)(weight.Value * 0.9)}kg | Deadlift ~{(int)(weight.Value * 1.4)}kg | OHP ~{(int)(weight.Value * 0.6)}kg");
            sb.AppendLine("Adjust these estimates based on stated fitness level.");
        }
    }

    // ─── SubType-specific Instructions ───────────────────────────────────────

    private static void AppendSubTypeInstructions(StringBuilder sb, GeneratePlanRequest request)
    {
        sb.AppendLine();
        sb.AppendLine("--- GOAL-SPECIFIC INSTRUCTIONS ---");

        var meta = string.IsNullOrWhiteSpace(request.GoalMetadata)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.GoalMetadata);

        switch (request.PlanSubType)
        {
            case "Competitions":
                AppendCompetitions(sb, request, meta);
                break;
            case "StrengthGain":
                AppendStrengthGain(sb, request, meta);
                break;
            case "Endurance":
                AppendEndurance(sb, request, meta);
                break;
            case "Custom":
                AppendCustom(sb, request, meta);
                break;
            case "FatLoss":
                AppendFatLoss(sb, request, meta);
                break;
            case "Hypertrophy":
                AppendHypertrophy(sb, request, meta);
                break;
            case "BodyPartFocus":
                AppendBodyPartFocus(sb, request, meta);
                break;
            case "FullBodyRelief":
                AppendFullBodyRelief(sb, request);
                break;
            case "PostureAndBack":
                AppendPostureAndBack(sb, request, meta);
                break;
            case "MobilityFlexibility":
                AppendMobilityFlexibility(sb, request);
                break;
            case "GFP":
                AppendGFP(sb, request, meta);
                break;
        }
    }

    private static void AppendCompetitions(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var sport = meta?.GetValueOrDefault("sport").GetString() ?? "sport";
        var competitionDate = meta?.GetValueOrDefault("competitionDate").GetString();

        sb.AppendLine($"Goal: Competition preparation for {sport}.");
        if (!string.IsNullOrEmpty(competitionDate))
            sb.AppendLine($"Competition date: {competitionDate} — structure the entire plan around this deadline.");

        sb.AppendLine();
        sb.AppendLine("Apply macrocycle periodization across the plan duration:");
        sb.AppendLine($"  Phase 1 — Base/Accumulation (25% of weeks): Low intensity, high volume.");
        sb.AppendLine("    Focus: joint/ligament conditioning, cardiovascular base, movement technique.");
        sb.AppendLine("    Sets: 3-5 | Reps: 12-20 | Intensity: 50-65% 1RM | Rest: 60-90s");
        sb.AppendLine($"  Phase 2 — Intensification (50% of weeks): Progressive overload toward peak.");
        sb.AppendLine("    Focus: sport-specific strength, speed, and specialized endurance. Train at competition pace.");
        sb.AppendLine("    Sets: 4-6 | Reps: 3-8 | Intensity: 75-90% 1RM | Rest: 2-4min");
        sb.AppendLine($"  Phase 3 — Taper/Peaking (25% of weeks): Supercompensation and recovery.");
        sb.AppendLine("    Reduce volume by 40-50%, maintain intensity. Peak for competition day.");
        sb.AppendLine("    Sets: 3-4 | Reps: 2-5 | Intensity: 85-95% 1RM | Rest: 3-5min");
        sb.AppendLine();
        sb.AppendLine($"Requested sessions per week: {request.SessionsPerWeek}. Use this as the target, but you may reduce if the goal or recovery demands require it. Justify via session structure.");
        sb.AppendLine($"Adapt all exercises to be sport-specific for {sport}. Prioritize movement patterns used in competition.");
        sb.AppendLine("Include deload weeks every 3-4 weeks (reduce volume 30%, keep intensity).");
    }

    private static void AppendStrengthGain(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var targetBodyPart = meta?.GetValueOrDefault("targetBodyPart").GetString() ?? "";
        var targetExercise = meta?.GetValueOrDefault("targetExercise").GetString() ?? "";
        var currentMax = meta?.GetValueOrDefault("currentMaxKg").GetDouble() ?? 0;
        var targetMax = meta?.GetValueOrDefault("targetMaxKg").GetDouble() ?? 0;

        sb.AppendLine("Goal: Maximal strength development.");
        if (!string.IsNullOrEmpty(targetExercise))
        {
            sb.AppendLine($"Target lift: {targetExercise}");
            if (currentMax > 0) sb.AppendLine($"Current 1RM: {currentMax} kg");
            if (targetMax > 0)  sb.AppendLine($"Target 1RM: {targetMax} kg — plan progressive milestones across weeks.");
        }
        if (!string.IsNullOrEmpty(targetBodyPart))
            sb.AppendLine($"Priority body part: {targetBodyPart}");

        sb.AppendLine();
        sb.AppendLine("Strength programming guidelines:");
        sb.AppendLine("  Main compound lifts: 4-6 sets × 1-5 reps @ 80-95% 1RM. These come first in the session.");
        sb.AppendLine("  Accessory work: 3-4 sets × 6-10 reps @ 65-75% 1RM to address weak points.");
        sb.AppendLine("  Rest between heavy sets: 3-5 minutes.");
        sb.AppendLine("  Apply linear or undulating periodization — increase load 2.5-5kg per week on main lifts.");
        sb.AppendLine("  Include technique work at 50-60% 1RM on deload weeks (every 4th week).");
        sb.AppendLine("  CNS fatigue management: no more than 2 maximal effort sessions per week.");
    }

    private static void AppendEndurance(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var activity = meta?.GetValueOrDefault("activity").GetString() ?? "running";
        var currentResult = meta?.GetValueOrDefault("currentResult").GetString() ?? "";
        var targetResult = meta?.GetValueOrDefault("targetResult").GetString() ?? "";

        sb.AppendLine($"Goal: Endurance improvement for {activity}.");
        if (!string.IsNullOrEmpty(currentResult)) sb.AppendLine($"Current performance: {currentResult}");
        if (!string.IsNullOrEmpty(targetResult))  sb.AppendLine($"Target performance: {targetResult}");

        sb.AppendLine();
        sb.AppendLine("Endurance programming guidelines:");
        sb.AppendLine("  80/20 rule: 80% of cardio sessions at low intensity (Zone 2, conversational pace), 20% at high intensity.");
        sb.AppendLine("  Include: long slow sessions, tempo intervals, VO2max intervals, and recovery sessions.");
        sb.AppendLine("  Strength work: focus on posterior chain, single-leg stability, and core — supports {activity} mechanics.");
        sb.AppendLine("  Increase weekly volume no more than 10% per week to prevent overuse injuries.");
        sb.AppendLine("  Every 4th week: reduce volume by 30% (recovery week).");
        sb.AppendLine($"  Activity-specific drills for {activity} should be integrated into warm-up.");
    }

    private static void AppendCustom(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var goals = meta?.GetValueOrDefault("goals").EnumerateArray().Select(g => g.GetString()).ToList() ?? [];
        var focusMuscles = meta?.GetValueOrDefault("focusMuscles").EnumerateArray().Select(m => m.GetString()).ToList() ?? [];
        var focusExercises = meta?.GetValueOrDefault("focusExercises").GetString() ?? "";

        sb.AppendLine("Goal: Custom combination training.");
        if (goals.Count > 0)
            sb.AppendLine($"User goals: {string.Join(", ", goals)}");
        if (focusMuscles.Count > 0)
            sb.AppendLine($"Priority muscles: {string.Join(", ", focusMuscles)}");
        if (!string.IsNullOrEmpty(focusExercises))
            sb.AppendLine($"Exercises to include: {focusExercises}");

        sb.AppendLine();
        sb.AppendLine("Balance the stated goals intelligently. If conflicting (e.g. strength + endurance), prioritize based on order listed.");
        sb.AppendLine("Apply appropriate periodization for the dominant goal. Include variety to address all stated objectives.");
    }

    private static void AppendFatLoss(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var calories = meta?.GetValueOrDefault("estimatedDailyCalories").GetInt32() ?? 0;
        var targetWeight = meta?.GetValueOrDefault("targetWeightKg").GetDouble() ?? 0;
        var targetDate = meta?.GetValueOrDefault("targetDate").GetString() ?? "";

        sb.AppendLine("Goal: Fat loss while preserving muscle mass.");
        if (calories > 0)      sb.AppendLine($"Estimated daily intake: {calories} kcal — user is likely in a caloric deficit.");
        if (targetWeight > 0)  sb.AppendLine($"Target weight: {targetWeight} kg");
        if (!string.IsNullOrEmpty(targetDate)) sb.AppendLine($"Target date: {targetDate}");

        sb.AppendLine();
        sb.AppendLine("Fat loss training guidelines:");
        sb.AppendLine("  Prioritize compound movements to maximize caloric expenditure and preserve muscle.");
        sb.AppendLine("  Rep ranges: 8-15 reps with moderate weight (60-75% 1RM). Short rest: 45-90s to elevate heart rate.");
        sb.AppendLine("  Include metabolic circuits 1-2x per week (supersets, tri-sets).");
        sb.AppendLine("  Add low-intensity cardio (20-40 min Zone 2) on non-lifting days or post-session.");
        sb.AppendLine("  Avoid excessive volume — recovery is harder in a deficit. 3-4 working sets per exercise.");
        if (calories > 0 && calories < 1800)
            sb.AppendLine("  WARNING: Very low calorie intake detected. Reduce training volume to prevent muscle catabolism.");
        sb.AppendLine("  Deload every 4th week — critical in a deficit as recovery capacity is reduced.");
    }

    private static void AppendHypertrophy(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var calories = meta?.GetValueOrDefault("estimatedDailyCalories").GetInt32() ?? 0;
        var targetWeight = meta?.GetValueOrDefault("targetWeightKg").GetDouble() ?? 0;
        var targetMuscles = meta?.GetValueOrDefault("targetMuscles").EnumerateArray().Select(m => m.GetString()).ToList() ?? [];

        sb.AppendLine("Goal: Muscle hypertrophy (size and mass gain).");
        if (calories > 0)           sb.AppendLine($"Estimated daily intake: {calories} kcal — user is likely in a caloric surplus.");
        if (targetWeight > 0)       sb.AppendLine($"Target weight: {targetWeight} kg");
        if (targetMuscles.Count > 0) sb.AppendLine($"Priority muscle groups: {string.Join(", ", targetMuscles)} — allocate additional volume here.");

        sb.AppendLine();
        sb.AppendLine("Hypertrophy programming guidelines:");
        sb.AppendLine("  Optimal rep range: 6-12 reps @ 65-80% 1RM. Train within 1-2 reps of failure on working sets.");
        sb.AppendLine("  Weekly volume per muscle group: 10-20 sets (beginners: 10-12, advanced: 16-20).");
        sb.AppendLine("  Rest between sets: 90-120s for isolation, 2-3min for compound movements.");
        sb.AppendLine("  Apply progressive overload: add weight or reps each week.");
        sb.AppendLine("  Split: push/pull/legs or upper/lower for optimal frequency (each muscle 2x/week).");
        if (targetMuscles.Count > 0)
            sb.AppendLine($"  Add 2-3 extra isolation sets for priority muscles: {string.Join(", ", targetMuscles)}.");
        sb.AppendLine("  Include mind-muscle connection cues in exercise notes.");
    }

    private static void AppendBodyPartFocus(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var bodyPart = meta?.GetValueOrDefault("bodyPart").GetString() ?? "";
        var characteristic = meta?.GetValueOrDefault("characteristic").GetString() ?? "Relief";

        sb.AppendLine($"Goal: Aesthetic focus on {bodyPart} — {characteristic}.");
        sb.AppendLine();

        if (characteristic == "Mass")
        {
            sb.AppendLine($"Building mass in {bodyPart}:");
            sb.AppendLine($"  Allocate 40-50% of session volume to {bodyPart} exercises.");
            sb.AppendLine("  Rep range: 6-12 @ 70-80% 1RM. Progressive overload weekly.");
            sb.AppendLine("  Use compound + isolation combination. Stretch under load for hypertrophy.");
        }
        else
        {
            sb.AppendLine($"Building definition/relief in {bodyPart}:");
            sb.AppendLine($"  Combine strength work (8-12 reps) with higher rep finishers (15-20 reps).");
            sb.AppendLine("  Include metabolic stress techniques: supersets, drop sets on last working set.");
            sb.AppendLine("  Add cardio to support overall body fat reduction.");
        }

        sb.AppendLine($"  Maintain balanced training for other body parts to avoid postural imbalances.");
    }

    private static void AppendFullBodyRelief(StringBuilder sb, GeneratePlanRequest request)
    {
        sb.AppendLine("Goal: Full body muscle definition and relief.");
        sb.AppendLine();
        sb.AppendLine("Full body relief guidelines:");
        sb.AppendLine("  Full-body sessions 3x/week or upper/lower split 4x/week.");
        sb.AppendLine("  Rep range: 10-15 reps @ 60-70% 1RM. Moderate rest: 60-90s.");
        sb.AppendLine("  Superset antagonist muscles (chest/back, biceps/triceps) to save time and increase density.");
        sb.AppendLine("  Include cardio finishers (10-15 min HIIT or circuits) at end of sessions.");
        sb.AppendLine("  Prioritize compound movements (squat, deadlift, press, row) — maximum muscle activation.");
        sb.AppendLine("  Progressive overload still applies — do not sacrifice load for rep count.");
    }

    private static void AppendPostureAndBack(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var symptoms = meta?.GetValueOrDefault("symptoms").GetString() ?? "";

        sb.AppendLine("Goal: Posture correction and back health.");
        if (!string.IsNullOrEmpty(symptoms))
            sb.AppendLine($"Reported symptoms/issues: {symptoms}");

        sb.AppendLine();
        sb.AppendLine("Posture & back rehabilitation guidelines:");
        sb.AppendLine("  SAFETY FIRST: Avoid heavy axial loading (heavy squats/deadlifts) in early phases.");
        sb.AppendLine("  Phase 1 (weeks 1-3): Activation and mobility. Focus on deep stabilizers (multifidus, transversus abdominis).");
        sb.AppendLine("    Exercises: dead bug, bird-dog, cat-cow, glute bridges, face pulls, band pull-aparts.");
        sb.AppendLine("  Phase 2 (weeks 4+): Strengthen posterior chain with controlled movements.");
        sb.AppendLine("    Exercises: Romanian deadlift (light), rows, reverse flies, hip hinges, planks.");
        sb.AppendLine("  Cue: retract and depress scapulae in all pulling movements.");
        sb.AppendLine("  Include thoracic mobility work every session (2-3 exercises).");
        sb.AppendLine("  Avoid: forward head posture exercises, excessive chest pressing without balancing rows.");
        if (!string.IsNullOrEmpty(symptoms) && symptoms.ToLower().Contains("pain"))
            sb.AppendLine("  IMPORTANT: User reports pain — keep intensity conservative, prioritize pain-free range of motion.");
    }

    private static void AppendMobilityFlexibility(StringBuilder sb, GeneratePlanRequest request)
    {
        sb.AppendLine("Goal: Improved mobility and flexibility.");
        sb.AppendLine();
        sb.AppendLine("Mobility & flexibility programming guidelines:");
        sb.AppendLine("  Structure each session: dynamic warm-up (10min) → strength/stability work → static stretching (10min).");
        sb.AppendLine("  Dynamic mobility: leg swings, hip circles, thoracic rotations, arm circles — before loading.");
        sb.AppendLine("  Static stretching: hold 30-60s per position, only after muscles are warm.");
        sb.AppendLine("  Include PNF stretching for faster flexibility gains (contract 6s, relax, deepen stretch).");
        sb.AppendLine("  Priority areas: hip flexors, hamstrings, thoracic spine, shoulders, ankles.");
        sb.AppendLine("  Strength component: resistance training through full range of motion builds functional flexibility.");
        sb.AppendLine("  Progress: increase range of motion gradually — never force stretch to pain.");
    }

    private static void AppendGFP(StringBuilder sb, GeneratePlanRequest request, Dictionary<string, JsonElement>? meta)
    {
        var description = meta?.GetValueOrDefault("description").GetString() ?? "";

        sb.AppendLine("Goal: General Physical Preparation (GPP) — well-rounded fitness base.");
        if (!string.IsNullOrEmpty(description))
            sb.AppendLine($"User description: {description}");

        sb.AppendLine();
        sb.AppendLine("GPP programming guidelines:");
        sb.AppendLine("  Develop all physical qualities: strength, endurance, mobility, coordination, power.");
        sb.AppendLine("  Use full-body workouts or push/pull/legs split — hit every major muscle group 2x/week.");
        sb.AppendLine("  Rep ranges: vary week to week (strength week: 4-6 reps, hypertrophy week: 8-12, endurance week: 15-20).");
        sb.AppendLine("  Include one dedicated cardio session per week (Zone 2, 30-45 min).");
        sb.AppendLine("  Include one mobility/flexibility session or add 10min mobility to each session.");
        sb.AppendLine("  Introduce basic movement patterns: squat, hinge, push, pull, carry, rotate.");
        sb.AppendLine("  Keep sessions varied and engaging — GPP benefits from exercise variety.");
    }

    // ─── Universal Progression Rules ─────────────────────────────────────────

    private static void AppendProgressionRules(StringBuilder sb, GeneratePlanRequest request)
    {
        sb.AppendLine();
        sb.AppendLine("--- UNIVERSAL PROGRESSION RULES ---");
        sb.AppendLine("- Each week should be slightly harder than the previous (add weight, reps, or reduce rest).");
        sb.AppendLine("- Week-over-week load increase: 2.5-5kg for upper body, 5-10kg for lower body lifts.");
        sb.AppendLine("- If user cannot complete prescribed reps with good form — keep weight, add reps first.");
        sb.AppendLine("- Deload protocol: every 3-4 weeks reduce volume by 30-40%, maintain intensity.");
        sb.AppendLine($"- Session duration must not exceed {request.SessionDurationMinutes} minutes including warm-up.");
        sb.AppendLine($"- Requested sessions per week: {request.SessionsPerWeek}. You may schedule fewer if recovery or goal structure demands it, but never more. Consider user wishes when deciding.");
        sb.AppendLine("- Warm-up: 5-10 min general + 2-3 activation sets for main movement (50% → 70% working weight).");
        sb.AppendLine("- Cool-down: 5 min light stretching of worked muscles.");

        if (request.FitnessLevel?.ToLower() == "beginner" || request.FitnessLevel?.ToLower() == "returning")
        {
            sb.AppendLine("- BEGINNER NOTE: Technique takes absolute priority over load. Mark technique cues in exercise notes.");
            sb.AppendLine("- Start conservatively — it is better to underestimate and progress fast than to injure.");
        }
    }
}
