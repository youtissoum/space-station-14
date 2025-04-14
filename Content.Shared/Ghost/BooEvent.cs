namespace Content.Shared.Ghost;

[ByRefEvent]
public record struct BooEvent(
    bool Handled = false
);
