namespace TwentyTwo;

public class Program
{

    public static async Task Main()
    {
        Console.WriteLine("Hello world.");

        var twentyTwo = new TwentyTwo();

        var smallPuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);
        var smallFlatMap = smallPuzzle.FlatMap;

        var graph = CubeMap.CreateUnconnected(smallFlatMap);
        graph.ConnectEdges();

        Console.WriteLine("a cube?");
        foreach(var face in graph.Faces) {
            Console.WriteLine(face.FetchDebugString());
            Console.WriteLine();
        }
    }
}
