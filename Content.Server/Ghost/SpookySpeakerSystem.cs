using Content.Server.Chat.Systems;
using Content.Shared.Ghost.Components;
using Content.Shared.Ghost.EntitySystems;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;

namespace Content.Server.Ghost;

public sealed class SpookySpeakerSystem : SharedSpookySpeakerSystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    protected override void SendMessage(Entity<SpookySpeakerComponent> entity)
    {
        if (!_proto.TryIndex(entity.Comp.MessageSet, out var messages))
            return;

        // Grab a random localized message from the set
        var message = Random.Pick(messages);
        // Chatcode moment: messages starting with '.' are considered radio messages unless prefixed with '>'
        // So this is a stupid trick to make the "...Oooo"-style messages work.
        message = '>' + message;
        // Say the message
        _chat.TrySendInGameICMessage(entity, message, InGameICChatType.Speak, hideChat: true);
    }
}
