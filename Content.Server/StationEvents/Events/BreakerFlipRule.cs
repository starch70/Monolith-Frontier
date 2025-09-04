// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 Morber
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2024 MilenVolf
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2025 Coenx-flex
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.StationEvents.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using JetBrains.Annotations;

namespace Content.Server.StationEvents.Events;

[UsedImplicitly]
public sealed class BreakerFlipRule : StationEventSystem<BreakerFlipRuleComponent>
{
    [Dependency] private readonly ApcSystem _apcSystem = default!;

    protected override void Added(EntityUid uid, BreakerFlipRuleComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        var str = Loc.GetString("station-event-breaker-flip-announcement", ("data", Loc.GetString(Loc.GetString($"random-sentience-event-data-{RobustRandom.Next(1, 6)}"))));
        stationEvent.StartAnnouncement = str;

        base.Added(uid, component, gameRule, args);

    }

    protected override void Started(EntityUid uid, BreakerFlipRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TryGetRandomStations(gameRule.NumberOfGrids.Min, gameRule.NumberOfGrids.Max, out var stations))
            return;

        var stationApcs = new List<Entity<ApcComponent>>();
        var query = EntityQueryEnumerator<ApcComponent, TransformComponent>();
        while (query.MoveNext(out var apcUid, out var apc, out var xform))
        {
            var station = CompOrNull<StationMemberComponent>(xform.GridUid)?.Station;
            if (apc.MainBreakerEnabled && station.HasValue && stations.Contains(station.Value))
            {
                stationApcs.Add((apcUid, apc));
            }
        }

        var toDisable = Math.Min(RobustRandom.Next(3, 7), stationApcs.Count);
        if (toDisable == 0)
            return;

        RobustRandom.Shuffle(stationApcs);

        for (var i = 0; i < toDisable; i++)
        {
            _apcSystem.ApcToggleBreaker(stationApcs[i], stationApcs[i]);
        }
    }
}
