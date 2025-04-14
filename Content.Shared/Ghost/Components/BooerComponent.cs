using Content.Shared.Actions;
using Content.Shared.Ghost.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Ghost.Components;

/// <summary>
/// Causes an entity to gain a "boo" action that allows it to act on a certain number of entities around it.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(BooSystem))]
[AutoGenerateComponentState]
public sealed partial class BooerComponent : Component
{
    /// <summary>
    /// The action that will be added to the entity.
    /// </summary>
    [DataField]
    public EntProtoId Action = "ActionGhostBoo";

    [AutoNetworkedField, ViewVariables]
    public EntityUid? ActionEntity;

    /// <summary>
    /// The radius in which entities will be detected.
    /// </summary>
    [DataField]
    public float Radius = 3;

    /// <summary>
    /// The maximum number of entities to be targeted.
    /// </summary>
    [DataField]
    public int MaxTargets = 3;

    /// <summary>
    /// The message that will be said if no entities manage to be affected when booing.
    /// </summary>
    [DataField]
    public LocId FailedMessage = "ghost-component-boo-action-failed";
}

[ByRefEvent]
public sealed partial class BooActionEvent : InstantActionEvent;
