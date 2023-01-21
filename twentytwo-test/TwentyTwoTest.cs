namespace twentytwo_test;

using TwentyTwo;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public async Task TestMethod1()
    {
        var twentyTwo = new TwentyTwo();
        await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);
    }

    [TestMethod]

    public async Task TestCalcCubeMapSize()
    {
        var twentyTwo = new TwentyTwo();

        var smallPuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);
        int smallSize = smallPuzzle.FlatMap.CalculateCubeMapSize();

        var largePuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.DataFilePath);
        int largeSize = largePuzzle.FlatMap.CalculateCubeMapSize();

        Assert.AreEqual(4, smallSize);
        Assert.AreEqual(50, largeSize);
    }
}