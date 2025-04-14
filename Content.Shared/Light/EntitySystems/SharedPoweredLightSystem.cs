using Content.Shared.Ghost;
using Content.Shared.Light.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Light.EntitySystems;

public abstract class SharedPoweredLightSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PoweredLightComponent, BooEvent>(OnGhostBoo);
    }

    private void OnGhostBoo(Entity<PoweredLightComponent> entity, ref BooEvent args)
    {
        if (entity.Comp.IgnoreGhostsBoo)
            return;

        // check cooldown first to prevent abuse
        var time = _gameTiming.CurTime;
        if (time < entity.Comp.NextGhostBlink)
            return;

        DoBoo(entity);

        entity.Comp.NextGhostBlink = time + entity.Comp.GhostBlinkingCooldown;
        Dirty(entity);

        args.Handled = true;
    }

    protected virtual void DoBoo(Entity<PoweredLightComponent> entity) {}
}
