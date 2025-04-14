using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Ghost.Components;
using Content.Shared.Popups;
using Robust.Shared.Random;

namespace Content.Shared.Ghost.EntitySystems;

public sealed class BooSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookupSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<BooerComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<BooerComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<BooerComponent, BooActionEvent>(OnBooAction);
    }

    private void OnMapInit(Entity<BooerComponent> entity, ref MapInitEvent args)
    {
        _actionsSystem.AddAction(entity, ref entity.Comp.ActionEntity, entity.Comp.Action);
    }

    private void OnComponentShutdown(Entity<BooerComponent> entity, ref ComponentShutdown args)
    {
        // No need to remove the action if the entity is being deleted.
        if (Terminating(entity))
            return;

        _actionsSystem.RemoveAction(entity, entity.Comp.ActionEntity);
    }

    private void OnBooAction(Entity<BooerComponent> entity, ref BooActionEvent args)
    {
        if (args.Handled)
            return;

        var entities = _entityLookupSystem.GetEntitiesInRange(args.Performer, entity.Comp.Radius).ToList();
        // Shuffle the possible targets so we don't favor any particular entities
        _random.Shuffle(entities);

        var booCounter = 0;
        foreach (var target in entities)
        {
            var handled = DoBoo(target);

            if (handled)
                booCounter++;

            if (booCounter >= entity.Comp.MaxTargets)
                break;
        }

        if (booCounter == 0)
            _popupSystem.PopupPredicted(Loc.GetString(entity.Comp.FailedMessage), entity, entity);

        args.Handled = true;
    }

    public bool DoBoo(EntityUid target)
    {
        var booEvent = new BooEvent();
        RaiseLocalEvent(target, ref booEvent);

        return booEvent.Handled;
    }
}
