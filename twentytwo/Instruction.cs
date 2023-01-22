namespace TwentyTwo;

public enum Turn
{
    LEFT,
    RIGHT,
    NONE
}

public record Instruction(int TilesToMove, Turn Turn)
{
}
