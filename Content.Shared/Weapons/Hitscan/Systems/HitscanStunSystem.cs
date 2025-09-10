// SPDX-FileCopyrightText: 2025 beck-thompson
// SPDX-FileCopyrightText: 2025 bitcrushing
//
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Systems;
using Content.Shared.Weapons.Hitscan.Components;
using Content.Shared.Weapons.Hitscan.Events;

namespace Content.Shared.Weapons.Hitscan.Systems;

public sealed class HitscanStunSystem : EntitySystem
{
    [Dependency] private readonly StaminaSystem _stamina = default!; // Mono - SharedStaminaSystem not ported yet

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HitscanStaminaDamageComponent, HitscanRaycastFiredEvent>(OnHitscanHit, after: [ typeof(HitscanReflectSystem) ]);
    }

    private void OnHitscanHit(Entity<HitscanStaminaDamageComponent> hitscan, ref HitscanRaycastFiredEvent args)
    {
        if (args.Canceled || args.HitEntity == null)
            return;

        _stamina.TakeStaminaDamage(args.HitEntity.Value, hitscan.Comp.StaminaDamage, source: args.Shooter ?? args.Gun);
    }
}
