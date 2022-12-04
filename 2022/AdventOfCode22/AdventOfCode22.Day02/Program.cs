/*
 * Advent of Code: Day 2
 * 
 * Experimenting with Ardalis.SmartEnum package
 * https://github.com/ardalis/SmartEnum
 */

using System.Diagnostics;
using Ardalis.SmartEnum;

using StreamReader reader = new("input.txt");

string input = reader.ReadToEnd();

Console.WriteLine("Day Two:");

var partOneRounds = input.Split(Environment.NewLine).Select(x => new PartOneRound(x)).ToList();
int partOneScore = partOneRounds.Sum(x => x.Score);

Console.WriteLine($"Part One score is: {partOneScore}");

var partTwoRounds = input.Split(Environment.NewLine).Select(x => new PartTwoRound(x)).ToList();
int partTwoScore = partTwoRounds.Sum(x => x.Score);

Console.WriteLine($"Part Two score is: {partTwoScore}");

public sealed class PartOneRound : BaseRound
{
    public PartOneRound(string input) : base(input) { }

    public override Shape PlayerShape =>
        PlayerInput switch
        {
            'X' => Shape.Rock,
            'Y' => Shape.Paper,
            'Z' => Shape.Scissors,
            _ => throw new UnreachableException()
        };
}

public sealed class PartTwoRound : BaseRound
{
    public PartTwoRound(string input) : base(input) { }

    public override Shape PlayerShape =>
        PlayerInput switch
        {
            'X' => OpponentShape.WinsAgainst,
            'Y' => OpponentShape,
            'Z' => OpponentShape.LosesTo,
            _ => throw new UnreachableException()
        };
}

public abstract class BaseRound
{
    public BaseRound(string input)
    {
        if (input is ['A' or 'B' or 'C', ' ', 'X' or 'Y' or 'Z'] && input is [var firstChar, _, var secondChar])
        {
            OpponentInput = firstChar;
            PlayerInput = secondChar;
        }
        else
        {
            throw new ArgumentException($"{nameof(input)} is not valid");
        }
    }

    protected char OpponentInput { get; }

    protected char PlayerInput { get; }

    public Shape OpponentShape =>
        OpponentInput switch
        {
            'A' => Shape.Rock,
            'B' => Shape.Paper,
            'C' => Shape.Scissors,
            _ => throw new UnreachableException()
        };

    public abstract Shape PlayerShape { get; }

    public int Score => OutcomeScore + PlayerShape.Score;

    private int OutcomeScore =>
          PlayerShape == OpponentShape ? 3
        : PlayerShape.WinsAgainst == OpponentShape ? 6
        : 0;
}

public abstract class Shape : SmartEnum<Shape>
{
    public static readonly Shape Rock = new RockType();
    public static readonly Shape Paper = new PaperType();
    public static readonly Shape Scissors = new ScissorsType();

    private Shape(string name, int value) : base(name, value) { }

    public int Score => Value;

    public abstract Shape WinsAgainst { get; }

    public abstract Shape LosesTo { get; }

    private sealed class RockType : Shape
    {
        public RockType() : base(nameof(Rock), 1) { }

        public override Shape WinsAgainst => Scissors;
        public override Shape LosesTo => Paper;
    }

    private sealed class PaperType : Shape
    {
        public PaperType() : base(nameof(Paper), 2) { }

        public override Shape WinsAgainst => Rock;
        public override Shape LosesTo => Scissors;
    }

    private sealed class ScissorsType : Shape
    {
        public ScissorsType() : base(nameof(Scissors), 3) { }

        public override Shape WinsAgainst => Paper;
        public override Shape LosesTo => Rock;
    }
}
