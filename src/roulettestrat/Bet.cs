namespace roulettestrat;

public interface IBet
{
    void SetSpinResult(SpinResult spin);
    
    bool Win { get; }

    int BetAmount { get; }

    int WinAmount { get; }
}

public class DoubleDozenBet : IBet
{
    private readonly int betOnEachDozen;
    private readonly int losingDozen; // 1, 2, or 3 (we bet on the other two)

    public DoubleDozenBet(int betOnEachDozen, int losingDozen)
    {
        if (betOnEachDozen < 4) throw new ArgumentException("Minimum bet is 4");

        if (losingDozen is < 1 or > 3) throw new ArgumentOutOfRangeException(nameof(losingDozen), losingDozen, "Must be 1st, 2nd, or 3rd dozen");

        this.betOnEachDozen = betOnEachDozen;
        this.losingDozen = losingDozen;
    }

    public void SetSpinResult(SpinResult spin)
    {
        this.Win = false;
        if (spin.Number == 0)
        {
            return;
        }

        int spinDozen = ((spin.Number - 1) / 12) + 1;
        if (spinDozen != this.losingDozen)
        {
            this.Win = true;
            this.WinAmount = this.betOnEachDozen;
        }
    }

    public bool Win { get; private set; }

    public int BetAmount => this.betOnEachDozen * 2;

    public int WinAmount { get; private set; }
}