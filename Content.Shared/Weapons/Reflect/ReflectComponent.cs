// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2024 Aviu00
// SPDX-FileCopyrightText: 2024 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2024 Whatstone
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 Redrover1760
// SPDX-FileCopyrightText: 2025 beck-thompson
// SPDX-FileCopyrightText: 2025 bitcrushing
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Reflect;

/// <summary>
/// Entities with this component have a chance to reflect projectiles and hitscan shots
/// Uses <c>ItemToggleComponent</c> to control reflection.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ReflectComponent : Component
{
    /// <summary>
    /// What we reflect.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("reflects")]
    public ReflectType Reflects = ReflectType.Energy | ReflectType.NonEnergy;

    /// <summary>
    /// Probability for a projectile to be reflected.
    /// </summary>
    [DataField("reflectProb"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public float ReflectProb = 0.25f;

    [DataField("spread"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public Angle Spread = Angle.FromDegrees(45);

    [DataField("soundOnReflect")]
    public SoundSpecifier? SoundOnReflect = new SoundPathSpecifier("/Audio/Weapons/Guns/Hits/laser_sear_wall.ogg");

    // WD START
    [DataField, AutoNetworkedField]
    public float DamageOnReflectModifier;
    // WD END
}

/// <summary>
/// Used for both the projectiles being reflected and the entities reflecting. If there is ever overlap between the
/// reflection types, the projectile will be reflected.
/// </summary>
[Flags, Serializable, NetSerializable]
public enum ReflectType : byte
{
    None = 0,
    NonEnergy = 1 << 0,
    Energy = 1 << 1,
}
