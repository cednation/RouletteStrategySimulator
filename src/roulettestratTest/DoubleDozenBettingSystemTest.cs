namespace roulettestratTest;

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using roulettestrat;

public class DoubleDozenBettingSystemTest
{
    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void TwoWinningSequences()
    {
        var bankRoll = new BankRoll(2000);
        var system = new DoubleDozenBettingSystem(bankRoll);

        system.CanStopWheelNow().Should().BeTrue();

        var spinResult = new SpinResult(30);
        system.CreateNextBet(_ => [spinResult]);

        // Win first sequence
        system.SetSpinResult(spinResult = new SpinResult(20));

        system.CanStopWheelNow().Should().BeTrue();
        system.Winnings.Should().Be(4);
        system.BettingSequences.Count().Should().Be(1);

        // Now make 2nd sequence require 3 bets
        system.CreateNextBet(_ => [spinResult]);

        system.CanStopWheelNow().Should().BeFalse();
        system.SetSpinResult(spinResult = new SpinResult(19));

        system.CanStopWheelNow().Should().BeFalse();
        system.Winnings.Should().Be(-4);

        system.CreateNextBet(_ => [spinResult]);
        system.SetSpinResult(spinResult = new SpinResult(17));

        system.CanStopWheelNow().Should().BeFalse();
        system.Winnings.Should().Be(-34);

        // Win on third bet
        system.CreateNextBet(_ => [spinResult]);
        system.SetSpinResult(spinResult = new SpinResult(2));

        system.CanStopWheelNow().Should().BeTrue();
        system.Winnings.Should().Be(16);
        system.BettingSequences.Count().Should().Be(2);
    }

    [Fact]
    public void TotalLossAtStart()
    {
        var bankRoll = new BankRoll(2000);
        var system = new DoubleDozenBettingSystem(bankRoll);

        var spinResult = new SpinResult(30);
        
        for (int i = 0; i < 5; i++)
        {
            system.CreateNextBet(_ => [spinResult]);
            system.SetSpinResult(spinResult);

            if (i < 4) system.CanStopWheelNow().Should().BeFalse();
        }

        system.CanStopWheelNow().Should().BeTrue();
        system.Winnings.Should().Be((4 + 15 + 50 + 150 + 300) * -2);
        system.BettingSequences.Count().Should().Be(1);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void FailSequenceWithInsufficientBankRoll()
    {
        // Create bankRoll where after one loss we don't have enough left to start a new sequence.
        // We can have a sequence that completes with negative money, if we win on the 5th bet (due to 5th bet not being high enough due to table restraints)
        var bankRoll = new BankRoll(1040);
        var system = new DoubleDozenBettingSystem(bankRoll);

        var spinResult = new SpinResult(30);

        for (int i = 0; i < 4; i++)
        {
            system.CreateNextBet(_ => [spinResult]);
            system.SetSpinResult(spinResult);
        }

        // Now win on the 5th bet.
        system.CreateNextBet(_ => [spinResult]);
        system.SetSpinResult(spinResult = new SpinResult(5));

        system.CanStopWheelNow().Should().BeTrue();
        system.Winnings.Should().BeLessThan(-100);

        // Now try to start a new sequence
        Action act = () => system.CreateNextBet(_ => [spinResult]);
        act.Should().Throw<BankRollTooLowForNewBettingSequenceException>();
    }
}