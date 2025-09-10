// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 InsanityMoose
// SPDX-FileCopyrightText: 2024 Dvir
// SPDX-FileCopyrightText: 2024 Jake Huxell
// SPDX-FileCopyrightText: 2024 Whatstone
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 Ilya246
//
// SPDX-License-Identifier: MPL-2.0

using Content.Shared.Shuttles.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Shuttles.Components;

/// <summary>
/// Handles what a grid should look like on radar.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedShuttleSystem))]
public sealed partial class IFFComponent : Component
{
    public static readonly Color SelfColor = Color.MediumSpringGreen;

    /// <summary>
    /// Default color to use for IFF if no component is found.
    /// </summary>
    public static readonly Color IFFColor = Color.Gold;

    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public IFFFlags Flags = IFFFlags.None;

    /// <summary>
    /// Color for this to show up on IFF.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public Color Color = IFFColor;

    // Frontier: POI IFF protection
    /// <summary>
    /// Whether or not this entity's IFF can be changed.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField(serverOnly: true)]
    public bool ReadOnly;
    // End Frontier
}

[Flags]
public enum IFFFlags : byte
{
    None = 0,

    /// <summary>
    /// Should the label for this grid be hidden at all ranges.
    /// Mono - only hides true IFF instead.
    /// </summary>
    HideLabel = 1,

    /// <summary>
    /// Should the grid hide entirely (AKA full stealth).
    /// Will also hide the label if that is not set.
    /// </summary>
    Hide = 2,

    /// <summary>
    /// Frontier - Is this a player shuttle
    /// </summary>
    IsPlayerShuttle = 4,

    // Mono
    /// <summary>
    /// If HideLabel is true, whether even detection labels should not be shown.
    /// </summary>
    HideLabelAlways = 8,

    // TODO: Need one that hides its outline, just replace it with a bunch of triangles or lines or something.
}
