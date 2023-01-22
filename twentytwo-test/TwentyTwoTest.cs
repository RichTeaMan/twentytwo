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
    public void TestNormalDirection()
    {

        var n = CubeConsts.NORTH_INDEX.NormalDirection();
        Assert.AreEqual(CubeConsts.WEST_INDEX, n.Item1);
        Assert.AreEqual(CubeConsts.EAST_INDEX, n.Item2);

        var s = CubeConsts.SOUTH_INDEX.NormalDirection();
        Assert.AreEqual(CubeConsts.WEST_INDEX, s.Item1);
        Assert.AreEqual(CubeConsts.EAST_INDEX, s.Item2);

        var e = CubeConsts.EAST_INDEX.NormalDirection();
        Assert.AreEqual(CubeConsts.NORTH_INDEX, e.Item1);
        Assert.AreEqual(CubeConsts.SOUTH_INDEX, e.Item2);

        var w = CubeConsts.WEST_INDEX.NormalDirection();
        Assert.AreEqual(CubeConsts.NORTH_INDEX, w.Item1);
        Assert.AreEqual(CubeConsts.SOUTH_INDEX, w.Item2);
    }

    [TestMethod]
    public void TestIsHorizontalDirection()
    {
        Assert.AreEqual(false, CubeConsts.NORTH_INDEX.IsHorizontal());
        Assert.AreEqual(false, CubeConsts.SOUTH_INDEX.IsHorizontal());
        Assert.AreEqual(true, CubeConsts.EAST_INDEX.IsHorizontal());
        Assert.AreEqual(true, CubeConsts.WEST_INDEX.IsHorizontal());
    }

    [TestMethod]
    public void TestOppositeDirection()
    {
        Assert.AreEqual(CubeConsts.SOUTH_INDEX, CubeConsts.NORTH_INDEX.OppositeDirection());
        Assert.AreEqual(CubeConsts.NORTH_INDEX, CubeConsts.SOUTH_INDEX.OppositeDirection());
        Assert.AreEqual(CubeConsts.WEST_INDEX, CubeConsts.EAST_INDEX.OppositeDirection());
        Assert.AreEqual(CubeConsts.EAST_INDEX, CubeConsts.WEST_INDEX.OppositeDirection());
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
    public void TestOrientationCombine()
    {
        Assert.AreEqual(Orientation.SAME, Orientation.SAME.Combine(Orientation.SAME));
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, Orientation.SAME.Combine(Orientation.ONE_CLOCKWISE));
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, Orientation.ONE_CLOCKWISE.Combine(Orientation.SAME));

        Assert.AreEqual(Orientation.SAME, Orientation.ONE_CLOCKWISE.Combine(Orientation.THREE_CLOCKWISE));

        Assert.AreEqual(Orientation.TWO_CLOCKWISE, Orientation.THREE_CLOCKWISE.Combine(Orientation.THREE_CLOCKWISE));
    }

    [TestMethod]
    public void TestDirectionOrientationResolve()
    {

        Assert.AreEqual(CubeConsts.NORTH_INDEX, Orientation.SAME.Resolve(CubeConsts.NORTH_INDEX));
        Assert.AreEqual(CubeConsts.EAST_INDEX, Orientation.ONE_CLOCKWISE.Resolve(CubeConsts.NORTH_INDEX));
        Assert.AreEqual(CubeConsts.NORTH_INDEX, Orientation.TWO_CLOCKWISE.Resolve(CubeConsts.SOUTH_INDEX));
        Assert.AreEqual(CubeConsts.EAST_INDEX, Orientation.THREE_CLOCKWISE.Resolve(CubeConsts.SOUTH_INDEX));
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

    [TestMethod]
    public async Task TestConnectedCube()
    {

        var twentyTwo = new TwentyTwo();

        var smallPuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);
        var smallFlatMap = smallPuzzle.FlatMap;

        var graph = CubeMap.CreateUnconnected(smallFlatMap);
        graph.ConnectEdges();

        Assert.AreEqual(6, graph.Faces.Count(f => f != null));
        Assert.AreEqual(24, graph.ConnectionCount);
    }

    //[TestMethod]
    public async Task TestConnectedCubeOneIteration()
    {

        var twentyTwo = new TwentyTwo();

        var smallPuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);
        var smallFlatMap = smallPuzzle.FlatMap;

        var graph = CubeMap.CreateUnconnected(smallFlatMap);
        graph.ConnectEdges(1);

        Assert.AreEqual(6, graph.Faces.Count(f => f != null));
        Assert.AreEqual(15, graph.ConnectionCount); // maybe correct?

        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[0].Connections[CubeConsts.WEST_INDEX].Orientation);

        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[3].Connections[CubeConsts.EAST_INDEX].Orientation);
    }
}
