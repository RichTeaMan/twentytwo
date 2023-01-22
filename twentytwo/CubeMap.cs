using System.Diagnostics;

namespace TwentyTwo;

public static class CubeConsts
{
    public const int NORTH_INDEX = 0;
    public const int EAST_INDEX = 1;
    public const int SOUTH_INDEX = 2;
    public const int WEST_INDEX = 3;

    public static int WrapDirection(this int direction)
    {
        return direction % 4;
    }

    public static bool IsHorizontal(this int direction)
    {
        return direction % 2 != 0;
    }

    public static (int, int) NormalDirection(this int direction)
    {

        switch (direction.WrapDirection())
        {
            case NORTH_INDEX:
            case SOUTH_INDEX:
                return (WEST_INDEX, EAST_INDEX);
            case EAST_INDEX:
            case WEST_INDEX:
                return (NORTH_INDEX, SOUTH_INDEX);
            default: throw new ArgumentException($"Unknown direction: {direction}");
        }
    }

    public static int OppositeDirection(this int direction)
    {
        return (direction + 2) % 4;
    }
}

public enum Orientation
{
    SAME,
    ONE_CLOCKWISE,
    TWO_CLOCKWISE,
    THREE_CLOCKWISE
}

public static class OrientationUtils
{
    public static Orientation Combine(this Orientation a, Orientation b)
    {
        int combined = (a.OrientationAsNumber() + b.OrientationAsNumber()) % 4;

        switch (combined)
        {
            case 0: return Orientation.SAME;
            case 1: return Orientation.ONE_CLOCKWISE;
            case 2: return Orientation.TWO_CLOCKWISE;
            case 3: return Orientation.THREE_CLOCKWISE;
        }
        throw new Exception($"Bad orientation number: {combined}.");
    }

    public static int OrientationAsNumber(this Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.SAME: return 0;
            case Orientation.ONE_CLOCKWISE: return 1;
            case Orientation.TWO_CLOCKWISE: return 2;
            case Orientation.THREE_CLOCKWISE: return 3;
        }
        throw new Exception("Unknown orientation.");
    }

    public static int Resolve(this Orientation orientation, int direction)
    {

        return (orientation.OrientationAsNumber() + direction) % 4;
    }
}

public class CubeFaceConnection
{

    public int CubeFaceId { get; set; }

    public Orientation Orientation { get; set; }

}
public class CubeFace
{
    public int Id { get; }

    public int X { get; }

    public int Y { get; }

    public CubeFaceConnection[] Connections { get; } = new CubeFaceConnection[4];

    public int KnownConnectionsCount
    {
        get
        {
            return Connections.Count(c => c != null);
        }
    }

    public CubeFace(int id, int x, int y)
    {
        Id = id;
        X = x;
        Y = y;
    }

    public string FetchDebugString()
    {
        string debug = $"CubeFace ID: {Id} X: {X} Y: {Y}";
        for (int i = 0; i < Connections.Length; i++)
        {
            var connection = Connections[i];
            if (connection == null)
            {
                Console.WriteLine($"\n    {i}: null");
            }
            else
            {
                debug += $"\n    {i}: {connection.CubeFaceId}, {connection.Orientation}";
            }
        }
        return debug;
    }

    public string GenerateAssertion()
    {

        List<string> assertions = new List<string>();
        assertions.Add($"Assert.IsNotNull(graph.Faces[{Id}]);");
        assertions.Add($"Assert.AreEqual({Id}, graph.Faces[{Id}].Id);");
        assertions.Add($"Assert.AreEqual({X}, graph.Faces[{Id}].X);");
        assertions.Add($"Assert.AreEqual({Y}, graph.Faces[{Id}].Y);");

        for (int i = 0; i < Connections.Length; i++)
        {
            var connection = Connections[i];
            if (connection == null)
            {
                throw new Exception("Unconnected face, cannot create assertion.");
            }
            assertions.Add($"Assert.IsNotNull(graph.Faces[{Id}].Connections[{i}]);");
            assertions.Add($"Assert.AreEqual({connection.CubeFaceId}, graph.Faces[{Id}].Connections[{i}].CubeFaceId);");
            assertions.Add($"Assert.AreEqual(Orientation.{connection.Orientation}, graph.Faces[{Id}].Connections[{i}].Orientation);");

        }
        return string.Join("\n", assertions);
    }

}

