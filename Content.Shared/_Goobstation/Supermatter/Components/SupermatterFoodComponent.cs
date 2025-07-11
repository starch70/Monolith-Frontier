namespace Content.Shared._Goobstation.Supermatter.Components;

[RegisterComponent]
public sealed partial class SupermatterFoodComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("energy")]
    public int Energy { get; set; } = 1;
}
