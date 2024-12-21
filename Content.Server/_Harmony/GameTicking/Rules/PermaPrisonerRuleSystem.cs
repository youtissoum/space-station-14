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

    private const string BriefingLocId = "permaprisoner-role-greeting";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PermaPrisonerRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagSelected);

        SubscribeLocalEvent<PermaPrisonerRoleComponent, GetBriefingEvent>(OnGetBriefing);
    }

    private void AfterAntagSelected(Entity<PermaPrisonerRuleComponent> mindId, ref AfterAntagEntitySelectedEvent args)
    {
        var ent = args.EntityUid;
        _antag.SendBriefing(ent, Loc.GetString(BriefingLocId), null, null);
    }

    private void OnGetBriefing(Entity<PermaPrisonerRoleComponent> role, ref GetBriefingEvent args)
    {
        args.Append(Loc.GetString(BriefingLocId));
    }

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
