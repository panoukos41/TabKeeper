namespace TabKeeper.Components;

[Flags]
public enum Anchor
{
    Bottom = 1 << 0,
    Top = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    Center = 1 << 4
}
