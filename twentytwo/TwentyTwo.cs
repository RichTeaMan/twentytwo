using System.Diagnostics;

namespace TwentyTwo;

public class TwentyTwo
{
    public string TestDataFilePath
    {
        get
        {
            return @"data/day-22-input-test.txt";
        }
    }

    public string DataFilePath
    {
        get
        {
            return @"data/day-22-input.txt";
        }
    }

    public async Task<PuzzleInput> LoadFlatMapFromFilePath(string filePath)
    {
        var map = new FlatMap();
        var instructions = new List<Instruction>();

        var lines = File.ReadLinesAsync(filePath);
        int y = 0;
        await foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.Contains('L') || line.Contains('R'))
            {

                // stupid hack to include L & R delimiters in the split. I don't care anymore.
                var directions = line.Replace("L", ",L").Replace("R", ",R").Split(',');
                foreach (var direction in directions)
                {
                    Console.WriteLine(direction);
                    Turn instructionTurn = Turn.NONE;
                    if (direction.Contains('L'))
                    {
                        instructionTurn = Turn.LEFT;
                    }
                    else if (direction.Contains('R'))
                    {
                        instructionTurn = Turn.RIGHT;
                    }
                    int moves = int.Parse(direction.Trim('L', 'R'));
                    instructions.Add(new Instruction(moves, instructionTurn));
                }

                continue;
            }

            var mapLine = new Dictionary<int, MapSection>();
            foreach (int x in Enumerable.Range(0, line.Length))
            {
                var c = line[x];
                MapSection? section = null;
                switch (c)
                {
                    case '.': section = MapSection.FLOOR; break;
                    case '#': section = MapSection.WALL; break;
                    case ' ': break;
                    default: throw new Exception($"Unknown section type: {c}.");
                }

                if (section != null)
                {
                    mapLine.Add(x, section.Value);
                }
            }

            map.InsertLine(y, mapLine);
            y++;
        }

        return new PuzzleInput(map, instructions.ToArray());

    }

    public async Task<int> Navigate(string filePath)
    {

        var puzzleInput = await LoadFlatMapFromFilePath(filePath);

        var flatMap = puzzleInput.FlatMap;
        var graph = CubeMap.CreateUnconnected(flatMap);
        graph.ConnectEdges();

        // start location is top left face, which is always the first
        int x = graph.Faces.First().X;
        int y = graph.Faces.First().Y;

        int direction = CubeConsts.EAST_INDEX;
        var face = graph.Faces.First();

        Console.WriteLine($"Starting at ({x}, {y})");

        foreach (var instruction in puzzleInput.Instructions)
        {
            if (instruction.Turn == Turn.LEFT)
            {
                direction--;
            }
            else if (instruction.Turn == Turn.RIGHT)
            {
                direction++;
            }
            direction = (direction + 4) % 4;
            Point? heading = CalcHeading(direction);

            Console.WriteLine($"Instruction {instruction.Turn}{instruction.TilesToMove}");

            foreach (var _ in Enumerable.Range(0, instruction.TilesToMove))
            {
                int newX = x + heading.x;
                int newY = y + heading.y;

                var newFace = face;
                var newDirection = direction;
                var newHeading = heading;

                if (face != graph.FetchFaceAtLocation(newX, newY))
                {
                    // just moved face. hold onto your butts
                    // FetchFaceAtLocation is not reliable until coords have been resolved

                    var connection = face.Connections[direction];
                    newFace = graph.Faces[connection.CubeFaceId];

                    int preRotX = newX - face.X;
                    int preRotY = newY - face.Y;

                    switch (direction)
                    {
                        case CubeConsts.NORTH_INDEX:
                            preRotY = graph.Size - 1;
                            break;
                        case CubeConsts.SOUTH_INDEX:
                            preRotY = 0;
                            break;
                        case CubeConsts.WEST_INDEX:
                            preRotX = graph.Size - 1;
                            break;
                        case CubeConsts.EAST_INDEX:
                            preRotX = 0;
                            break;
                    }
                    Debug.Assert(preRotX >= 0 && preRotX < graph.Size);
                    Debug.Assert(preRotY >= 0 && preRotY < graph.Size);

                    int rotX = preRotX;
                    int rotY = preRotY;

                    // now do a rotation
                    foreach (var _i in Enumerable.Range(0, (4 - connection.Orientation.OrientationAsNumber()) % 4))
                    {
                        int tx = rotX;
                        int ty = rotY;

                        rotX = (graph.Size  - 1) - ty;
                        rotY = tx;
                    }


                    Console.WriteLine($"Rot: ({rotX}, {rotY})");

                    Debug.Assert(rotX >= 0 && rotX < graph.Size);
                    Debug.Assert(rotY >= 0 && rotY < graph.Size);

                    newX = rotX + newFace.X;
                    newY = rotY + newFace.Y;

                    newDirection = (direction + (4 - connection.Orientation.OrientationAsNumber())) % 4;
                    newHeading = CalcHeading(newDirection);

                    Console.WriteLine($"    Maybe moving to face {newFace.Id}. Direction: {newDirection}. ({x}, {y}) -> ({newX}, {newY})");
                }

                var tile = flatMap.FetchTile(newX, newY);
                Debug.Assert(tile != null);

                if (tile == MapSection.FLOOR)
                {
                    x = newX;
                    y = newY;

                    direction = newDirection;
                    heading = newHeading;
                    face = newFace;

                    Console.WriteLine($"({x}, {y})    D: {direction} -> {heading} ");
                }
                else
                {
                    Console.WriteLine($"Blocked at {newX}, {newY}");
                    break;
                }
            }
        }

        // get score
        // convert direction to right = 0
        int scoreDirection = direction - 1;
        if (scoreDirection < 0)
        {
            scoreDirection = 3;
        }

        Console.WriteLine("Complete");
        Console.WriteLine($"Direction: {direction}  ({x}, {y})");
        int score = scoreDirection + (4 * (x + 1)) + (1000 * (y + 1));
        Console.WriteLine($"Score: {score} (5031)");
        return score;
    }

    private Point CalcHeading(int direction)
    {

        return ((direction + 4) % 4) switch
        {
            0 => new Point(0, -1),
            1 => new Point(1, 0),
            2 => new Point(0, 1),
            3 => new Point(-1, 0),
            _ => throw new Exception($"Unknown direction: {direction}."),
        };
    }

}
