namespace roulettestratTest;

using FluentAssertions;
using roulettestrat;

public class SimulatorTest
{
    [Fact]
    public void RunRandomSimulation1000Spins()
    {
        var bankRoll = new BankRoll(5000);
        var wheel = new Wheel(new RandomGenerator(139));
        var bettingSystem = new DoubleDozenBettingSystem(bankRoll);
        var simulator = new Simulator(bankRoll, wheel, bettingSystem, 1000, hardLimit: false);
        simulator.RunSimulation();
        simulator.NumSpins.Should().BeGreaterThanOrEqualTo(1000);
        bettingSystem.Winnings.Should().BeGreaterThan(0);
    }

    [Fact]
    public void RunManySmallSimulations()
    {
        var winnings = new List<int>();

        for (int i = 1; i <= 20; i++)
        {
            var bankRoll = new BankRoll(3000);
            var wheel = new Wheel(new RandomGenerator(i * 6));

            // Spin the wheel an initial 10 times as the betting system needs an existing history to create its initial bet.
            for (int j = 0; j < 10; j++) wheel.Spin();

            var bettingSystem = new MilestoneReducingBettingSystem(bankRoll);
            var simulator = new Simulator(bankRoll, wheel, bettingSystem, 60, hardLimit: false); // playing for about an hour

            simulator.RunSimulation();
            winnings.Add(bettingSystem.Winnings);

            // var wheelHistory = wheel.GetHistory(100).Select(x => x is { Number: 0, ZeroType: SpinZero.Double } ? 37 : x.Number).ToArray();
        }

        winnings.Count.Should().Be(20);
        int total = winnings.Sum();
    }
}