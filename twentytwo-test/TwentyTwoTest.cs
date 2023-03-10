namespace twentytwo_test;

using TwentyTwo;

[TestClass]
public class TwentyTwoTest
{

    [TestMethod]
    public async Task TestMethod1()
    {
        var twentyTwo = new TwentyTwo();
        var flatMap = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.TestDataFilePath);

        // 10R5L5R10L4R5L5
        Assert.AreEqual(7, flatMap.Instructions.Length);
        Assert.AreEqual(new Instruction(10, Turn.NONE), flatMap.Instructions[0]);
        Assert.AreEqual(new Instruction(5, Turn.RIGHT), flatMap.Instructions[1]);
        Assert.AreEqual(new Instruction(5, Turn.LEFT), flatMap.Instructions[2]);
        Assert.AreEqual(new Instruction(10, Turn.RIGHT), flatMap.Instructions[3]);
        Assert.AreEqual(new Instruction(4, Turn.LEFT), flatMap.Instructions[4]);
        Assert.AreEqual(new Instruction(5, Turn.RIGHT), flatMap.Instructions[5]);
        Assert.AreEqual(new Instruction(5, Turn.LEFT), flatMap.Instructions[6]);
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

        var hashSet = new HashSet<Point>
        {
            p1,
            p2,
            p3,
            p4,
            p5,
            p6,
            p7,
            p8
        };

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

        Assert.AreEqual(4, graph.Size);
        Assert.AreEqual(6, graph.Faces.Count(f => f != null));
        Assert.AreEqual(24, graph.ConnectionCount);

