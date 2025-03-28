namespace roulettestratTest;

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using roulettestrat;

public class DoubleDozenBettingSequenceTest
{
    [Fact]
    public void WinFirstSpin()
    {
        var lastSpin = new SpinResult(30);
        var bank = new BankRoll(2000);

        var sequence = new DoubleDozenBettingSequence(bank);
        sequence.IsComplete.Should().BeFalse();
        sequence.CreateNextBet(_ => [lastSpin]);
        sequence.SetSpinResult(new SpinResult(20));

        sequence.IsComplete.Should().BeTrue();
        sequence.SequenceTotalAmount.Should().Be(4);
        sequence.Bets.Count().Should().Be(1);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void TotalLossSequence()
    {
        // Total loss is losing 5 times. So five times in the losing dozen or on 0.

        var lastSpinResult = new SpinResult(5);
        var bank = new BankRoll(2000);

        var sequence = new DoubleDozenBettingSequence(bank);
        
        // loss 1
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(lastSpinResult = new SpinResult(0, SpinZero.Double));

        sequence.IsComplete.Should().BeFalse();

        // loss 2
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(lastSpinResult = new SpinResult(10));

        sequence.IsComplete.Should().BeFalse();
        sequence.Bets.Last().Win.Should().BeFalse();
        sequence.Bets.Last().BetAmount.Should().Be(30);

        // loss 3
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(lastSpinResult = new SpinResult(8));

        sequence.IsComplete.Should().BeFalse();
        sequence.SequenceTotalAmount.Should().Be(-69 * 2);

        // loss 4
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(lastSpinResult = new SpinResult(0, SpinZero.Single));

        sequence.IsComplete.Should().BeFalse();

        // loss 5
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(lastSpinResult = new SpinResult(4));

        sequence.IsComplete.Should().BeTrue();
        sequence.SequenceTotalAmount.Should().Be((4 + 15 + 50 + 150 + 300) * -2);
    }

    [Fact]
    public void WinOnLastProgressionBet()
    {
        var lastSpinResult = new SpinResult(5);
        var bank = new BankRoll(2000);
        var sequence = new DoubleDozenBettingSequence(bank);

        for (int i = 0; i < 4; i++)
        {
            sequence.CreateNextBet(_ => [lastSpinResult]);
            sequence.SetSpinResult(lastSpinResult);
            sequence.IsComplete.Should().BeFalse();
        }

        // Now win on the last bet
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(new SpinResult(27));

        sequence.IsComplete.Should().BeTrue();
        sequence.SequenceTotalAmount.Should().Be((4 + 15 + 50 + 150) * -2 + 300); // -138
    }

    [Fact]
    public void NoBetAfterComplete()
    {
        var lastSpinResult = new SpinResult(5);
        var bank = new BankRoll(2000);
        var sequence = new DoubleDozenBettingSequence(bank);
        
        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(new SpinResult(31));
        sequence.IsComplete.Should().BeTrue();
        Action act = () => sequence.CreateNextBet(_ => [lastSpinResult]);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoBetIfLastBetNoSpin()
    {
        var lastSpinResult = new SpinResult(5);
        var bank = new BankRoll(2000);
        var sequence = new DoubleDozenBettingSequence(bank);

        sequence.CreateNextBet(_ => [lastSpinResult]);
        Action act = () => sequence.CreateNextBet(_ => [lastSpinResult]);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoDoubleSpin()
    {
        var lastSpinResult = new SpinResult(5);
        var bank = new BankRoll(2000);
        var sequence = new DoubleDozenBettingSequence(bank);

        sequence.CreateNextBet(_ => [lastSpinResult]);
        sequence.SetSpinResult(new SpinResult(30));
        Action act = () => sequence.SetSpinResult(new SpinResult(22));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoSpinHistory()
    {
        var bank = new BankRoll(2000);
        var sequence = new DoubleDozenBettingSequence(bank);

        sequence.CreateNextBet(_ => Enumerable.Empty<SpinResult>()); // If no history we choose first dozen to bet against.
        sequence.SetSpinResult(new SpinResult(30));
        sequence.IsComplete.Should().BeTrue();
        sequence.SequenceTotalAmount.Should().Be(4);
    }
}