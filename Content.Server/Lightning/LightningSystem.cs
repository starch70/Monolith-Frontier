// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2023 AJCM-git
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2024 Emisse
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2024 TinManTim
// SPDX-FileCopyrightText: 2024 lzk
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 Ilya246
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Beam;
using Content.Server.Beam.Components;
using Content.Server.Lightning.Components;
using Content.Shared.Lightning;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes; // Mono
using Robust.Shared.Random;

namespace Content.Server.Lightning;

// TheShuEd:
//I've redesigned the lightning system to be more optimized.
//Previously, each lightning element, when it touched something, would try to branch into nearby entities.
//So if a lightning bolt was 20 entities long, each one would check its surroundings and have a chance to create additional lightning...
//which could lead to recursive creation of more and more lightning bolts and checks.

//I redesigned so that lightning branches can only be created from the point where the lightning struck, no more collide checks
//and the number of these branches is explicitly controlled in the new function.
public sealed class LightningSystem : SharedLightningSystem
{
    [Dependency] private readonly BeamSystem _beam = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!; // Mono
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LightningComponent, ComponentRemove>(OnRemove);
    }

    private void OnRemove(EntityUid uid, LightningComponent component, ComponentRemove args)
    {
        if (!TryComp<BeamComponent>(uid, out var lightningBeam) || !TryComp<BeamComponent>(lightningBeam.VirtualBeamController, out var beamController))
        {
            return;
        }

        beamController.CreatedBeams.Remove(uid);
    }

    /// <summary>
    /// Fires lightning from user to target
    /// </summary>
    /// <param name="user">Where the lightning fires from</param>
    /// <param name="target">Where the lightning fires to</param>
    /// <param name="lightningPrototype">The prototype for the lightning to be created</param>
    /// <param name="triggerLightningEvents">if the lightnings being fired should trigger lightning events.</param>
    public void ShootLightning(EntityUid user, EntityUid target, string lightningPrototype = "Lightning", bool triggerLightningEvents = true)
    {
        // Mono
        EntProtoId? spawnOnHit = null;
        var proto = _proto.Index(lightningPrototype);
        if (proto.TryGetComponent<LightningComponent>(out var lightningComp, EntityManager.ComponentFactory))
            spawnOnHit = lightningComp.SpawnOnHit;

        ShootLightning(user, target, lightningPrototype, triggerLightningEvents);
    }

    // Mono - for optimisation purposes
    private void ShootLightning(EntityUid user, EntityUid target, EntProtoId? spawnOnHit, string lightningPrototype = "Lightning", bool triggerLightningEvents = true)
    {
        var spriteState = LightningRandomizer();
        _beam.TryCreateBeam(user, target, lightningPrototype, spriteState);

        if (triggerLightningEvents) // we don't want certain prototypes to trigger lightning level events
        {
            var ev = new HitByLightningEvent(user, target);
            RaiseLocalEvent(target, ref ev);
        }

        if (spawnOnHit != null)
            Spawn(spawnOnHit.Value, _transform.GetMapCoordinates(target));
    }


    /// <summary>
    /// Looks for objects with a LightningTarget component in the radius, prioritizes them, and hits the highest priority targets with lightning.
    /// </summary>
    /// <param name="user">Where the lightning fires from</param>
    /// <param name="range">Targets selection radius</param>
    /// <param name="boltCount">Number of lightning bolts</param>
    /// <param name="lightningPrototype">The prototype for the lightning to be created</param>
    /// <param name="arcDepth">how many times to recursively fire lightning bolts from the target points of the first shot.</param>
    /// <param name="triggerLightningEvents">if the lightnings being fired should trigger lightning events.</param>
    public void ShootRandomLightnings(EntityUid user, float range, int boltCount, string lightningPrototype = "Lightning", int arcDepth = 0, bool triggerLightningEvents = true)
    {
        // Mono
        EntProtoId? spawnOnHit = null;
        var proto = _proto.Index(lightningPrototype);
        if (proto.TryGetComponent<LightningComponent>(out var lightningComp, EntityManager.ComponentFactory))
            spawnOnHit = lightningComp.SpawnOnHit;

        ShootRandomLightnings(user, range, boltCount, spawnOnHit, lightningPrototype, arcDepth, triggerLightningEvents);
    }

    // Mono - for optimisation purposes
    private void ShootRandomLightnings(EntityUid user, float range, int boltCount, EntProtoId? spawnOnHit, string lightningPrototype = "Lightning", int arcDepth = 0, bool triggerLightningEvents = true)
    {
        //TODO: add support to different priority target tablem for different lightning types
        //TODO: Remove Hardcode LightningTargetComponent (this should be a parameter of the SharedLightningComponent)
        //TODO: This is still pretty bad for perf but better than before and at least it doesn't re-allocate
        // several hashsets every time

        var targets = _lookup.GetEntitiesInRange<LightningTargetComponent>(_transform.GetMapCoordinates(user), range).ToList();
        _random.Shuffle(targets);
        targets.Sort((x, y) => y.Comp.Priority.CompareTo(x.Comp.Priority));

        int shootedCount = 0;
        int count = -1;
        while(shootedCount < boltCount)
        {
            count++;

            if (count >= targets.Count) { break; }

            var curTarget = targets[count];
            if (!_random.Prob(curTarget.Comp.HitProbability)) //Chance to ignore target
                continue;

            ShootLightning(user, targets[count].Owner, spawnOnHit, lightningPrototype, triggerLightningEvents);
            if (arcDepth - targets[count].Comp.LightningResistance > 0)
            {
                ShootRandomLightnings(targets[count].Owner, range, 1, spawnOnHit, lightningPrototype, arcDepth - targets[count].Comp.LightningResistance, triggerLightningEvents);
            }
            shootedCount++;
        }
    }
}

/// <summary>
/// Raised directed on the target when an entity becomes the target of a lightning strike (not when touched)
/// </summary>
/// <param name="Source">The entity that created the lightning</param>
/// <param name="Target">The entity that was struck by lightning.</param>
[ByRefEvent]
public readonly record struct HitByLightningEvent(EntityUid Source, EntityUid Target);
