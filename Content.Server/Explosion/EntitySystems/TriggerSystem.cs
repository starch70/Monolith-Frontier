// SPDX-FileCopyrightText: 2019 Injazz
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 Chris V
// SPDX-FileCopyrightText: 2022 EmoGarbage404
// SPDX-FileCopyrightText: 2022 Jack Fox
// SPDX-FileCopyrightText: 2022 Júlio César Ueti
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 ScalyChimp
// SPDX-FileCopyrightText: 2022 Snowni
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 themias
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2023 AJCM
// SPDX-FileCopyrightText: 2023 AlexMorgan3817
// SPDX-FileCopyrightText: 2023 Arendian
// SPDX-FileCopyrightText: 2023 Doru991
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2023 checkraze
// SPDX-FileCopyrightText: 2023 eclips_e
// SPDX-FileCopyrightText: 2024 Aexxie
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2024 Dvir
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2024 KISS
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 Whatstone
// SPDX-FileCopyrightText: 2024 Winkarst
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 to4no_fix
// SPDX-FileCopyrightText: 2025 Ark
// SPDX-FileCopyrightText: 2025 Redrover1760
// SPDX-FileCopyrightText: 2025 TemporalOroboros
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Administration.Logs;
using Content.Server.Body.Systems;
using Content.Server.Explosion.Components;
using Content.Server.Flash;
using Content.Server.Electrocution;
using Content.Server.Pinpointer;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Flash.Components;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Database;
using Content.Shared.Explosion.Components;
using Content.Shared.Explosion.Components.OnTrigger;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Payload.Components;
using Content.Shared.Radio;
using Content.Shared.Slippery;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Trigger;
using Content.Shared.Weapons.Ranged.Events;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Content.Server.Station.Systems;
using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Body.Components; // Frontier: Gib organs
using Content.Shared.Projectiles; // Frontier: embed triggers
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;

namespace Content.Server.Explosion.EntitySystems
{
    /// <summary>
    /// Raised whenever something is Triggered on the entity.
    /// </summary>
    public sealed class TriggerEvent : HandledEntityEventArgs
    {
        public EntityUid Triggered { get; }
        public EntityUid? User { get; }

        public TriggerEvent(EntityUid triggered, EntityUid? user = null)
        {
            Triggered = triggered;
            User = user;
        }
    }

    /// <summary>
    /// Raised when timer trigger becomes active.
    /// </summary>
    [ByRefEvent]
    public readonly record struct ActiveTimerTriggerEvent(EntityUid Triggered, EntityUid? User);

    [UsedImplicitly]
    public sealed partial class TriggerSystem : EntitySystem
    {
        [Dependency] private readonly ExplosionSystem _explosions = default!;
        [Dependency] private readonly FixtureSystem _fixtures = default!;
        [Dependency] private readonly FlashSystem _flashSystem = default!;
        [Dependency] private readonly SharedBroadphaseSystem _broadphase = default!;
        [Dependency] private readonly IAdminLogManager _adminLogger = default!;
        [Dependency] private readonly SharedContainerSystem _container = default!;
        [Dependency] private readonly BodySystem _body = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
        [Dependency] private readonly NavMapSystem _navMap = default!;
        [Dependency] private readonly RadioSystem _radioSystem = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly InventorySystem _inventory = default!;
        [Dependency] private readonly ElectrocutionSystem _electrocution = default!;
        [Dependency] private readonly StationSystem _station = default!; // Frontier: medical insurance

