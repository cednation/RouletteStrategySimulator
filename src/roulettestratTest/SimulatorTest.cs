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

        for (int i = 0; i < 20; i++)
        {
            var bankRoll = new BankRoll(2000);
            var wheel = new Wheel(new RandomGenerator(i * 205));
            var bettingSystem = new DoubleDozenBettingSystem(bankRoll);
            var simulator = new Simulator(bankRoll, wheel, bettingSystem, 60, hardLimit: false); // playing for about an hour

            simulator.RunSimulation();
            winnings.Add(bettingSystem.Winnings);
        }

        winnings.Count.Should().Be(20);
        int total = winnings.Sum();
    }
}