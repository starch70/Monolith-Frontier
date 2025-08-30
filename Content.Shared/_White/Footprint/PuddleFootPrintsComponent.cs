// SPDX-FileCopyrightText: 2024 Aiden
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared._White.FootPrint;

[RegisterComponent]
public sealed partial class PuddleFootPrintsComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public float SizeRatio = 0.2f;

    [ViewVariables(VVAccess.ReadWrite)]
    public float OffPercent = 80f;
}
