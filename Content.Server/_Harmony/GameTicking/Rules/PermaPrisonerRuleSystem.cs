using Content.Server._Harmony.GameTicking.Rules.Components;
using Content.Server._Harmony.Roles;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Roles;
using Content.Shared.Mind;

namespace Content.Server._Harmony.GameTicking.Rules;

public sealed class PermaPrisonerRuleSystem : GameRuleSystem<PermaPrisonerRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;

    public List<Entity<MindComponent>> GetAllPrisonerMinds()
    {
        List<Entity<MindComponent>> allPrisoners = [];

        var query = EntityQueryEnumerator<PermaPrisonerRuleComponent>();
        while (query.MoveNext(out var uid, out var rule))
        {
            foreach (var role in _antag.GetAntagMinds(uid))
            {
                if (!allPrisoners.Contains(role))
                    allPrisoners.Add(role);
            }
        }

        return allPrisoners;
    }
}
