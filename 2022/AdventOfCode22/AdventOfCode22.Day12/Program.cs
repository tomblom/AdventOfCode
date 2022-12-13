const char StartChar = 'S';
const char EndChar = 'E';

using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

char[][] intputGrid = input.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray();

Dictionary<Vertex, int> heightMap = intputGrid
    .SelectMany((row, y) => row.Select((c, x) => (x, y, h: ConvertSymbolToHeight(c))))
    .ToDictionary(t => new Vertex(t.x, t.y), t => t.h);

Vertex start = heightMap.Keys.First(x => intputGrid[x.Y][x.X] == StartChar);
Vertex end = heightMap.Keys.First(x => intputGrid[x.Y][x.X] == EndChar);

Dictionary<Vertex, Vertex[]> adjacencyList = new();

foreach (Vertex vertex in heightMap.Keys)
{
    Vertex[] edges = new[]
    {
        vertex with { X = vertex.X - 1},
        vertex with { X = vertex.X + 1},
        vertex with { Y = vertex.Y - 1},
        vertex with { Y = vertex.Y + 1}
    }
    .Where(x => heightMap.ContainsKey(x) && heightMap[vertex] - heightMap[x] < 2)
    .ToArray();

    adjacencyList.Add(vertex, edges);
}

Dictionary<Vertex, int> distanceMap = BFSTraversal(adjacencyList, end);

int minDistFromStartToEnd = distanceMap[start];
int minDistFromLowToEnd = heightMap.Where(x => x.Value == 1)
    .Min(x => distanceMap.ContainsKey(x.Key) ? distanceMap[x.Key] : int.MaxValue);

Console.WriteLine("Day Twelve:");
Console.WriteLine($"Minimum Distance from Start to End     = {minDistFromStartToEnd}");
Console.WriteLine($"Minimum Distance from Low Point to End = {minDistFromLowToEnd}");

static Dictionary<Vertex, int> BFSTraversal(Dictionary<Vertex, Vertex[]> adjacencyList, Vertex start)
{
    Dictionary<Vertex, int> distMap = new();
    Queue<(Vertex vertex, int dist)> queue = new();
    queue.Enqueue((start, 0));

    while (queue.Count > 0)
    {
        (Vertex vertex, int dist) = queue.Dequeue();

        if (distMap.ContainsKey(vertex)) continue;
        distMap[vertex] = dist;

        foreach (Vertex neighbour in adjacencyList[vertex])
        {
            if (!distMap.ContainsKey(neighbour))
                queue.Enqueue((neighbour, dist + 1));
        }
    }

    return distMap;
}

static int ConvertSymbolToHeight(char c) => c == StartChar ? 1 : c == EndChar ? 26 : c - 96;

record struct Vertex(int X, int Y);
