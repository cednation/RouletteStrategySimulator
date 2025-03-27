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

public readonly struct SpinResult(SpinColor color, int number, SpinZero zeroType)
{
    public SpinColor Color { get; } = color;
    public int Number { get; } = number;
    public SpinZero ZeroType { get; } = zeroType;
    public static int HighestNumber => 36;
}

public class Wheel(IRandomGenerator randomGenerator, int maxHistory = 1000)
{
    private readonly Queue<SpinResult> history = new(capacity: maxHistory);

    public SpinResult Spin()
    {
        int spinNumber = randomGenerator.Next(0, SpinResult.HighestNumber + 1);
        SpinZero zeroType = SpinZero.Single;
        if (spinNumber > SpinResult.HighestNumber)
        {
            spinNumber = 0;
            zeroType = SpinZero.Double;
        }
        SpinColor spinColor = spinNumber switch
        {
            0 => SpinColor.Green,
            _ when spinNumber % 2 == 0 => SpinColor.Red,
            _ => SpinColor.Black
        };

        SpinResult spinResult = new SpinResult(spinColor, spinNumber, zeroType);
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
