using StreamReader reader = new("input.txt");
string input = reader.ReadToEnd();

PacketPair[] pairs = input.Split(Environment.NewLine + Environment.NewLine)
    .Select(x => x.Split(Environment.NewLine))
    .Select((x, i) => new PacketPair(i + 1, Item.Map(x[0]), Item.Map(x[1])))
    .ToArray();

int sumOfOrderedPairs = pairs.Where(CheckOrder).Sum(x => x.Index);

Item[] dividerPackets = new[] { Item.Map("[[2]]", true), Item.Map("[[6]]", true) };
Item[] packets = pairs.SelectMany(x => new[] { x.Left, x.Right }).Concat(dividerPackets).ToArray();

var decoderKey = packets
    .OrderByDescending(x => x, new ItemComparer())
    .Select((x, i) => (x.IsDividerPacket, Index: i + 1)).Where(x => x.IsDividerPacket)
    .Aggregate(1, (acc, x) => acc * x.Index);

Console.WriteLine("Day Thirteen:");
Console.WriteLine($"The sum of the indices of ordered pairs = {sumOfOrderedPairs}");
Console.WriteLine($"The decoder key = {decoderKey}");

static bool CheckOrder(PacketPair pair) => new ItemComparer().Compare(pair.Left, pair.Right) == 1;

file record PacketPair(int Index, Item Left, Item Right);

file class Item
{
    public int? Value { get; private set; }
    public List<Item>? Children { get; private set; }
    public Item? Parent { get; }

    public bool IsDividerPacket { get; private set; }
    public string Input { get; private set; } = null!;

    public bool IsInteger => Value.HasValue;
    public int ChildCount => IsInteger ? 0 : Children!.Count;

    private Item(Item parent, int value)
    {
        Parent = parent;
        Value = value;
    }

    private Item(Item? parent = null)
    {
        Parent = parent;
        Children = new();
    }

    public void ConvertToList()
    {
        if (!IsInteger) return;
        Children = new() { new Item(this, Value!.Value) };
        Value = null;
    }

    public static Item Map(string line, bool isDividerPacket = false)
    {
        Item parentItem = new();
        Item item = parentItem;

        int i = 0;
        while (i < line.Length)
        {
            char c = line[i];
            switch (c)
            {
                case '[':
                    item.Children!.Add(new Item(item));
                    item = item.Children!.Last();
                    break;

                case ']':
                    item = item.Parent!;
                    break;

                case char x when char.IsNumber(x):
                    char[] chars = line[i..].TakeWhile(char.IsNumber).ToArray();
                    item.Children!.Add(new Item(item, int.Parse(chars)));
                    i += chars.Length - 1;
                    break;

                default:
                    break;
            };
            i++;
        }

        item = parentItem.Children![0];
        item.IsDividerPacket = isDividerPacket;
        item.Input = line;
        return item;
    }
}

file class ItemComparer : IComparer<Item>
{
    public int Compare(Item? left, Item? right)
    {
        if (left is null || right is null)
            throw new ArgumentNullException();

        if (left.IsInteger && right.IsInteger)
        {
            return left.Value < right.Value
                ? 1
                : left.Value == right.Value
                ? 0
                : -1;
        }

        while (left.ChildCount == 1 && right.ChildCount == 1)
        {
            left = left.Children![0];
            right = right.Children![0];
        }

        left.ConvertToList();
        right.ConvertToList();

        for (int i = 0; i < left.ChildCount; i++)
        {
            if (right.ChildCount < i + 1) return -1;

            int status = Compare(left.Children![i], right.Children![i]);
            if (status != 0) return status;
        }

        return left.ChildCount == right.ChildCount ? 0 : 1;
    }
}