public class CubeMap
{

    public CubeFace[] Faces { get; }

    public int Size { get; }

    public int ConnectionCount
    {
        get
        {
            return Faces.SelectMany(f => f?.Connections ?? new CubeFaceConnection[0]).Count(c => c != null);
        }
    }

    public CubeMap(int size, IEnumerable<CubeFace> cubeFaces)
    {

        int cubeFaceCount = cubeFaces.Count(cf => cf != null);
        if (cubeFaceCount != 6)
        {
            throw new ArgumentException($"There must be exactly 6 non null cube faces. Found {cubeFaceCount}.");
        }

        Size = size;
        Faces = cubeFaces.Where(f => f != null).ToArray();
    }

    public static CubeMap CreateUnconnected(FlatMap flatMap)
    {

        int size = flatMap.CalculateCubeMapSize();
        var faces = new List<CubeFace>();

        foreach (int faceRow in Enumerable.Range(0, 6))
        {
            foreach (int faceColumn in Enumerable.Range(0, 6))
            {

                int x = faceColumn * size;
                int y = faceRow * size;

                if (flatMap.FetchTile(x, y) != null)
                {
                    // check if another face already touches this one
                    // a face can only possibly be left or up
                    CubeFace? upFace = faces.SingleOrDefault(f => f.X == x && f.Y == (y - size));
                    CubeFace? leftFace = faces.SingleOrDefault(f => f.X == (x - size) && f.Y == y);

                    CubeFace face = new CubeFace(faces.Count, x, y);
                    faces.Add(face);
                    if (upFace != null)
                    {
                        CubeFaceConnection connect = new CubeFaceConnection
                        {
                            CubeFaceId = upFace.Id,
                            Orientation = Orientation.SAME
                        };
                        face.Connections[CubeConsts.NORTH_INDEX] = connect;

                        CubeFaceConnection partnerConnect = new CubeFaceConnection
                        {
                            CubeFaceId = face.Id,
                            Orientation = Orientation.SAME
                        };
                        upFace.Connections[CubeConsts.SOUTH_INDEX] = partnerConnect;
                    }

                    if (leftFace != null)
                    {
                        CubeFaceConnection connect = new CubeFaceConnection
                        {
                            CubeFaceId = leftFace.Id,
                            Orientation = Orientation.SAME
                        };
                        face.Connections[CubeConsts.WEST_INDEX] = connect;

                        CubeFaceConnection partnerConnect = new CubeFaceConnection
                        {
                            CubeFaceId = face.Id,
                            Orientation = Orientation.SAME
                        };
                        leftFace.Connections[CubeConsts.EAST_INDEX] = partnerConnect;
                    }
                }
            }
        }

        CubeMap cubeMap = new CubeMap(size, faces);
        return cubeMap;
    }

    public CubeFace? FetchFaceAtLocation(int x, int y)
    {

        return Faces.SingleOrDefault(f => f.X == f.Y);
    }

