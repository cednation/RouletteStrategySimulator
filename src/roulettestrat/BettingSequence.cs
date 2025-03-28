namespace roulettestrat;

public interface IBettingSequence
{
    IEnumerable<IBet> Bets { get; }

    bool IsComplete { get; }

    int SequenceTotalAmount { get; } // Negative if lost money

    void SetSpinResult(SpinResult spin);

    void CreateNextBet(Func<int, IEnumerable<SpinResult>> getSpinHistoryFunc);
}

public class DoubleDozenBettingSequence(BankRoll bankRoll) : IBettingSequence
{
    private readonly int[] betProgression = [4, 15, 50, 150, 300];
    private readonly List<DoubleDozenBet> bets = [];

    private int currentBetIndex = -1;
    private bool currentBetHasSpinResult;
    private int losingDozen = -1;

    public IEnumerable<IBet> Bets => bets.AsReadOnly();
    public bool IsComplete { get; protected set; }
    public int SequenceTotalAmount { get; protected set; }

    public void SetSpinResult(SpinResult spin)
    {
        if (this.currentBetHasSpinResult) throw new InvalidOperationException("current bet already has a spin result");

        var currentBet = this.bets[this.currentBetIndex];
        currentBet.SetSpinResult(spin);
        this.currentBetHasSpinResult = true;

        if (currentBet.Win)
        {
            this.IsComplete = true;
            this.SequenceTotalAmount += currentBet.WinAmount;
            bankRoll.AddBetWin(currentBet.BetAmount + currentBet.WinAmount);
        }
        else
        {
            this.SequenceTotalAmount -= currentBet.BetAmount;

            // Check if we have reached the end of our sequence. This is a total loss scenario.
            if (this.currentBetIndex == this.betProgression.Length - 1)
            {
                this.IsComplete = true;
            }
        }
    }

    public void CreateNextBet(Func<int, IEnumerable<SpinResult>> getSpinHistoryFunc)
    {
        if (this.IsComplete) throw new InvalidOperationException("Betting sequence is complete");

        if (this.currentBetIndex > -1 && !this.currentBetHasSpinResult) throw new InvalidOperationException("We have a current bet that has not received a spin result");

        // If this is the first bet - we decide which 2 dozens to bet on and stick with it for the rest of the sequence.
        if (this.losingDozen == -1)
        {
            int spinNumber = getSpinHistoryFunc(1).FirstOrDefault().Number;
            if (spinNumber == 0) spinNumber = 1; // If number is 0, then choose the first dozen

            this.losingDozen = ((spinNumber - 1) / 12) + 1; // We bet against the last dozen
        }

        int betAmount = this.betProgression[++this.currentBetIndex];
        bankRoll.TakeCashForBet(betAmount * 2); // bet this amount on each of our two dozens
        var bet = new DoubleDozenBet(betAmount, this.losingDozen);
        this.bets.Add(bet);
        this.currentBetHasSpinResult = false;
    }
}