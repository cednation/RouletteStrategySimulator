namespace roulettestratTest;

using System.Reflection;
using FluentAssertions;
using roulettestrat;

public class MilestoneReducingBetsSequenceTest
{
    [Fact]
    public void SetupInitialBetWithHistory()
    {
        var bankRoll = new BankRoll(2000);
        var numberHistory = new[]{ 2, 5, 33, 8, 8, 9, 11, 32, 34, 1 }; // Should be 2, 5, 33, 8, 9, 11 not covered
        var bettingSequence = new MilestoneReducingBetSequence(bankRoll);

        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(n => new SpinResult(n)));
        var bet = bettingSequence.Bets.First() as MilestoneReducingBet;
        bet.Should().NotBeNull();
        bet.BetAmount.Should().Be(32);

        var coveredNumbersField = typeof(MilestoneReducingBet).GetField("coveredNumbers", BindingFlags.NonPublic | BindingFlags.Instance);
        var coveredNumbers = (bool[]?)coveredNumbersField!.GetValue(bet);

        coveredNumbers![11].Should().Be(false);
        coveredNumbers.Count(x => x).Should().Be(32);
    }

    [Fact]
    public void RealSequence()
    {
        var bankRoll = new BankRoll(2000);
        var numberHistory = new List<int> { 26, 4, 33, 6, 13, 17 };
        
        var bettingSequence = new MilestoneReducingBetSequence(bankRoll);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));

        var spin = CreateSpinResult(15);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(4);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(25);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(9);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(26); // loss
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-21);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(4); // loss
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-81);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(29); 
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-57);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(3);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-29);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(16);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(3);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(10);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(39);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(33); // loss
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-65);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(7);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(15);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(3); // loss
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-185);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(2);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-9);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(4); // loss
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-393);

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(19);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(-9);
        bettingSequence.IsComplete.Should().BeFalse();

        numberHistory.Insert(0, spin.Number);
        bettingSequence.CreateNextBet(historyLength => numberHistory.Take(historyLength).Select(CreateSpinResult));
        spin = CreateSpinResult(36);
        bettingSequence.SetSpinResult(spin);
        bettingSequence.SequenceTotalAmount.Should().Be(407);
        bettingSequence.IsComplete.Should().BeTrue();
        bettingSequence.Bets.Count().Should().Be(15);
    }

    private static SpinResult CreateSpinResult(int number)
    {
        return number == 37 ? new SpinResult(0, SpinZero.Double) : new SpinResult(number);
    }
}