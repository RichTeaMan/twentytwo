namespace TwentyTwo;

public static class CubeConsts
{
    public const int NORTH_INDEX = 0;
    public const int EAST_INDEX = 1;
    public const int SOUTH_INDEX = 2;
    public const int WEST_INDEX = 3;
}

public enum Orientation
{
    SAME,
    ONE_CLOCKWISE,
    TWO_CLOCKWISE,
    THREE_CLOCKWISE
}

public class CubeFaceConnection
{

    public int CubeFaceId { get; set; }

    public Orientation Orientation { get; set; }

}
public class CubeFace
{
    public int Id { get; }

    //public Dictionary<Point, MapSection> MapSections = new Dictionary<Point, MapSection>();

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

}

public class CubeMap
{

    public CubeFace[] Faces { get; }

    public int ConnectionCount
    {
        get
        {
            return Faces.SelectMany(f => f?.Connections ?? new CubeFaceConnection[0]).Count(c => c != null);
        }
    }

    public CubeMap(IEnumerable<CubeFace> cubeFaces)
    {

        int cubeFaceCount = cubeFaces.Count(cf => cf != null);
        if (cubeFaceCount != 6)
        {
            throw new ArgumentException($"There must be exactly 6 non null cube faces. Found {cubeFaceCount}.");
        }

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
                    CubeFace? up_face_id_opt = faces.SingleOrDefault(f => f.X == x && f.Y == (y - size));
                    CubeFace? left_face_id_opt = faces.SingleOrDefault(f => f.X == (x - size) && f.Y == y);


                    CubeFace face = new CubeFace(faces.Count, x, y);
                    faces.Add(face);
                    if (up_face_id_opt != null)
                    {
                        CubeFaceConnection connect = new CubeFaceConnection
                        {
                            CubeFaceId = up_face_id_opt.Id,
                            Orientation = Orientation.SAME
                        };
                        face.Connections[CubeConsts.NORTH_INDEX] = connect;



                        CubeFaceConnection partnerConnect = new CubeFaceConnection
                        {
                            CubeFaceId = face.Id,
                            Orientation = Orientation.SAME
                        };
                        up_face_id_opt.Connections[CubeConsts.SOUTH_INDEX] = partnerConnect;
                    }

                    if (left_face_id_opt != null)
                    {

                        CubeFaceConnection connect = new CubeFaceConnection
                        {
                            CubeFaceId = left_face_id_opt.Id,
                            Orientation = Orientation.SAME
                        };
                        face.Connections[CubeConsts.WEST_INDEX] = connect;



                        CubeFaceConnection partnerConnect = new CubeFaceConnection
                        {
                            CubeFaceId = face.Id,
                            Orientation = Orientation.SAME
                        };
                        left_face_id_opt.Connections[CubeConsts.EAST_INDEX] = partnerConnect;
                    }


                }
            }
        }

        CubeMap cubeMap = new CubeMap(faces);
        return cubeMap;
    }

}