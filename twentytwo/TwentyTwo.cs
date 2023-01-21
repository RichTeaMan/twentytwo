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
        string instructions = "";

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
                instructions = line;

                /*
                            let directions = line.split_inclusive(&['L', 'R']);

                            for direction in directions {
                                let steps_str = direction.trim_end_matches(&['L', 'R']);
                                let steps = steps_str.parse::<i32>().unwrap();
                                instructions.push(Instruction {
                                    steps,
                                    direction: if direction.contains('L') {
                                        Direction::LEFT
                                    } else if direction.contains('R') {
                                        Direction::RIGHT
                                    } else {
                                        Direction::NONE
                                    },
                                });
                            }
                */

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

        return new PuzzleInput(map, instructions);

    }
}
