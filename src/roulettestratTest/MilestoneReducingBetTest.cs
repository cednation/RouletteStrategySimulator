namespace roulettestratTest;

using FluentAssertions;
using roulettestrat;

public class MilestoneReducingBetTest
{
    [Fact]
    public void TestWin()
    {
        var coverage = new bool[38];
        for (int i = 0; i < 38; i++)
        {
            coverage[i] = true;
        }

        coverage[0] = false;
        coverage[3] = false;
        coverage[30] = false;
        coverage[14] = false;
        coverage[19] = false;
        coverage[21] = false;
        coverage[25] = false;
        coverage[10] = false;

        var bet = new MilestoneReducingBet(coverage, 2);
        var spin = new SpinResult(32);
        bet.SetSpinResult(spin);

        bet.Win.Should().BeTrue();
        bet.WinAmount.Should().Be(12);
        bet.BetAmount.Should().Be(30 * 2);
    }

    [Fact]
    public void TestLoss()
    {
        var coverage = new bool[38];
        for (int i = 0; i < 38; i++)
        {
            coverage[i] = true;
        }

        coverage[0] = false;
        coverage[3] = false;
        coverage[32] = false;
        coverage[14] = false;
        coverage[19] = false;
        coverage[21] = false;
        coverage[25] = false;
        coverage[10] = false;

        var bet = new MilestoneReducingBet(coverage, 2);
        var spin = new SpinResult(3);
        bet.SetSpinResult(spin);

        bet.Win.Should().BeFalse();
        bet.WinAmount.Should().Be(0);
        bet.BetAmount.Should().Be(30 * 2);
    }
}