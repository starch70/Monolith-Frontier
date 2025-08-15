// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2025 Redrover1760
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Client.Parallax.Data;

/// <summary>
/// Prototype data for a parallax.
/// </summary>
[Prototype]
public sealed partial class ParallaxPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Parallax layers.
    /// </summary>
    [DataField("layers")]
    public List<ParallaxLayerConfig> Layers { get; private set; } = new();

    /// <summary>
    /// Parallax layers, low-quality.
    /// </summary>
    [DataField("layersLQ")]
    public List<ParallaxLayerConfig> LayersLQ { get; private set; } = new();

    /// <summary>
    /// If low-quality layers don't exist for this parallax and high-quality should be used instead.
    /// </summary>
    [DataField("layersLQUseHQ")]
    public bool LayersLQUseHQ { get; private set; } = true;
}
