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
}