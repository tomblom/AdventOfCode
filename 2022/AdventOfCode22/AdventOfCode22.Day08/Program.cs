using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

int[][] heightGrid = input.Split(Environment.NewLine)
    .Select(x => Array.ConvertAll(x.ToCharArray(), c => (int)char.GetNumericValue(c))).ToArray();

int xLength = heightGrid[0].Length;
int yLength = heightGrid.Length;

IEnumerable<(int x, int y)> coordinates = Enumerable.Range(0, yLength)
    .Select(y => Enumerable.Range(0, xLength).Select(x => (x, y)))
    .SelectMany(x => x);

int numberVisibleTrees = 0;
int maxScenicScore     = 0;

bool[,] visibilityMap = new bool[xLength, yLength];

foreach ((int cx, int cy) in coordinates)
{
    int height = heightGrid[cy][cx];

    int[] treesToRht = Enumerable.Range(cx + 1, xLength - (cx + 1)).Select(x => heightGrid[cy][x]).ToArray();
    int[] treesToLft = Enumerable.Range(0, cx).Reverse().Select(x => heightGrid[cy][x]).ToArray();
    int[] treesToBtm = Enumerable.Range(cy + 1, yLength - (cy + 1)).Select(y => heightGrid[y][cx]).ToArray();
    int[] treesToTop = Enumerable.Range(0, cy).Reverse().Select(y => heightGrid[y][cx]).ToArray();

    int[][] directions = new[]{ treesToRht, treesToLft, treesToBtm, treesToTop };

    if (directions.Any(d => IsVisibleFromOutside(height, d)))
    {
        numberVisibleTrees++;
        visibilityMap[cx, cy] = true;
    }

    int scenicScore = directions.Select(x => VisibleTreeCount(height, x)).Aggregate(1, (acc, x) => acc * x);
    if (scenicScore > maxScenicScore) { maxScenicScore = scenicScore; }
}

Console.WriteLine("Day Eight:");
Console.WriteLine(MapToString(visibilityMap));
Console.WriteLine($"Number of trees visisble = {numberVisibleTrees}");
Console.WriteLine($"Highest scenic score     = {maxScenicScore}");

static int VisibleTreeCount(int treeHeight, int[] treeLine) =>
      IsVisibleFromOutside(treeHeight, treeLine)
    ? treeLine.Length
    : treeLine.TakeWhile(x => x < treeHeight).Count() + 1;

static bool IsVisibleFromOutside(int treeHeight, int[] treeLine) => treeLine.All(x => x < treeHeight);

static string MapToString(bool[,] map, char trueChar = '^', char falseChar = '0')
{
    int xl = map.GetLength(0);
    int yl = map.GetLength(1);

    string str = Environment.NewLine;
    for (int y = 0; y < yl; y++)
    {
        for (int x = 0; x < xl; x++)
        {
            str += map[x, y] ? trueChar : falseChar;
        }
        str += Environment.NewLine;
    }

    return str;
}
