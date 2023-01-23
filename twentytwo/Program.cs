namespace TwentyTwo;

public class Program
{

    public static async Task Main()
    {
        Console.WriteLine("Advent of Code, Day 22.");

        var twentyTwo = new TwentyTwo();
        await twentyTwo.Navigate(twentyTwo.DataFilePath);
    }
}
