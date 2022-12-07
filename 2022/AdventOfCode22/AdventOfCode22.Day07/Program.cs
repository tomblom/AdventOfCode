using System.Diagnostics;
using System.Text.RegularExpressions;

using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

const int maxSize = 100_000;
const int totalDiskSpace = 70_000_000;
const int requiredUpdateSpace = 30_000_000;

string[][] commandExecutions = input
    .Split('$').Select(x => x.Split(Environment.NewLine).Where(l => !string.IsNullOrEmpty(l)).ToArray())
    .Where(x => x.Length > 0).ToArray();

Directory root = new("/");
Directory current = root;

foreach (string[] lines in commandExecutions)
{
    string command = lines[0].Trim();
    string[] output = lines[1..];

    switch (command)
    {
        case "ls":
            foreach (string line in output)
            {
                ApplyListOutput(current, line);
            }
            break;

        case "cd ..":
            current = current.Parent!;
            break;

        case string c when Regex.IsMatch(c, @"cd \w+"):
            current = current.Children.First(x => x.Name == c.Split(' ').Last());
            break;

        case "cd /": // noop
            break;

        default:
            throw new UnreachableException();
    }
}

Directory[] directories = Flatten(root.Children).Concat(new[] { root }).ToArray();

int totalSizeUnderLimit = directories.Where(x => x.Size <= maxSize).Sum(x => x.Size);
int spaceToFreeUp = requiredUpdateSpace - (totalDiskSpace - root.Size);
int sizeOfDirectoryToDelete = directories.Where(x => x.Size > spaceToFreeUp).Min(x => x.Size);

Console.WriteLine("Day Seven:");
Console.WriteLine($"The total size of dirs (under {maxSize}) = {totalSizeUnderLimit}");
Console.WriteLine($"The total size of dir to delete          = {sizeOfDirectoryToDelete}");

static void ApplyListOutput(Directory current, string output)
{
    string[] segments = output.Split(' ');

    if (segments[0] == "dir")
    {
        current.Children.Add(new Directory(segments[1], current));
    }
    else
    {
        current.Files.Add(new File(segments[1], int.Parse(segments[0])));
    }
}

static IEnumerable<Directory> Flatten(IEnumerable<Directory> d) => d.SelectMany(x => Flatten(x.Children)).Concat(d);

file record Directory(string Name, Directory? Parent = null)
{
    public List<File> Files { get; } = new();
    public List<Directory> Children { get; } = new();
    public int Size => Files.Sum(x => x.Size) + Children.Sum(x => x.Size);
}

file record File(string Name, int Size);
