using Content.Server._Harmony.Objectives.Components;
using Content.Server.Cuffs;
using Content.Server.Mind;
using Content.Server.Objectives.Systems;
using Content.Shared.Cuffs.Components;
using Content.Shared.Mind;
using Content.Shared.Objectives.Components;

namespace Content.Server._Harmony.Objectives.Systems;

public sealed class KeepUnrestrainedConditionSystem : EntitySystem
{
    [Dependency] private readonly CuffableSystem _cuffableSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly TargetObjectiveSystem _target = default!;

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

        args.Progress = GetProgress((args.MindId, args.Mind));
    }

    public float GetProgress(Entity<MindComponent> mind)
    {
        if (mind.Comp.OwnedEntity is not {} entity || _mindSystem.IsCharacterDeadIc(mind))
            return 0f;

        if (!TryComp<CuffableComponent>(entity, out var cuffable))
            return 0f;

        return _cuffableSystem.IsCuffed((entity, cuffable)) ? 0f : 1f;
    }
}
