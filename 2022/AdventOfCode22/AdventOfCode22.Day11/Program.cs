using System.Diagnostics;
using System.Text.RegularExpressions;

using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

Monkey[] monkeys = input.Split(Environment.NewLine + Environment.NewLine).Select(input => Monkey.MapFromString(input, x => x / 3)).ToArray();

Console.WriteLine("Day Eleven:");

Console.WriteLine("Part One:");
for (int i = 1; i <= 20; i++)
{
    foreach (Monkey monkey in monkeys)
    {
        monkey.InspectItems(monkeys);
    }

    Console.WriteLine();
    Console.WriteLine($"After round {i}, the monkeys are holding items with these worry levels:");
    Console.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, monkeys.Length).Select(i => $"Monkey {i}: {string.Join(", ", monkeys[i].Items)}")));
}

long levelOfMonkeyBusiness = monkeys.Select(x => (long)x.InspectionCount).OrderByDescending(x => x).Take(2).Aggregate(1, (long acc, long x) => acc * x);

Console.WriteLine();
Console.WriteLine($"Level Of Monkey Business = {levelOfMonkeyBusiness}");

Console.WriteLine();
Console.WriteLine("Part Two:");

// Taking the product of all mods and using `x => x % modProduct` gives us a smaller number (to keep worry managable) that is divisible by all mods.
int modProduct = monkeys.Aggregate(1, (mod, monkey) => mod * monkey.Mod);
monkeys = input.Split(Environment.NewLine + Environment.NewLine).Select(input => Monkey.MapFromString(input, x => x % modProduct)).ToArray();

int[] roundsToLog = Enumerable.Range(1, 10).Select(x => x * 1000).Concat(new[] { 1, 20 }).ToArray();

for (int i = 1; i <= 10_000; i++)
{
    foreach (Monkey monkey in monkeys)
    {
        monkey.InspectItems(monkeys);
    }

    if (roundsToLog.Contains(i))
    {
        Console.WriteLine();
        Console.WriteLine($"== After round {i} ==");
        Console.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, monkeys.Length).Select(i => $"Monkey {i} inspected items {monkeys[i].InspectionCount} times.")));
    }
}

levelOfMonkeyBusiness = monkeys.Select(x => (long)x.InspectionCount).OrderByDescending(x => x).Take(2).Aggregate(1, (long acc, long x) => acc * x);

Console.WriteLine();
Console.WriteLine($"Level Of Monkey Business = {levelOfMonkeyBusiness}");

partial record Monkey(Func<long, long> Operation, int Mod, int MonkeyIfPass, int MonkeyIfFail, Func<long, long> ApplyReleif)
{
    public List<long> Items { get; } = new();

    public int InspectionCount { get; private set; } = 0;

    public void InspectItems(Monkey[] monkeys)
    {
        foreach (long item in Items)
        {
            InspectionCount++;
            long worryLevel = ApplyReleif(Operation(item));
            monkeys[worryLevel % Mod == 0 ? MonkeyIfPass : MonkeyIfFail].Items.Add(worryLevel);
        }

        Items.Clear();
    }

    private bool TestWorry(long worryLevel)
    {
        decimal result = worryLevel / new decimal();
        return result == (long)result;
    }

    public static Monkey MapFromString(string input, Func<long, long> applyReleif)
    {
        string[] inputs = input.Split(Environment.NewLine);

        Func<long, long> operation = inputs[2] switch
        {
            string s when s.Contains("old + old") => x => x + x,
            string s when s.Contains("old * old") => x => x * x,
            string s when s.Contains('+') => x => x + inputs[2].FindInt(),
            string s when s.Contains('*') => x => x * inputs[2].FindInt(),
            _ => throw new UnreachableException()
        };

        Monkey monkey = new(operation, inputs[3].FindInt(), inputs[4].FindInt(), inputs[5].FindInt(), applyReleif);
        monkey.Items.AddRange(inputs[1].FindInts().Select(x => (long)x));

        return monkey;
    }
}

static partial class RegexHelpers
{
    [GeneratedRegex("\\d+")]
    public static partial Regex IntegerRegex();

    public static int FindInt(this string input) => int.Parse(IntegerRegex().Match(input).Value);
    public static IEnumerable<int> FindInts(this string input) => IntegerRegex().Matches(input).Select(x => int.Parse(x.Value));
}
