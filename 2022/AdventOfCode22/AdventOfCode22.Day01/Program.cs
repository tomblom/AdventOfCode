StreamReader reader = new("input.txt");

string input = reader.ReadToEnd();

string[] inventories = input.Split(Environment.NewLine + Environment.NewLine);

List<int> inventoryTotals = inventories
    .Select(x => x.Split(Environment.NewLine).Sum(int.Parse))
    .OrderByDescending(x => x)
    .ToList();

Console.WriteLine("Day One:");

Console.WriteLine($"Largest Total is: {inventoryTotals.First()}");

Console.WriteLine($"Top 3 totals are: {inventoryTotals.Take(3).Sum()}");
