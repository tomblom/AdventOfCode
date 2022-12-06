using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

int startOfPacketMarkerIndex  = FindMarkerIndex(input, 4);
int startOfMessageMarkerIndex = FindMarkerIndex(input, 14);

Console.WriteLine("Day Six:");
Console.WriteLine($"Start Of Packet  Marker Index = {startOfPacketMarkerIndex}");
Console.WriteLine($"Start Of Message Marker Index = {startOfMessageMarkerIndex}");

static int FindMarkerIndex(string input, int windowSize)
{
    for (int i = 0; i < input.Length - windowSize + 1; i++)
    {
        string slice = input[i..(i + windowSize)];
        if (input[i..(i + windowSize)].Distinct().Count() == windowSize) return i + windowSize;
    }

    return -1;
}
