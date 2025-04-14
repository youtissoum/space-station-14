using Content.Shared.Actions;
using Content.Shared.Ghost.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Ghost.Components;

/// <summary>
/// This component allows an entity to boo
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(BooSystem))]
[AutoGenerateComponentState]
public sealed partial class BooerComponent : Component
{
    [DataField]
    public EntProtoId Action = "ActionGhostBoo";

    [AutoNetworkedField, ViewVariables]
    public EntityUid? ActionEntity;

    [DataField]
    public float Radius = 3;

    [DataField]
    public int MaxTargets = 3;

    [DataField]
    public LocId FailedMessage = "ghost-component-boo-action-failed";
}

[ByRefEvent]
public sealed partial class BooActionEvent : InstantActionEvent;
