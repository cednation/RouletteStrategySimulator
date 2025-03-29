namespace roulettestrat;

public class BankRoll(int initialCash)
{
    public void TakeCashForBet(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be positive");
        }

        if (amount > this.Amount)
        {
            throw new BankRollExceededException("Not enough cash to cover bet", this.Amount);
        }

        this.Amount -= amount;
    }

    public void AddBetWin(int amount)
    {
        this.Amount += amount;
    }

    public int InitialBankRoll => initialCash;

    // We always bet in whole dollar amounts, no need to use decimals.
    public int Amount { get; private set; } = initialCash;
}

public class BankRollExceededException(string message, int remainingBank) : Exception(message)
{
    public int RemainingBank => remainingBank;
}