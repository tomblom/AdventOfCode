using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();
string[] lines = input.Split(Environment.NewLine);

var partOnePrioritySum = FindAndSumPriorities(lines.Select(x => x.Chunk(x.Length / 2)));
var partTwoPrioritySum = FindAndSumPriorities(lines.Chunk(3).Select(x => x.Select(xx => xx.ToArray())));

Console.WriteLine("Day Three:");
Console.WriteLine($"Part One: The sum of the priorities is {partOnePrioritySum}");
Console.WriteLine($"Part Two: The sum of the priorities is {partTwoPrioritySum}");

static int FindAndSumPriorities(IEnumerable<IEnumerable<char[]>> chunks)
    => chunks
    .Select(x => x.Aggregate((IEnumerable<char> intersection, IEnumerable<char> next) => intersection.Intersect(next)).Single())
    .Select(item => char.IsUpper(item) ? item - ('A' - 27) : item - 'a' + 1)
    .Sum();
