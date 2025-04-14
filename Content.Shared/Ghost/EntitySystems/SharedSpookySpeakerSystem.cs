using Content.Shared.Ghost.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Ghost.EntitySystems;

public abstract class SharedSpookySpeakerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpookySpeakerComponent, BooEvent>(OnGhostBoo);
    }

    private void OnGhostBoo(Entity<SpookySpeakerComponent> entity, ref BooEvent args)
    {
        // Include the NetID in the random seed so all the machines don't have the same randomness
        var random = new System.Random((int)_timing.CurTick.Value + MetaData(entity).NetEntity.Id);
        // Only activate sometimes, so groups don't all trigger together
        if (!random.Prob(entity.Comp.SpeakChance))
            return;

        var curTime = _timing.CurTime;
        // Enforce a delay between messages to prevent spam
        if (curTime < entity.Comp.NextSpeakTime)
            return;

        SendMessage(entity);

        // Set the delay for the next message
        entity.Comp.NextSpeakTime = curTime + entity.Comp.Cooldown;
        Dirty(entity);

        args.Handled = true;
    }

    // Couldn't find a way to predict sending a chat message
    protected virtual void SendMessage(Entity<SpookySpeakerComponent> entity) {}
}
