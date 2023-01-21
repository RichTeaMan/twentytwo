namespace TwentyTwo;

public class FlatMap
{

    private readonly Dictionary<int, Dictionary<int, MapSection>> map = new Dictionary<int, Dictionary<int, MapSection>>();

    public Dictionary<int, MapSection> FetchLine(int lineNumber)
    {

        return map[lineNumber];
    }
}