        Assert.IsNotNull(graph.Faces[0]);
        Assert.AreEqual(0, graph.Faces[0].Id);
        Assert.AreEqual(8, graph.Faces[0].X);
        Assert.AreEqual(0, graph.Faces[0].Y);
        Assert.IsNotNull(graph.Faces[0].Connections[0]);
        Assert.AreEqual(1, graph.Faces[0].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[0].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[0].Connections[1]);
        Assert.AreEqual(5, graph.Faces[0].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[0].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[0].Connections[2]);
        Assert.AreEqual(3, graph.Faces[0].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[0].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[0].Connections[3]);
        Assert.AreEqual(2, graph.Faces[0].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[0].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[1]);
        Assert.AreEqual(1, graph.Faces[1].Id);
        Assert.AreEqual(0, graph.Faces[1].X);
        Assert.AreEqual(4, graph.Faces[1].Y);
        Assert.IsNotNull(graph.Faces[1].Connections[0]);
        Assert.AreEqual(0, graph.Faces[1].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[1].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[1].Connections[1]);
        Assert.AreEqual(2, graph.Faces[1].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[1].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[1].Connections[2]);
        Assert.AreEqual(4, graph.Faces[1].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[1].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[1].Connections[3]);
        Assert.AreEqual(5, graph.Faces[1].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[1].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[2]);
        Assert.AreEqual(2, graph.Faces[2].Id);
        Assert.AreEqual(4, graph.Faces[2].X);
        Assert.AreEqual(4, graph.Faces[2].Y);
        Assert.IsNotNull(graph.Faces[2].Connections[0]);
        Assert.AreEqual(0, graph.Faces[2].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[2].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[2].Connections[1]);
        Assert.AreEqual(3, graph.Faces[2].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[2].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[2].Connections[2]);
        Assert.AreEqual(4, graph.Faces[2].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[2].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[2].Connections[3]);
        Assert.AreEqual(1, graph.Faces[2].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[2].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[3]);
        Assert.AreEqual(3, graph.Faces[3].Id);
        Assert.AreEqual(8, graph.Faces[3].X);
        Assert.AreEqual(4, graph.Faces[3].Y);
        Assert.IsNotNull(graph.Faces[3].Connections[0]);
        Assert.AreEqual(0, graph.Faces[3].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[3].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[3].Connections[1]);
        Assert.AreEqual(5, graph.Faces[3].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[3].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[3].Connections[2]);
        Assert.AreEqual(4, graph.Faces[3].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[3].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[3].Connections[3]);
        Assert.AreEqual(2, graph.Faces[3].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[3].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[4]);
        Assert.AreEqual(4, graph.Faces[4].Id);
        Assert.AreEqual(8, graph.Faces[4].X);
        Assert.AreEqual(8, graph.Faces[4].Y);
        Assert.IsNotNull(graph.Faces[4].Connections[0]);
        Assert.AreEqual(3, graph.Faces[4].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[4].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[4].Connections[1]);
        Assert.AreEqual(5, graph.Faces[4].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[4].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[4].Connections[2]);
        Assert.AreEqual(1, graph.Faces[4].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[4].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[4].Connections[3]);
        Assert.AreEqual(2, graph.Faces[4].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[4].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[5]);
        Assert.AreEqual(5, graph.Faces[5].Id);
        Assert.AreEqual(12, graph.Faces[5].X);
        Assert.AreEqual(8, graph.Faces[5].Y);
        Assert.IsNotNull(graph.Faces[5].Connections[0]);
        Assert.AreEqual(3, graph.Faces[5].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[5].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[5].Connections[1]);
        Assert.AreEqual(0, graph.Faces[5].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[5].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[5].Connections[2]);
        Assert.AreEqual(1, graph.Faces[5].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[5].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[5].Connections[3]);
        Assert.AreEqual(4, graph.Faces[5].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[5].Connections[3].Orientation);
    }

    [TestMethod]
    public async Task TestLargeConnectedCube()
    {

        var twentyTwo = new TwentyTwo();

        var largePuzzle = await twentyTwo.LoadFlatMapFromFilePath(twentyTwo.DataFilePath);
        var largeFlatMap = largePuzzle.FlatMap;

        var graph = CubeMap.CreateUnconnected(largeFlatMap);
        graph.ConnectEdges();

        Assert.AreEqual(50, graph.Size);
        Assert.AreEqual(6, graph.Faces.Count(f => f != null));
        Assert.AreEqual(24, graph.ConnectionCount);

        Assert.IsNotNull(graph.Faces[0]);
        Assert.AreEqual(0, graph.Faces[0].Id);
        Assert.AreEqual(50, graph.Faces[0].X);
        Assert.AreEqual(0, graph.Faces[0].Y);
        Assert.IsNotNull(graph.Faces[0].Connections[0]);
        Assert.AreEqual(5, graph.Faces[0].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[0].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[0].Connections[1]);
        Assert.AreEqual(1, graph.Faces[0].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[0].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[0].Connections[2]);
        Assert.AreEqual(2, graph.Faces[0].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[0].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[0].Connections[3]);
        Assert.AreEqual(3, graph.Faces[0].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[0].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[1]);
        Assert.AreEqual(1, graph.Faces[1].Id);
        Assert.AreEqual(100, graph.Faces[1].X);
        Assert.AreEqual(0, graph.Faces[1].Y);
        Assert.IsNotNull(graph.Faces[1].Connections[0]);
        Assert.AreEqual(5, graph.Faces[1].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[1].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[1].Connections[1]);
        Assert.AreEqual(4, graph.Faces[1].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[1].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[1].Connections[2]);
        Assert.AreEqual(2, graph.Faces[1].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[1].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[1].Connections[3]);
        Assert.AreEqual(0, graph.Faces[1].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[1].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[2]);
        Assert.AreEqual(2, graph.Faces[2].Id);
        Assert.AreEqual(50, graph.Faces[2].X);
        Assert.AreEqual(50, graph.Faces[2].Y);
        Assert.IsNotNull(graph.Faces[2].Connections[0]);
        Assert.AreEqual(0, graph.Faces[2].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[2].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[2].Connections[1]);
        Assert.AreEqual(1, graph.Faces[2].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[2].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[2].Connections[2]);
        Assert.AreEqual(4, graph.Faces[2].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[2].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[2].Connections[3]);
        Assert.AreEqual(3, graph.Faces[2].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[2].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[3]);
        Assert.AreEqual(3, graph.Faces[3].Id);
        Assert.AreEqual(0, graph.Faces[3].X);
        Assert.AreEqual(100, graph.Faces[3].Y);
        Assert.IsNotNull(graph.Faces[3].Connections[0]);
        Assert.AreEqual(2, graph.Faces[3].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[3].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[3].Connections[1]);
        Assert.AreEqual(4, graph.Faces[3].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[3].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[3].Connections[2]);
        Assert.AreEqual(5, graph.Faces[3].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[3].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[3].Connections[3]);
        Assert.AreEqual(0, graph.Faces[3].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[3].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[4]);
        Assert.AreEqual(4, graph.Faces[4].Id);
        Assert.AreEqual(50, graph.Faces[4].X);
        Assert.AreEqual(100, graph.Faces[4].Y);
        Assert.IsNotNull(graph.Faces[4].Connections[0]);
        Assert.AreEqual(2, graph.Faces[4].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[4].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[4].Connections[1]);
        Assert.AreEqual(1, graph.Faces[4].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.TWO_CLOCKWISE, graph.Faces[4].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[4].Connections[2]);
        Assert.AreEqual(5, graph.Faces[4].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.THREE_CLOCKWISE, graph.Faces[4].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[4].Connections[3]);
        Assert.AreEqual(3, graph.Faces[4].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[4].Connections[3].Orientation);

        Assert.IsNotNull(graph.Faces[5]);
        Assert.AreEqual(5, graph.Faces[5].Id);
        Assert.AreEqual(0, graph.Faces[5].X);
        Assert.AreEqual(150, graph.Faces[5].Y);
        Assert.IsNotNull(graph.Faces[5].Connections[0]);
        Assert.AreEqual(3, graph.Faces[5].Connections[0].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[5].Connections[0].Orientation);
        Assert.IsNotNull(graph.Faces[5].Connections[1]);
        Assert.AreEqual(4, graph.Faces[5].Connections[1].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[5].Connections[1].Orientation);
        Assert.IsNotNull(graph.Faces[5].Connections[2]);
        Assert.AreEqual(1, graph.Faces[5].Connections[2].CubeFaceId);
        Assert.AreEqual(Orientation.SAME, graph.Faces[5].Connections[2].Orientation);
        Assert.IsNotNull(graph.Faces[5].Connections[3]);
        Assert.AreEqual(0, graph.Faces[5].Connections[3].CubeFaceId);
        Assert.AreEqual(Orientation.ONE_CLOCKWISE, graph.Faces[5].Connections[3].Orientation);
    }

    [TestMethod]
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

    [TestMethod]
    public async Task TestCubeNavigate()
    {
        var twentyTwo = new TwentyTwo();
        int score = await twentyTwo.Navigate(twentyTwo.TestDataFilePath);

        Assert.AreEqual(5031, score);
    }

    [TestMethod]
    public async Task TestLargeCubeNavigate()
    {
        var twentyTwo = new TwentyTwo();
        int score = await twentyTwo.Navigate(twentyTwo.DataFilePath);

        Assert.AreEqual(189097, score);
    }
}
