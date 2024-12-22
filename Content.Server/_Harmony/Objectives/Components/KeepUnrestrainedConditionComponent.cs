using Content.Server._Harmony.Objectives.Systems;
using Content.Server.Objectives.Components;

namespace Content.Server._Harmony.Objectives.Components;

/// <summary>
/// Requires that a target stays alive and unrestrained.
/// Depends on <see cref="TargetObjectiveComponent"/> to function.
/// </summary>
[RegisterComponent, Access(typeof(KeepUnrestrainedConditionSystem))]
public sealed partial class KeepUnrestrainedConditionComponent : Component;
