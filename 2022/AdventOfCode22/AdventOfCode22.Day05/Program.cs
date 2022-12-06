using System.Text.RegularExpressions;

using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

string[] lineTypes = input.Split(Environment.NewLine + Environment.NewLine);
string[] stackDefinitions = lineTypes[0].Split(Environment.NewLine);
string[] moveInstructions = lineTypes[1].Split(Environment.NewLine);

string stackInfoLine = stackDefinitions.Last();
StackInfo[] stackInformation = MatchOnIntegerRegex().Matches(stackInfoLine).Select(x => new StackInfo(int.Parse(x.Value), x.Index)).ToArray();

Dictionary<int, List<char>> stacks = stackInformation.ToDictionary(x => x.Stack, _ => new List<char>());
foreach (string line in stackDefinitions.Reverse().Skip(1))
{
    foreach (StackInfo info in stackInformation)
    {
        char crateId = line[info.Index];    
        if (char.IsLetter(crateId))
        {
            stacks[info.Stack].Add(crateId);
        }
    }
}

List<MoveInstruction> moves = moveInstructions
    .Select(x => MatchOnIntegerRegex().Matches(x).Select(x => int.Parse(x.Value)).ToArray())
    .Select(x => new MoveInstruction(x[0], x[1], x[2]))
    .ToList();

Dictionary<int, List<char>> partOneStacks = stacks;
Dictionary<int, List<char>> partTwoStacks = stacks.ToDictionary(x => x.Key, x => x.Value.ToList());

ApplyMoveInstructions(partOneStacks, moves, CrateMover9000MoveOperation);
ApplyMoveInstructions(partTwoStacks, moves, CrateMover9001MoveOperation);

string partOneMessage = string.Concat(partOneStacks.Values.Select(x => x.Last()));
string partTwoMessage = string.Concat(partTwoStacks.Values.Select(x => x.Last()));

Console.WriteLine("Day Five:");
Console.WriteLine($"Part One: Message = {partOneMessage}");
Console.WriteLine($"Part Two: Message = {partTwoMessage}");

static void ApplyMoveInstructions(Dictionary<int, List<char>> stacks, List<MoveInstruction> moves, Func<List<char>, int, IEnumerable<char>> moveOperation)
{
    foreach (MoveInstruction move in moves)
    {
        List<char> startStack = stacks[move.StartStack];
        stacks[move.EndStack].AddRange(moveOperation(startStack, move.Number));
        startStack.RemoveRange(startStack.Count - move.Number, move.Number);
    }
}

static IEnumerable<char> CrateMover9000MoveOperation(List<char> crates, int number) => crates.TakeLast(number).Reverse();
static IEnumerable<char> CrateMover9001MoveOperation(List<char> crates, int number) => crates.TakeLast(number);

record struct StackInfo(int Stack, int Index);
record struct MoveInstruction(int Number, int StartStack, int EndStack);

partial class Program
{
    [GeneratedRegex("\\d+")]
    private static partial Regex MatchOnIntegerRegex();
}
