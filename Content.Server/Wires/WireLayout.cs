// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Redrover1760
// SPDX-FileCopyrightText: 2025 Tayrtahn
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Server.Wires;

/// <summary>
///     WireLayout prototype.
///
///     This is meant for ease of organizing wire sets on entities that use
///     wires. Once one of these is initialized, it should be stored in the
///     WiresSystem as a functional wire set.
/// </summary>
[Prototype]
public sealed partial class WireLayoutPrototype : IPrototype, IInheritingPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<WireLayoutPrototype>))]
    public string[]? Parents { get; private set; }

    [AbstractDataField]
    public bool Abstract { get; private set; }

    /// <summary>
    ///     How many wires in this layout will do
    ///     nothing (these are added upon layout
    ///     initialization)
    /// </summary>
    [DataField("dummyWires")]
    [NeverPushInheritance]
    public int DummyWires { get; private set; } = default!;

    /// <summary>
    ///     All the valid IWireActions currently in this layout.
    /// </summary>
    [DataField("wires")]
    [NeverPushInheritance]
    public List<IWireAction>? Wires { get; private set; }
}
