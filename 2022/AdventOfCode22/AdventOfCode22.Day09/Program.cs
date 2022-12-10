using System.Diagnostics;

using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

IEnumerable<Instruction> instructions = input.Split(Environment.NewLine)
    .Select(x => x.Split())
    .Select(x => new Instruction(x[0][0], int.Parse(x[1])));

Rope part1Rope = new(1);
Rope part2Rope = new(9);

foreach (Instruction instruction in instructions)
{

    for (int i = 0; i < instruction.Count; i++)
    {
        part1Rope.Move(instruction.Direction);
        part2Rope.Move(instruction.Direction);
    }

    //Console.Clear();
    //Print(part2Rope);
}

Console.WriteLine("Day Nine:");
Console.WriteLine($"Part 1: Tail positions visited = {part1Rope.TailPositionsVisited.Count}");
Console.WriteLine($"Part 2: Tail positions visited = {part2Rope.TailPositionsVisited.Count}");

static void Print(Rope rope)
{
    string display = "";

    for (int y = 20; y > -20; y--)
    {
        for (int x = -20; x < 20; x++)
        {
            Position pos = new(x, y);

            if (pos == Rope.StartPosition)
            {
                display += 's';
            }
            else if (pos == rope.HeadPosition)
            {
                display += 'H';
            }
            else if (rope.KnotPositions.Any(pos.Equals))
            {
                display += '#';
            }
            else if (rope.TailPositionsVisited.Any(pos.Equals))
            {
                display += '+';
            }
            else
            {
                display += '.';
            }
        }

        display += Environment.NewLine;
    }

    Console.WriteLine(display);
}

file class Rope
{
    public static Position StartPosition = new(0, 0);

    public Position HeadPosition { get; private set; } = StartPosition;

    public Position[] KnotPositions { get; }

    public HashSet<Position> TailPositionsVisited { get; } = new HashSet<Position> { StartPosition };

    public Rope(int numberKnots) => KnotPositions = Enumerable.Range(0, numberKnots).Select(x => StartPosition).ToArray();

    public void Move(char direction)
    {
        HeadPosition = HeadPosition.Move(direction);

        Position previousPosition = HeadPosition;
        for (int i = 0; i < KnotPositions.Length; i++)
        {
            int xDiff = previousPosition.X - KnotPositions[i].X;
            int yDiff = previousPosition.Y - KnotPositions[i].Y;

            if (Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1)
            {
                KnotPositions[i] = KnotPositions[i] with { X = KnotPositions[i].X + Math.Sign(xDiff) };
                KnotPositions[i] = KnotPositions[i] with { Y = KnotPositions[i].Y + Math.Sign(yDiff) };
            }

            previousPosition = KnotPositions[i];
        }

        TailPositionsVisited.Add(KnotPositions.Last());
    }
}

file record struct Position(int X, int Y)
{
    public Position Move(char direction) => direction switch
    {
        'U' => this with { Y = Y + 1 },
        'D' => this with { Y = Y - 1 },
        'R' => this with { X = X + 1 },
        'L' => this with { X = X - 1 },
        _ => throw new UnreachableException()
    };
}

file record struct Instruction(char Direction, int Count);
