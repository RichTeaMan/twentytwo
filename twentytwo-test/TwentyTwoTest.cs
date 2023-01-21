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
    public void TestPointHash()
    {

        var p1 = new Point(0, 0);
        var p2 = new Point(1, 0);
        var p3 = new Point(0, 12);
        var p4 = new Point(100, 50);

        var p5 = new Point(0, 0);
        var p6 = new Point(1, 0);
        var p7 = new Point(0, 12);
        var p8 = new Point(100, -50);

        var hashSet = new HashSet<Point>();

        hashSet.Add(p1);
        hashSet.Add(p2);
        hashSet.Add(p3);
        hashSet.Add(p4);
        hashSet.Add(p5);
        hashSet.Add(p6);
        hashSet.Add(p7);
        hashSet.Add(p8);

        Assert.AreEqual(5, hashSet.Count);
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



    [TestMethod]
    public async Task TestUnconnectedCube()
    {

        var twentyTwo = new TwentyTwo();

        var smallPuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);
        var smallFlatMap = smallPuzzle.FlatMap;

        var graph = CubeMap.CreateUnconnected(smallFlatMap);

        Assert.AreEqual(6, graph.Faces.Count(f => f != null));
        Assert.AreEqual(10, graph.ConnectionCount);

        Assert.AreEqual(0, graph.Faces[0].Id);
        Assert.AreEqual(8, graph.Faces[0].X);
        Assert.AreEqual(0, graph.Faces[0].Y);
        Assert.AreEqual(1, graph.Faces[0].KnownConnectionsCount);
        Assert.AreEqual(
            3,
            graph.Faces[0].Connections[CubeConsts.SOUTH_INDEX].CubeFaceId
        );

        Assert.AreEqual(1, graph.Faces[1].Id);
        Assert.AreEqual(0, graph.Faces[1].X);
        Assert.AreEqual(4, graph.Faces[1].Y);
        Assert.AreEqual(2, graph.Faces[1].Connections[CubeConsts.EAST_INDEX].CubeFaceId);

        Assert.AreEqual(2, graph.Faces[2].Id);
        Assert.AreEqual(4, graph.Faces[2].X);
        Assert.AreEqual(4, graph.Faces[2].Y);
        Assert.AreEqual(3, graph.Faces[2].Connections[CubeConsts.EAST_INDEX].CubeFaceId);
        Assert.AreEqual(1, graph.Faces[2].Connections[CubeConsts.WEST_INDEX].CubeFaceId);

        Assert.AreEqual(3, graph.Faces[3].Id);
        Assert.AreEqual(8, graph.Faces[3].X);
        Assert.AreEqual(4, graph.Faces[3].Y);
        Assert.AreEqual(
            0,
            graph.Faces[3].Connections[CubeConsts.NORTH_INDEX].CubeFaceId
        );
        Assert.AreEqual(2, graph.Faces[3].Connections[CubeConsts.WEST_INDEX].CubeFaceId);

        Assert.AreEqual(5, graph.Faces[5].Id);
        Assert.AreEqual(12, graph.Faces[5].X);
        Assert.AreEqual(8, graph.Faces[5].Y);
    }
}