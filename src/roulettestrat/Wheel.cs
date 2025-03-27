namespace roulettestrat;

public enum SpinColor
{
    Green,
    Red,
    Black
}

public enum SpinZero
{
    Single,
    Double
}

public readonly struct SpinResult(int number, SpinZero zeroType = SpinZero.Single)
{
    public SpinColor Color { get; } = SpinColorStats.GetSpinColor(number);
    public int Number { get; } = number;
    public SpinZero ZeroType { get; } = zeroType;
    public static int HighestNumber => 36;
}

public static class SpinColorStats
{
    public static int[] RedNumbers { get; } = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

    public static int[] BlackNumbers { get; } = [2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35];

    public static SpinColor GetSpinColor(int number)
    {
        return number switch
        {
            0 => SpinColor.Green,
            _ when RedNumbers.Contains(number) => SpinColor.Red,
            _ when BlackNumbers.Contains(number) => SpinColor.Black,
            _ => throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be between 0-36")
        };
    }
}

public class Wheel(IRandomGenerator randomGenerator, int maxHistory = 1000)
{
    private readonly Queue<SpinResult> history = new(capacity: maxHistory);

    public int RandomSeed => randomGenerator.Seed;

    public SpinResult Spin()
    {
        int spinNumber = randomGenerator.Next(0, SpinResult.HighestNumber + 1);
        SpinZero zeroType = SpinZero.Single;
        if (spinNumber > SpinResult.HighestNumber)
        {
            spinNumber = 0;
            zeroType = SpinZero.Double;
        }
        
        SpinResult spinResult = new SpinResult(spinNumber, zeroType);
        this.AddToHistory(spinResult);

        return spinResult;
    }

    public IEnumerable<SpinResult> GetHistory(int length)
    {
        var lastSpins = history.TakeLast(length).Reverse();
        return lastSpins;
    }

    protected void AddToHistory(SpinResult spinResult)
    {
        if (this.history.Count >= maxHistory)
        {
            this.history.Dequeue();
        }
        this.history.Enqueue(spinResult);
    }
}
