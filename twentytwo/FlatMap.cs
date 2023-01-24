namespace TwentyTwo;

public class FlatMap
{

    private readonly Dictionary<int, Dictionary<int, MapSection>> map = new Dictionary<int, Dictionary<int, MapSection>>();

    public Dictionary<int, MapSection>? FetchLine(int lineNumber)
    {

        if (map.TryGetValue(lineNumber, out Dictionary<int, MapSection>? line))
        {
            return line;
        }
        return null;
    }

    public FlatMap InsertLine(int y, Dictionary<int, MapSection> mapLine)
    {
        map.Add(y, mapLine);
        return this;
    }

    public MapSection? FetchTile(int x, int y)
    {
        var line = FetchLine(y);
        if (line != null && line.TryGetValue(x, out MapSection value))
        {
            return value;
        }
        return null;
    }

    public int CalculateCubeMapSize()
    {

        // calculate area of a single face
        int area = map.Select(l => l.Value.Count).Sum() / 6;

        // integer square root we have at home
        foreach (int i in Enumerable.Range(1, 4097))
        {
            if (i * i == area)
            {
                return i;
            }
        }
        throw new Exception("Unable to find a integer square area for map.");
    }
}