        public override void Initialize()
        {
            base.Initialize();

            InitializeProximity();
            InitializeOnUse();
            InitializeSignal();
            InitializeTimedCollide();
            InitializeVoice();
            InitializeMobstate();
            InitializeBeingGibbed(); // Frontier

            SubscribeLocalEvent<TriggerOnSpawnComponent, MapInitEvent>(OnSpawnTriggered);
            SubscribeLocalEvent<TriggerOnCollideComponent, StartCollideEvent>(OnTriggerCollide);
            SubscribeLocalEvent<TriggerOnActivateComponent, ActivateInWorldEvent>(OnActivate);
            SubscribeLocalEvent<TriggerImplantActionComponent, ActivateImplantEvent>(OnImplantTrigger);
            SubscribeLocalEvent<TriggerOnStepTriggerComponent, StepTriggeredOffEvent>(OnStepTriggered);
            SubscribeLocalEvent<TriggerOnSlipComponent, SlipEvent>(OnSlipTriggered);
            SubscribeLocalEvent<TriggerWhenEmptyComponent, OnEmptyGunShotEvent>(OnEmptyTriggered);
            SubscribeLocalEvent<RepeatingTriggerComponent, MapInitEvent>(OnRepeatInit);
            SubscribeLocalEvent<TriggerOnProjectileHitComponent, ProjectileHitEvent>(OnProjectileHitEvent); // Frontier: trigger on embed

            SubscribeLocalEvent<SpawnOnTriggerComponent, TriggerEvent>(OnSpawnTrigger);
            SubscribeLocalEvent<DeleteOnTriggerComponent, TriggerEvent>(HandleDeleteTrigger);
            SubscribeLocalEvent<ExplodeOnTriggerComponent, TriggerEvent>(HandleExplodeTrigger);
            SubscribeLocalEvent<FlashOnTriggerComponent, TriggerEvent>(HandleFlashTrigger);
            SubscribeLocalEvent<GibOnTriggerComponent, TriggerEvent>(HandleGibTrigger);

            SubscribeLocalEvent<AnchorOnTriggerComponent, TriggerEvent>(OnAnchorTrigger);
            SubscribeLocalEvent<SoundOnTriggerComponent, TriggerEvent>(OnSoundTrigger);
            SubscribeLocalEvent<ShockOnTriggerComponent, TriggerEvent>(HandleShockTrigger);
            SubscribeLocalEvent<RattleComponent, TriggerEvent>(HandleRattleTrigger);
        }

        private void OnSoundTrigger(EntityUid uid, SoundOnTriggerComponent component, TriggerEvent args)
        {
            if (component.RemoveOnTrigger) // if the component gets removed when it's triggered
            {
                var xform = Transform(uid);
                _audio.PlayPvs(component.Sound, xform.Coordinates); // play the sound at its last known coordinates
            }
            else // if the component doesn't get removed when triggered
            {
                _audio.PlayPvs(component.Sound, uid); // have the sound follow the entity itself
            }
        }

        private void HandleShockTrigger(Entity<ShockOnTriggerComponent> shockOnTrigger, ref TriggerEvent args)
        {
            if (!_container.TryGetContainingContainer(shockOnTrigger.Owner, out var container))
                return;

            var containerEnt = container.Owner;
            var curTime = _timing.CurTime;

            if (curTime < shockOnTrigger.Comp.NextTrigger)
            {
                // The trigger's on cooldown.
                return;
            }

            _electrocution.TryDoElectrocution(containerEnt, null, shockOnTrigger.Comp.Damage, shockOnTrigger.Comp.Duration, true);
            shockOnTrigger.Comp.NextTrigger = curTime + shockOnTrigger.Comp.Cooldown;
        }

        private void OnAnchorTrigger(EntityUid uid, AnchorOnTriggerComponent component, TriggerEvent args)
        {
            var xform = Transform(uid);

            if (xform.Anchored)
                return;

            _transformSystem.AnchorEntity(uid, xform);

            if (component.RemoveOnTrigger)
                RemCompDeferred<AnchorOnTriggerComponent>(uid);
        }

        private void OnSpawnTrigger(EntityUid uid, SpawnOnTriggerComponent component, TriggerEvent args)
        {
            var xform = Transform(uid);

            var coords = xform.Coordinates;

            if (!coords.IsValid(EntityManager))
                return;

            Spawn(component.Proto, coords);
        }

        private void HandleExplodeTrigger(EntityUid uid, ExplodeOnTriggerComponent component, TriggerEvent args)
        {
            _explosions.TriggerExplosive(uid, user: args.User);
            args.Handled = true;
        }

        private void HandleFlashTrigger(EntityUid uid, FlashOnTriggerComponent component, TriggerEvent args)
        {
            // TODO Make flash durations sane ffs.
            _flashSystem.FlashArea(uid, args.User, component.Range, component.Duration * 1000f, probability: component.Probability);
            args.Handled = true;
        }

