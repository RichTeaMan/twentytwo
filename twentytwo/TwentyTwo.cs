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
}
