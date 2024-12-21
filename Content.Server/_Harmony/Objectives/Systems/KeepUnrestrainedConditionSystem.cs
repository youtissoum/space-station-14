using Content.Server._Harmony.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;

namespace Content.Server._Harmony.Objectives.Systems;

public sealed class KeepUnrestrainedConditionSystem : EntitySystem
{
    [Dependency] private readonly TargetObjectiveSystem _target = default!;
    [Dependency] private readonly UnrestrainedConditionSystem _unrestrainedCondition = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KeepUnrestrainedConditionComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnGetProgress(EntityUid uid,
        KeepUnrestrainedConditionComponent comp,
        ref ObjectiveGetProgressEvent args)
    {
        if (!_target.GetTarget(uid, out var target))
            return;

        args.Progress = _unrestrainedCondition.GetProgress((args.MindId, args.Mind));
    }
}
