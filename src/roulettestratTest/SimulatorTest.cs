namespace roulettestratTest;

using FluentAssertions;
using roulettestrat;

public class SimulatorTest
{
    [Fact]
    public void RunRandomSimulation1000Spins()
    {
        var bankRoll = new BankRoll(5000);
        var wheel = new Wheel(new RandomGenerator(127));
        var bettingSystem = new DoubleDozenBettingSystem(bankRoll);
        var simulator = new Simulator(bankRoll, wheel, bettingSystem, 1000, hardLimit: false);
        simulator.RunSimulation();
        simulator.NumSpins.Should().BeGreaterThanOrEqualTo(1000);
        bettingSystem.Winnings.Should().BeGreaterThan(0);
    }
}