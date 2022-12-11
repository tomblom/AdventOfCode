using System.Diagnostics;

using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

Instruction[] instructions = input.Split(Environment.NewLine)
    .Select(x => x.Split())
    .Select(x =>
    x[0] switch
    {
        "noop" => new Noop(),
        "addx" => new Addx(int.Parse(x[1])) as Instruction,
        _ => throw new UnreachableException()
    }).ToArray();

Dictionary<int, int> registryValues = new();
int cycleIndex = 0;
registryValues[cycleIndex] = 1;

foreach (Instruction instruction in instructions)
{
    while (!instruction.IsComplete)
    {
        cycleIndex++;
        instruction.Tick();

        registryValues[cycleIndex] = instruction.IsComplete
            ? instruction.Execute(registryValues[cycleIndex - 1])
            : registryValues[cycleIndex - 1];
    }
}

Console.WriteLine("Day Ten:");

(int i, int v)[] interestingValues = Enumerable.Range(0, 6)
    .Select(x => 20 + (x * 40) - 1)
    .Select(x => (i: x + 1, v: registryValues[x]))
    .ToArray();

foreach ((int i, int v) in interestingValues)
{
    Console.WriteLine($"Value at {i} = {v}");
}

Console.WriteLine($"Signal Strength sum  = {interestingValues.Sum(x => (x.i) * x.v)}");

string[] lines = registryValues
    .Chunk(40)
    .Select((c, i) => new string(c.Select(x => Math.Abs((x.Key % 40) - x.Value) < 2 ? '#' : '.').ToArray()))
    .ToArray();

Console.WriteLine();
Console.WriteLine("Outputting CRT Screen:");
Console.WriteLine(string.Join(Environment.NewLine, lines));

class Noop : Instruction
{
    public Noop() : base(1) { }
    public override int Execute(int x) => x;
}

class Addx : Instruction
{
    private readonly int _value;
    public Addx(int value) : base(2) => _value = value;
    public override int Execute(int x) => x + _value;
}

abstract class Instruction
{
    private readonly int _numberCycles;
    private int _cycleCount;

    public Instruction(int numberCycles) => _numberCycles = numberCycles;

    public bool IsComplete => _cycleCount == _numberCycles;

    public void Tick() => _cycleCount++;

    public abstract int Execute(int x);
}
