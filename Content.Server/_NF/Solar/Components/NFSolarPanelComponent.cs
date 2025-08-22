// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 Redrover1760
// SPDX-FileCopyrightText: 2025 Whatstone
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._NF.Solar.EntitySystems;

namespace Content.Server._NF.Solar.Components;

/// <summary>
///     This is a solar panel.
///     It updates with grid-based tracking information.
///     It generates power from the sun based on coverage.
///     Largely based on Space Station 14's SolarPanelComponent.
/// </summary>
[RegisterComponent]
[Access(typeof(NFPowerSolarSystem))]
public sealed partial class NFSolarPanelComponent : Component
{
    /// <summary>
    /// Maximum supply output by this panel (coverage = 1)
    /// </summary>
    [DataField]
    public int MaxSupply = 1500;

    // Mono
    /// <summary>
    /// Coverage to still have if there's a wall directly in front of us.
    /// </summary>
    [DataField]
    public float ObstructedCoverage = 0.5f;

    /// <summary>
    /// Current coverage of this panel (from 0 to 1).
    /// This is updated by <see cref='PowerSolarSystem'/>.
    /// DO NOT WRITE WITHOUT CALLING UpdateSupply()!
    /// </summary>
    [ViewVariables]
    public float Coverage { get; set; } = 0;
}
