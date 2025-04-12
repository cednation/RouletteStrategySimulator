namespace roulettestrat;

public class Simulator(BankRoll bankRoll, Wheel wheel, IBettingSystem bettingSystem, int totalSpins = 1000, bool hardLimit = false)
{
    private int spinCount;

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public int NumSpins => this.spinCount;

    public void RunSimulation()
    {
        try
        {
            while (true)
            {
                if (bettingSystem.ShouldEndEarly()) break;

                if (this.spinCount > totalSpins)
                {
                    bool stop = true;
                    if (!hardLimit) // Ask betting system if we can stop. If in a sequence we may continue a few more spins.
                    {
                        stop = bettingSystem.CanStopWheelNow();
                    }

                    if (stop) break;
                }

                bettingSystem.CreateNextBet(wheel.GetHistory);
                var spinResult = wheel.Spin();
                this.spinCount++;
                bettingSystem.SetSpinResult(spinResult);
            }
        }
        catch (BankRollTooLowForNewBettingSequenceException)
        {
            // Not enough money to continue. Stop the simulation.
        }
        catch (BankRollExceededException)
        {
            // Ran out of money to place a bet
        }
    }
}
