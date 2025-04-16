namespace roulettestrat;
public class MilestoneReducingBetSequence(BankRoll bankRoll) : IBettingSequence
{
    private const int mileStone = 75;

    private readonly List<MilestoneReducingBet> bets = [];

    private bool[]? coveredNumbers;
    private int betAmountPerNumber = 1;
    private bool currentBetHasSpinResult;
    
    public IEnumerable<IBet> Bets => bets.AsReadOnly();
    public bool IsComplete { get; protected set; }
    public int SequenceTotalAmount { get; protected set; }
    
    public void SetSpinResult(SpinResult spin)
    {
        if (this.coveredNumbers == null) throw new InvalidOperationException("no bet has been created yet");

        if (this.currentBetHasSpinResult) throw new InvalidOperationException("current bet already has a spin result");

        var currentBet = this.bets.Last();
        currentBet.SetSpinResult(spin);
        this.currentBetHasSpinResult = true;

        if (currentBet.Win)
        {
            this.SequenceTotalAmount += currentBet.WinAmount;
            bankRoll.AddBetWin(currentBet.BetAmount + currentBet.WinAmount);
            if (this.SequenceTotalAmount >= mileStone) IsComplete = true;

            // Setup for next bet
            int number = spin.Number;
            if (number == 0 && spin.ZeroType == SpinZero.Double) number = 37; // Treat double zero as 37
            this.coveredNumbers[number] = false; // Uncover the winning number
        }
        else
        {
            this.SequenceTotalAmount -= currentBet.BetAmount;

            // Max doubling bet amount per number to 32 - at this amount each bet is about 800
            if (this.betAmountPerNumber < 32)
                this.betAmountPerNumber *= 2;
        }
    }

    public void CreateNextBet(Func<int, IEnumerable<SpinResult>> getSpinHistoryFunc)
    {
        if (this.IsComplete) throw new InvalidOperationException("Betting sequence is complete");

        this.coveredNumbers ??= CreateInitialCoveredNumbers(getSpinHistoryFunc);

        int betAmount = this.coveredNumbers.Count(x => x) * this.betAmountPerNumber;
        bankRoll.TakeCashForBet(betAmount);
        var bet = new MilestoneReducingBet(this.coveredNumbers, this.betAmountPerNumber);
        this.bets.Add(bet);
        this.currentBetHasSpinResult = false;
    }

    private static bool[] CreateInitialCoveredNumbers(Func<int, IEnumerable<SpinResult>> getSpinHistoryFunc)
    {
        var initialCoveredNumbers = new bool[38];
        for (int i = 0; i < 38; i++)
        {
            initialCoveredNumbers[i] = true;
        }

        // Get more history in case there are duplicates
        int unCoveredNumbers = 0;
        var spinHistory = getSpinHistoryFunc(10);
        foreach (var spin in spinHistory)
        {
            int spinNumber = spin.Number;
            if (spinNumber == 0 && spin.ZeroType == SpinZero.Double) spinNumber = 37; // Treat double zero as 37

            if (initialCoveredNumbers[spinNumber])
            {
                initialCoveredNumbers[spinNumber] = false;
                unCoveredNumbers++;
            }

            if (unCoveredNumbers >= 6) break;
        }

        if (unCoveredNumbers < 6)
        {
            // Starting at 0 look for and uncover enough uncovered numbers
            for (int j = 0; j < initialCoveredNumbers.Length && unCoveredNumbers < 6; j++)
            {
                if (initialCoveredNumbers[j])
                {
                    initialCoveredNumbers[j] = false;
                    unCoveredNumbers++;
                }
            }
        }

        return initialCoveredNumbers;
    }
}
