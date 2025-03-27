namespace roulettestratTest;

using FluentAssertions;
using roulettestrat;

public class DoubleDozenBetTest
{
    [Fact]
    public void SetSpinResult_Win()
    {
        var bet = new DoubleDozenBet(4, 1);

        var spin = new SpinResult(32);
        bet.SetSpinResult(spin);

        bet.Win.Should().BeTrue();
        bet.WinAmount.Should().Be(4);
        bet.BetAmount.Should().Be(8);
    }

    [Fact]
    public void SetSpinResult_Loss()
    {
        var bet = new DoubleDozenBet(12, 3);

        var spin = new SpinResult(26);
        bet.SetSpinResult(spin);

        bet.Win.Should().BeFalse();
        bet.WinAmount.Should().Be(0);
        bet.BetAmount.Should().Be(24);
    }

    [Fact]
    public void SetSpinResult_Edges()
    {
        var bet = new DoubleDozenBet(4, 2);
        var spin = new SpinResult(12);
        bet.SetSpinResult(spin);
        bet.Win.Should().BeTrue();

        bet = new DoubleDozenBet(4, 2);
        spin = new SpinResult(13);
        bet.SetSpinResult(spin);
        bet.Win.Should().BeFalse();

        bet = new DoubleDozenBet(4, 2);
        spin = new SpinResult(24);
        bet.SetSpinResult(spin);
        bet.Win.Should().BeFalse();

        bet = new DoubleDozenBet(4, 2);
        spin = new SpinResult(25);
        bet.SetSpinResult(spin);
        bet.Win.Should().BeTrue();
    }

    [Fact]
    public void SetSpinResult_Zero()
    {
        var bet = new DoubleDozenBet(4, 1);
        var spin = new SpinResult(0);
        bet.SetSpinResult(spin);
        bet.Win.Should().BeFalse();

        bet = new DoubleDozenBet(4, 1);
        spin = new SpinResult(0, SpinZero.Double);
        bet.SetSpinResult(spin);
        bet.Win.Should().BeFalse();
    }
}