    public void ConnectEdges(int maxIterations = int.MaxValue)
    {
        int loopCount = 0;
        while (ConnectionCount < 12 * 2 && loopCount < maxIterations)
        {
            int oldConnectionCount = ConnectionCount;

            foreach (var face in Faces)
            {
                Debug.Assert(face.KnownConnectionsCount > 0);
                if (face.KnownConnectionsCount == 4)
                {
                    continue;
                }

                List<Candiate> candidates = new List<Candiate>();
                foreach (var directionIndex in Enumerable.Range(0, 4).Where(i => face.Connections[i] == null))
                {
                    // look for faces next to the missing direction
                    var candiateDirection = directionIndex.NormalDirection();

                    // are either tiles linked?
                    var linkedDirections = face.Connections.Where((c, i) => c != null && (i == candiateDirection.Item1 || i == candiateDirection.Item2)).ToArray();

                    foreach (var linked in linkedDirections)
                    {
                        var via = Faces[linked.CubeFaceId];

                        int resolvedIndex = directionIndex;
                        if (linked.Orientation == Orientation.TWO_CLOCKWISE)
                        {
                            resolvedIndex = linked.Orientation.Resolve(directionIndex);
                        }
                        else if (linked.Orientation != Orientation.SAME)
                        {
                            resolvedIndex = linked.Orientation.Resolve(directionIndex).OppositeDirection();
                        }

                        var target = via.Connections[resolvedIndex];
                        if (target != null && face.Id != target.CubeFaceId)
                        {
                            candidates.Add(new Candiate(face, via, linked, target, Faces[target.CubeFaceId], directionIndex));
                        }
                    }
                }
                foreach (var c in candidates)
                {
                    var ids = new[] { c.Source.Id, c.Via.Id, c.Target.Id };
                    Debug.Assert(ids.Distinct().Count() == 3);
                }

                if (!candidates.Any())
                {
                    continue;
                }

                var candidate = candidates.First();

                Debug.Assert(candidate.Source.Connections[candidate.TargetDirection] == null);
                Debug.Assert(!candidate.Source.Connections.Any(c => c?.CubeFaceId == candidate.Target.Id));
                var connection = new CubeFaceConnection { CubeFaceId = candidate.Target.Id, Orientation = candidate.FetchRotation().Combine(candidate.SourceConnection.Orientation).Combine(candidate.TargetConnection.Orientation) };
                candidate.Source.Connections[candidate.TargetDirection] = connection;

                Console.WriteLine($"Connection -> Source {candidate.Source.Id}, Target {candidate.Target.Id}, Direction {candidate.TargetDirection}, Rotation {connection.Orientation}");
                Console.WriteLine();
            }

            if (ConnectionCount == oldConnectionCount)
            {
                throw new Exception("No new connections.");
            }
            loopCount++;
        }
    }

}

public record Candiate(CubeFace Source, CubeFace Via, CubeFaceConnection SourceConnection, CubeFaceConnection TargetConnection, CubeFace Target, int TargetDirection)
{
    public Orientation FetchRotation()
    {
        int dx = 0;
        int dy = 0;

        int targetDirection = Via.Connections.Select((e, i) => new { index = i, entity = e }).Where(c => c.entity?.CubeFaceId == Target.Id).Select(e => e.index).Single();
        int sourceDirection = Source.Connections.Select((e, i) => new { index = i, entity = e }).Where(c => c.entity?.CubeFaceId == Via.Id).Select(e => e.index).Single();

        if (SourceConnection.Orientation != Orientation.SAME && TargetConnection.Orientation != Orientation.SAME)
        {
            Console.WriteLine("    Double orientation");
        }

        int resolvedTargetDirection = SourceConnection.Orientation.Resolve(targetDirection);
        int resolvedSourceDirection = sourceDirection;

        Console.WriteLine($"    Fetch rotation: Source ID {Source.Id}, Target ID {Target.Id}, Via ID {Via.Id}. TD {targetDirection} RTD {resolvedTargetDirection} | SD {sourceDirection} RSD {resolvedSourceDirection}");

        var directions = new[] { resolvedTargetDirection, resolvedSourceDirection };

        foreach (var direction in directions)
        {

            switch (direction)
            {
                case CubeConsts.NORTH_INDEX:
                    dy--;
                    break;
                case CubeConsts.SOUTH_INDEX:
                    dy++;
                    break;
                case CubeConsts.EAST_INDEX:
                    dx++;
                    break;
                case CubeConsts.WEST_INDEX:
                    dx--;
                    break;
            }
        }

        if (dx == 0 || dx < -1)
        {
            Console.WriteLine("finding rotation error.");
            Console.WriteLine($"dX: {dx}");
            Console.WriteLine($"dY: {dy}");
        }

        if (true)
        {
            Console.WriteLine($"    dX: {dx}");
            Console.WriteLine($"    dY: {dy}");
        }

        Debug.Assert(dx == -1 || dx == 1, $"dX: {dx}");
        Debug.Assert(dy == -1 || dy == 1, $"dY: {dy}");

        Orientation orientation = Orientation.ONE_CLOCKWISE;
        if ((dx == -1 && dy == 1) || (dx == 1 && dy == -1))
        {
            orientation = Orientation.THREE_CLOCKWISE;
        }

        if (TargetDirection.IsHorizontal())
        {
            if (orientation == Orientation.ONE_CLOCKWISE)
            {
                orientation = Orientation.THREE_CLOCKWISE;
            }
            else
            {
                orientation = Orientation.ONE_CLOCKWISE;
            }
        }
        Console.WriteLine($"    {orientation}");
        return orientation;
    }
}
