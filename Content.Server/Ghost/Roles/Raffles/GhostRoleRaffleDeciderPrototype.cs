// SPDX-FileCopyrightText: 2024 no
// SPDX-FileCopyrightText: 2025 Redrover1760
// SPDX-FileCopyrightText: 2025 Tayrtahn
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Server.Ghost.Roles.Raffles;

/// <summary>
/// Allows getting a <see cref="IGhostRoleRaffleDecider"/> as prototype.
/// </summary>
[Prototype]
public sealed partial class GhostRoleRaffleDeciderPrototype : IPrototype
{
    /// <inheritdoc />
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The <see cref="IGhostRoleRaffleDecider"/> instance that chooses the winner of a raffle.
    /// </summary>
    [DataField("decider", required: true)]
    public IGhostRoleRaffleDecider Decider { get; private set; } = new RngGhostRoleRaffleDecider();
}
