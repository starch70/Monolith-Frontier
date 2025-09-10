// SPDX-FileCopyrightText: 2025 Ark
// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 ark1368
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Mono.Radar;

using Content.Shared._Mono.Radar;

/// <summary>
/// These handle objects which should be represented by radar blips.
/// </summary>
[RegisterComponent]
public sealed partial class RadarBlipComponent : Component
{
    /// <summary>
    /// Color that gets shown on the radar screen.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("radarColor")]
    public Color RadarColor = Color.Red;

    /// <summary>
    /// Color that gets shown on the radar screen when the blip is highlighted.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("highlightedRadarColor")]
    public Color HighlightedRadarColor = Color.OrangeRed;

    /// <summary>
    /// Scale of the blip.
    /// </summary>
    [DataField]
    public float Scale = 1;

    /// <summary>
    /// The shape of the blip on the radar.
    /// </summary>
    [DataField]
    public RadarBlipShape Shape = RadarBlipShape.Circle;

    /// <summary>
    /// Whether this blip should be shown even when parented to a grid.
    /// </summary>
    [DataField]
    public bool RequireNoGrid = false;

    /// <summary>
    /// Whether this blip should be visible on radar across different grids.
    /// </summary>
    [DataField]
    public bool VisibleFromOtherGrids = true;

    [DataField]
    public bool Enabled = true;

    /// <summary>
    /// Do not show the blip beyond this distance to the viewing mass scanner.
    /// </summary>
    [DataField]
    public float MaxDistance = 1024f;
}
