using System.Linq;
using Content.Server._Harmony.GameTicking.Rules;
using Content.Server._Harmony.Objectives.Components;
using Content.Server.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;
using Robust.Shared.Random;

namespace Content.Server._Harmony.Objectives.Systems;

public sealed class PermaPrisonerTargetSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PermaPrisonerRuleSystem _permaPrisonerRule = default!;
    [Dependency] private readonly TargetObjectiveSystem _target = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PermaPrisonerTargetComponent, ObjectiveAssignedEvent>(OnAssigned);
    }

    private void OnAssigned(EntityUid uid, PermaPrisonerTargetComponent comp, ref ObjectiveAssignedEvent args)
    {
        // invalid prototype
        if (!TryComp<TargetObjectiveComponent>(uid, out var target))
        {
            args.Cancelled = true;
            return;
        }

        var prisoners = _permaPrisonerRule.GetAllPrisonerMinds().ToList();

        // no prisoners are found
        if (prisoners.Count == 0)
        {
            args.Cancelled = true;
            return;
        }

        _target.SetTarget(uid, _random.Pick(prisoners).Owner, target);
    }
}
