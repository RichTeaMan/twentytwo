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

            if (line.Contains("L") || line.Contains("R"))
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
        var heading = CalcHeading(direction);
        var face = graph.Faces.First();

        Console.WriteLine($"Starting at ({x}, {y})");

        foreach (var instruction in puzzleInput.Instructions)
        {
            // turn ???
            if (instruction.Turn == Turn.LEFT)
            {
                direction--;
            }
            else if (instruction.Turn == Turn.RIGHT)
            {
                direction++;
            }
            direction = (direction + 4) % 4;
            heading = CalcHeading(direction);

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

                    // DEAL WITH MIRRORS!!!!!!!


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
                    //Console.WriteLine($"Direction {direction} | ({x}, {y}) -> ({newX}, {newY})");
                    //Console.WriteLine($"Face {face.Id} X ({newX} - {face.X}) PreRotX: {preRotX}");
                    //Console.WriteLine($"Face {face.Id} Y ({newY} - {face.Y}) PreRotY: {preRotY}");
                    Debug.Assert(preRotX >= 0 && preRotX < graph.Size);
                    Debug.Assert(preRotY >= 0 && preRotY < graph.Size);

                    int rotX = preRotX;
                    int rotY = preRotY;
                    // now do a rotation
                    foreach (var _i in Enumerable.Range(0, connection.Orientation.OrientationAsNumber()))
                    {
                        int tx = rotX;
                        int ty = rotY;

                        rotX = -ty;
                        rotY = tx;
                    }

                    newX = rotX + newFace.X;
                    newY = rotY + newFace.Y;

                    // change direction
                    foreach (var _i in Enumerable.Range(0, 4 - connection.Orientation.OrientationAsNumber()))
                    {

                    }
                    newDirection = (direction + (4 - connection.Orientation.OrientationAsNumber())) % 4;
                    newHeading = CalcHeading(newDirection);

                    Console.WriteLine($"    Maybe moving to face {newFace.Id}. Direction: {newDirection}");
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
        return scoreDirection + (4 * x) + (1000 * y);
    }

    private Point CalcHeading(int direction)
    {

        switch ((direction + 4) % 4)
        {
            case 0: return new Point(0, -1);
            case 1: return new Point(1, 0);
            case 2: return new Point(0, 1);
            case 3: return new Point(-1, 0);
            default: throw new Exception($"Unknown direction: {direction}.");
        }
    }

}
