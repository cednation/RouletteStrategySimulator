namespace roulettestrat;

public interface IBettingSystem
{
    IEnumerable<IBettingSequence> BettingSequences { get; }

    bool ShouldEndEarly();

    bool CanStopWheelNow();

    int Winnings { get; } // Negative if lost money

    void SetSpinResult(SpinResult spin);

    void CreateNextBet(Func<int, IEnumerable<SpinResult>> getSpinHistoryFunc);
}

public interface IBettingSystemFactory
{
    IBettingSystem CreateBettingSystem(BankRoll bankRoll);
}

public class DoubleDozenBettingSystem(BankRoll bankRoll) : IBettingSystem
{
    private const int requiredBankRoll = 1038;

    private readonly List<DoubleDozenBettingSequence> bettingSequences = [];
    private DoubleDozenBettingSequence? currentSequence;
    private bool currentSequenceHasSpinResult;
    private int winningsOfCompleteSequences;

    public IEnumerable<IBettingSequence> BettingSequences => bettingSequences.AsReadOnly();

    public bool ShouldEndEarly()
    {
        return this.currentSequence is { IsComplete: true } && this.winningsOfCompleteSequences > bankRoll.InitialBankRoll * 0.15;
    }

    public bool CanStopWheelNow()
    {
        return this.currentSequence == null || this.currentSequence.IsComplete;
    }

    public int Winnings
    {
        get
        {
            return this.winningsOfCompleteSequences + (this.currentSequence is { IsComplete: false } ? this.currentSequence.SequenceTotalAmount : 0);
        }
    }

    public void SetSpinResult(SpinResult spin)
    {
        if (this.currentSequence == null) throw new InvalidOperationException("no bet has been created yet");

        if (this.currentSequenceHasSpinResult) throw new InvalidOperationException("current sequence already has a spin result");

        this.currentSequence.SetSpinResult(spin);
        this.currentSequenceHasSpinResult = true;

        if (this.currentSequence.IsComplete)
        {
            this.winningsOfCompleteSequences += this.currentSequence.SequenceTotalAmount;
        }
    }

    public void CreateNextBet(Func<int, IEnumerable<SpinResult>> getSpinHistoryFunc)
    {
        if (this.currentSequence != null && !this.currentSequenceHasSpinResult) throw new InvalidOperationException("We have a current sequence that has not received a spin result");

        if (this.currentSequence == null || this.currentSequence.IsComplete)
        {
            if (bankRoll.Amount < requiredBankRoll)
                throw new BankRollTooLowForNewBettingSequenceException(
                    $"Bank roll is too low to create a new betting sequence. We require at least full sequence amount of: {requiredBankRoll}", bankRoll.Amount);

            this.currentSequence = new DoubleDozenBettingSequence(bankRoll);
            this.bettingSequences.Add(this.currentSequence);
        }

        this.currentSequence.CreateNextBet(getSpinHistoryFunc);
        this.currentSequenceHasSpinResult = false;
    }
}

public class DoubleDozenBettingSystemFactory : IBettingSystemFactory
{
    public IBettingSystem CreateBettingSystem(BankRoll bankRoll)
    {
        return new DoubleDozenBettingSystem(bankRoll);
    }
}

public class BankRollTooLowForNewBettingSequenceException(string message, int remainingBank) : Exception(message)
{
    public int RemainingBank { get; } = remainingBank;
}