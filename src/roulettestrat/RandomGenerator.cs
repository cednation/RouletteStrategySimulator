namespace roulettestrat;

public interface IRandomGenerator
{
    int Seed { get; }
    int Next(int minValue, int maxValue);
}

public class RandomGenerator(int seed) : IRandomGenerator
{
    private readonly Random random = new Random(seed);

    public int Seed => seed;

    public int Next(int minValue, int maxValue)
    {
        return this.random.Next(minValue, maxValue);
    }
}