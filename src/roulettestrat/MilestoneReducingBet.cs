namespace roulettestrat;

/// <summary>
/// Starts off covering most numbers and then takes them off as those numbers win.
/// </summary>
/// <remarks>https://www.youtube.com/watch?v=0MNJTVHI_zY&t=473s</remarks>
public class MilestoneReducingBet : IBet
{
    private readonly bool[] coveredNumbers;
    private readonly int betAmountPerNumber;
    private readonly int coverage;

    public MilestoneReducingBet(bool[] coveredNumbers, int betAmountPerNumber)
    {
        this.coveredNumbers = coveredNumbers;
        this.betAmountPerNumber = betAmountPerNumber;

        this.coverage = coveredNumbers.Count(x => x);
    }

    public void SetSpinResult(SpinResult spin)
    {
        int number = spin.Number;
        if (number == 0 && spin.ZeroType == SpinZero.Double) number = 37; // Treat double zero as 37

        this.Win = this.coveredNumbers[number];
        this.WinAmount = this.Win ? this.betAmountPerNumber * 36 - this.BetAmount : 0;
    }

    public bool Win { get; protected set; }
    public int BetAmount => this.coverage * this.betAmountPerNumber;
    public int WinAmount { get; protected set; }
}
