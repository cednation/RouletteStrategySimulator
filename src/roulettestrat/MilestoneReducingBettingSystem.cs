namespace roulettestrat;

public class MilestoneReducingBettingSystem(BankRoll bankRoll) : IBettingSystem
{
    private readonly List<MilestoneReducingBetSequence> bettingSequences = [];
    private MilestoneReducingBetSequence? currentSequence;
    private bool currentSequenceHasSpinResult;
    private int winningsOfCompleteSequences;

    public IEnumerable<IBettingSequence> BettingSequences => bettingSequences.AsReadOnly();

    public bool ShouldEndEarly()
    {
        int currentSequenceWinnings = this.currentSequence?.SequenceTotalAmount ?? 0;
        int totalWinnings = currentSequenceWinnings + this.winningsOfCompleteSequences;

        return totalWinnings > 475;
    }

    public bool CanStopWheelNow()
    {
        return this.currentSequence == null || this.currentSequence.IsComplete;
    }

    public int Winnings
    {
        get
        {
            int currentSequenceWinnings = this.currentSequence?.SequenceTotalAmount ?? 0;
            int totalWinnings = currentSequenceWinnings + this.winningsOfCompleteSequences;

            return totalWinnings;
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
            this.currentSequence = new MilestoneReducingBetSequence(bankRoll);
            this.bettingSequences.Add(this.currentSequence);
        }

        this.currentSequence.CreateNextBet(getSpinHistoryFunc);
        this.currentSequenceHasSpinResult = false;
    }
}
