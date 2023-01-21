namespace TwentyTwo;

public class TwentyTwo
{
    public string TestDataFilePath
    {
        get
        {
            return //System.IO.File.ReadAllLines(
                @"data/day-22-input-test.txt";
            // );
        }
    }
    public async Task<FlatMap> LoadFlatMapFromFilePath(string filePath)
    {

        var lines = File.ReadLinesAsync(filePath);
        await foreach (var line in lines)
        {

            Console.WriteLine(line);
        }

        throw new NotImplementedException();
    }
}
