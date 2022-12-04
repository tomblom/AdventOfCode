using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

var assignmentPairs = input
    .Split(Environment.NewLine).Select(l => l.Split(',', '-')
    .Select(int.Parse).Chunk(2).Select(x => new Assignment(x[0], x[1])).ToArray());

int fullyContainedAssignments = assignmentPairs.Count(x => x[0].Contains(x[1]) || x[1].Contains(x[0]));
int overlappingAssignments    = assignmentPairs.Count(x => x[0].Overlaps(x[1]) && x[1].Overlaps(x[0]));

Console.WriteLine("Day Four:");
Console.WriteLine($"Part One: Fully contained assignments = {fullyContainedAssignments}");
Console.WriteLine($"Part Two: Overlapping assignments     = {overlappingAssignments}");

record struct Assignment(int Start, int End)
{
    public bool Contains(Assignment a) => Start <= a.Start && End >= a.End;
    public bool Overlaps(Assignment a) => End >= a.Start;
}