        private void HandleDeleteTrigger(EntityUid uid, DeleteOnTriggerComponent component, TriggerEvent args)
        {
            EntityManager.QueueDeleteEntity(uid);
            args.Handled = true;
        }

        // Frontier: more configurable gib triggers
        private void HandleGibTrigger(EntityUid uid, GibOnTriggerComponent component, TriggerEvent args)
        {
            EntityUid ent;
            if (component.UseArgumentEntity)
            {
                ent = uid;
            }
            else
            {
                if (!TryComp(uid, out TransformComponent? xform))
                    return;
                ent = xform.ParentUid;
            }

            if (component.DeleteItems)
            {
                var items = _inventory.GetHandOrInventoryEntities(ent);
                foreach (var item in items)
                {
                    Del(item);
                }
            }

            if (component.DeleteOrgans) // Frontier - Gib organs
            {
                if (TryComp<BodyComponent>(ent, out var body))
                {
                    var organs = _body.GetBodyOrganEntityComps<TransformComponent>((ent, body));
                    foreach (var organ in organs)
                    {
                        Del(organ.Owner);
                    }
                }
            } // Frontier

            if (component.Gib)
                _body.GibBody(ent, true);
            args.Handled = true;
        }
        // End Frontier

        // Frontier: custom function implementation
        private void HandleRattleTrigger(EntityUid uid, RattleComponent component, TriggerEvent args)
        {
            if (!TryComp<SubdermalImplantComponent>(uid, out var implanted))
                return;

            if (implanted.ImplantedEntity == null)
                return;

            // Gets location of the implant
            var ownerXform = Transform(uid);
            var pos = ownerXform.MapPosition;
            var x = (int) pos.X;
            var y = (int) pos.Y;
            var posText = $"({x}, {y})";

            // Frontier: Gets station location of the implant
            var station = _station.GetOwningStation(uid);
            var stationText = station is null ? null : $"{Name(station.Value)} ";

            if (stationText == null)
                stationText = "";

            // Frontier: Gets species of the implant user
            var speciesText = $"";
            if (TryComp<HumanoidAppearanceComponent>(implanted.ImplantedEntity, out var species))
                speciesText = $" ({species!.Species})";

            var critMessage = Loc.GetString(component.CritMessage, ("user", implanted.ImplantedEntity.Value), ("specie", speciesText), ("grid", stationText!), ("position", posText));
            var deathMessage = Loc.GetString(component.DeathMessage, ("user", implanted.ImplantedEntity.Value), ("specie", speciesText), ("grid", stationText!), ("position", posText));

            if (!TryComp<MobStateComponent>(implanted.ImplantedEntity, out var mobstate))
                return;

            if (mobstate.CurrentState != MobState.Alive)
            {
                // Check if this is a TSF job
                var isTSF = false;
                if (TryComp<MindContainerComponent>(implanted.ImplantedEntity, out var mindContainer) &&
                    mindContainer.Mind.HasValue &&
                    TryComp<MindComponent>(mindContainer.Mind.Value, out var mindComp))
                {
                    string jobTitle = "";

                    // Try to get job name from the mind roles
                    foreach (var roleId in mindComp.MindRoles)
                    {
                        if (!TryComp<MindRoleComponent>(roleId, out var mindRole) || mindRole.JobPrototype == null)
                            continue;

                        if (!_prototypeManager.TryIndex(mindRole.JobPrototype.Value, out var jobPrototype))
                            continue;

                        jobTitle = jobPrototype.LocalizedName;
                        break;
                    }

                    isTSF = jobTitle.Equals(Loc.GetString("job-name-bailiff"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-brigmedic"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-cadet-nf"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-deputy"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-nf-detective"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-sheriff"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-stc"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-sr"), StringComparison.OrdinalIgnoreCase) ||
                            jobTitle.Equals(Loc.GetString("job-name-pal"), StringComparison.OrdinalIgnoreCase);
                }

                // Sends a message to the radio channel specified by the implant
                if (mobstate.CurrentState == MobState.Critical)
                {
                    // Use TSF channel for TSF jobs, otherwise use the original channel
                    RadioChannelPrototype radioChannel;
                    if (isTSF)
                    {
                        // Use explicit ProtoId for TSF channel
                        radioChannel = _prototypeManager.Index<RadioChannelPrototype>(new ProtoId<RadioChannelPrototype>("Nfsd"));
                    }
                    else
                    {
                        // Use component's channel directly
                        radioChannel = _prototypeManager.Index<RadioChannelPrototype>(component.RadioChannel);
                    }

                    _radioSystem.SendRadioMessage(uid, critMessage, radioChannel, uid);
                }

                if (mobstate.CurrentState == MobState.Dead)
                {
                    var radioChannel = _prototypeManager.Index<RadioChannelPrototype>(component.RadioChannel);
                    _radioSystem.SendRadioMessage(uid, deathMessage, radioChannel, uid);
                }
            }

            args.Handled = true;
        }
        // End Frontier

        private void OnTriggerCollide(EntityUid uid, TriggerOnCollideComponent component, ref StartCollideEvent args)
        {
            if (args.OurFixtureId == component.FixtureID && (!component.IgnoreOtherNonHard || args.OtherFixture.Hard))
                Trigger(uid, args.OtherEntity);
        }

        private void OnSpawnTriggered(EntityUid uid, TriggerOnSpawnComponent component, MapInitEvent args)
        {
            Trigger(uid);
        }

        private void OnActivate(EntityUid uid, TriggerOnActivateComponent component, ActivateInWorldEvent args)
        {
            if (args.Handled || !args.Complex)
                return;

            Trigger(uid, args.User);
            args.Handled = true;
        }

        private void OnImplantTrigger(EntityUid uid, TriggerImplantActionComponent component, ActivateImplantEvent args)
        {
            args.Handled = Trigger(uid);
        }

        private void OnStepTriggered(EntityUid uid, TriggerOnStepTriggerComponent component, ref StepTriggeredOffEvent args)
        {
            Trigger(uid, args.Tripper);
        }

        private void OnSlipTriggered(EntityUid uid, TriggerOnSlipComponent component, ref SlipEvent args)
        {
            Trigger(uid, args.Slipped);
        }

        private void OnEmptyTriggered(EntityUid uid, TriggerWhenEmptyComponent component, ref OnEmptyGunShotEvent args)
        {
            Trigger(uid, args.EmptyGun);
        }

        // Frontier: embed triggers
        private void OnProjectileHitEvent(EntityUid uid, TriggerOnProjectileHitComponent component, ref ProjectileHitEvent args)
        {
            Trigger(uid, args.Target);
        }
        // End Frontier

        private void OnRepeatInit(Entity<RepeatingTriggerComponent> ent, ref MapInitEvent args)
        {
            ent.Comp.NextTrigger = _timing.CurTime + ent.Comp.Delay;
        }

        public bool Trigger(EntityUid trigger, EntityUid? user = null)
        {
            var triggerEvent = new TriggerEvent(trigger, user);
            EntityManager.EventBus.RaiseLocalEvent(trigger, triggerEvent, true);
            return triggerEvent.Handled;
        }

        public void TryDelay(EntityUid uid, float amount, ActiveTimerTriggerComponent? comp = null)
        {
            if (!Resolve(uid, ref comp, false))
                return;

            comp.TimeRemaining += amount;
        }

        /// <summary>
        /// Start the timer for triggering the device.
        /// </summary>
        public void StartTimer(Entity<OnUseTimerTriggerComponent?> ent, EntityUid? user)
        {
            if (!Resolve(ent, ref ent.Comp, false))
                return;

            var comp = ent.Comp;
            HandleTimerTrigger(ent, user, comp.Delay, comp.BeepInterval, comp.InitialBeepDelay, comp.BeepSound);
        }

        public void HandleTimerTrigger(EntityUid uid, EntityUid? user, float delay, float beepInterval, float? initialBeepDelay, SoundSpecifier? beepSound)
        {
            if (delay <= 0)
            {
                RemComp<ActiveTimerTriggerComponent>(uid);
                Trigger(uid, user);
                return;
            }

            if (HasComp<ActiveTimerTriggerComponent>(uid))
                return;

            if (user != null)
            {
                // Check if entity is bomb/mod. grenade/etc
                if (_container.TryGetContainer(uid, "payload", out BaseContainer? container) &&
                    container.ContainedEntities.Count > 0 &&
                    TryComp(container.ContainedEntities[0], out ChemicalPayloadComponent? chemicalPayloadComponent))
                {
                    // If a beaker is missing, the entity won't explode, so no reason to log it
                    if (chemicalPayloadComponent?.BeakerSlotA.Item is not { } beakerA ||
                        chemicalPayloadComponent?.BeakerSlotB.Item is not { } beakerB ||
                        !TryComp(beakerA, out SolutionContainerManagerComponent? containerA) ||
                        !TryComp(beakerB, out SolutionContainerManagerComponent? containerB) ||
                        !TryComp(beakerA, out FitsInDispenserComponent? fitsA) ||
                        !TryComp(beakerB, out FitsInDispenserComponent? fitsB) ||
                        !_solutionContainerSystem.TryGetSolution((beakerA, containerA), fitsA.Solution, out _, out var solutionA) ||
                        !_solutionContainerSystem.TryGetSolution((beakerB, containerB), fitsB.Solution, out _, out var solutionB))
                        return;

                    _adminLogger.Add(LogType.Trigger,
                        $"{ToPrettyString(user.Value):user} started a {delay} second timer trigger on entity {ToPrettyString(uid):timer}, which contains {SharedSolutionContainerSystem.ToPrettyString(solutionA)} in one beaker and {SharedSolutionContainerSystem.ToPrettyString(solutionB)} in the other.");
                }
                else
                {
                    _adminLogger.Add(LogType.Trigger,
                        $"{ToPrettyString(user.Value):user} started a {delay} second timer trigger on entity {ToPrettyString(uid):timer}");
                }

            }
            else
            {
                _adminLogger.Add(LogType.Trigger,
                    $"{delay} second timer trigger started on entity {ToPrettyString(uid):timer}");
            }

            var active = AddComp<ActiveTimerTriggerComponent>(uid);
            active.TimeRemaining = delay;
            active.User = user;
            active.BeepSound = beepSound;
            active.BeepInterval = beepInterval;
            active.TimeUntilBeep = initialBeepDelay == null ? active.BeepInterval : initialBeepDelay.Value;

            var ev = new ActiveTimerTriggerEvent(uid, user);
            RaiseLocalEvent(uid, ref ev);

            if (TryComp<AppearanceComponent>(uid, out var appearance))
                _appearance.SetData(uid, TriggerVisuals.VisualState, TriggerVisualState.Primed, appearance);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            UpdateProximity();
            UpdateTimer(frameTime);
            UpdateTimedCollide(frameTime);
            UpdateRepeat();
        }

        private void UpdateTimer(float frameTime)
        {
            HashSet<EntityUid> toRemove = new();
            var query = EntityQueryEnumerator<ActiveTimerTriggerComponent>();
            while (query.MoveNext(out var uid, out var timer))
            {
                timer.TimeRemaining -= frameTime;
                timer.TimeUntilBeep -= frameTime;

                if (timer.TimeRemaining <= 0)
                {
                    Trigger(uid, timer.User);
                    toRemove.Add(uid);
                    continue;
                }

                if (timer.BeepSound == null || timer.TimeUntilBeep > 0)
                    continue;

                timer.TimeUntilBeep += timer.BeepInterval;
                _audio.PlayPvs(timer.BeepSound, uid, timer.BeepSound.Params);
            }

            foreach (var uid in toRemove)
            {
                RemComp<ActiveTimerTriggerComponent>(uid);

                // In case this is a re-usable grenade, un-prime it.
                if (TryComp<AppearanceComponent>(uid, out var appearance))
                    _appearance.SetData(uid, TriggerVisuals.VisualState, TriggerVisualState.Unprimed, appearance);
            }
        }

        private void UpdateRepeat()
        {
            var now = _timing.CurTime;
            var query = EntityQueryEnumerator<RepeatingTriggerComponent>();
            while (query.MoveNext(out var uid, out var comp))
            {
                if (comp.NextTrigger > now)
                    continue;

                comp.NextTrigger = now + comp.Delay;
                Trigger(uid);
            }
        }
    }
}
