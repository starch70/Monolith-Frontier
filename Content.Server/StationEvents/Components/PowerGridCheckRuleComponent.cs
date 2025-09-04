// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 Coenx-flex
// SPDX-FileCopyrightText: 2025 Winkarst
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading;
using Content.Server.Power.EntitySystems;
using Content.Server.StationEvents.Events;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(PowerGridCheckRule), typeof(PowerMonitoringConsoleSystem))]
public sealed partial class PowerGridCheckRuleComponent : Component
{
    /// <summary>
    /// Default sound of the announcement when power is back on.
    /// </summary>
    private static readonly ProtoId<SoundCollectionPrototype> DefaultPowerOn = new("PowerOn");

    /// <summary>
    /// Sound of the announcement to play when power is back on.
    /// </summary>
    [DataField]
    public SoundSpecifier PowerOnSound = new SoundCollectionSpecifier(DefaultPowerOn, AudioParams.Default.WithVolume(-4f));

    public CancellationTokenSource? AnnounceCancelToken;

    public List<EntityUid> AffectedStations;
    public readonly List<EntityUid> Powered = new();
    public readonly List<EntityUid> Unpowered = new();

    public float SecondsUntilOff = 30.0f;

    public int NumberPerSecond = 0;
    public float UpdateRate => 1.0f / NumberPerSecond;
    public float FrameTimeAccumulator = 0.0f;
}
