using roulettestrat;

namespace roulettestratTest;

using FluentAssertions;
using Moq;

public class WheelTest
{
    [Fact]
    public void Spin_ShouldReturnCorrectResults()
    {
        var mockGenerator = new Mock<IRandomGenerator>();
        mockGenerator.SetupSequence(r => r.Next(0, SpinResult.HighestNumber + 1))
            .Returns(0)
            .Returns(1)
            .Returns(22)
            .Returns(SpinResult.HighestNumber)
            .Returns(SpinResult.HighestNumber + 1);

        var wheel = new Wheel(mockGenerator.Object);

        // Act
        var spin1 = wheel.Spin();
        var spin2 = wheel.Spin();
        var spin3 = wheel.Spin();
        var spin4 = wheel.Spin();
        var spin5 = wheel.Spin();

        // Assert
        spin1.Number.Should().Be(0);
        spin1.Color.Should().Be(SpinColor.Green);
        spin1.ZeroType.Should().Be(SpinZero.Single);

        spin2.Number.Should().Be(1);
        spin2.Color.Should().Be(SpinColor.Red);

        spin3.Number.Should().Be(22);
        spin3.Color.Should().Be(SpinColor.Black);

        spin4.Number.Should().Be(36);
        spin4.Color.Should().Be(SpinColor.Red);

        spin5.Number.Should().Be(0);
        spin5.Color.Should().Be(SpinColor.Green);
        spin5.ZeroType.Should().Be(SpinZero.Double);
    }

    [Fact]
    public void GetHistory_ShouldReturnLastNResults()
    {
        var mockGenerator = new Mock<IRandomGenerator>();
        mockGenerator.SetupSequence(r => r.Next(0, SpinResult.HighestNumber + 1))
            .Returns(0)
            .Returns(1)
            .Returns(2)
            .Returns(3)
            .Returns(4)
            .Returns(5);

        var wheel = new Wheel(mockGenerator.Object);
        for (int i = 0; i <= 5; i++)
        {
            wheel.Spin();
        }

        // Act
        var history = wheel.GetHistory(3).ToArray();

        // Assert
        history.Should().HaveCount(3);
        history[0].Number.Should().Be(5);
        history[1].Number.Should().Be(4);
        history[2].Number.Should().Be(3);
    }

    [Fact]
    public void GetHistory_ReturnsAllHistoryWhenNIsLarge()
    {
        var mockGenerator = new Mock<IRandomGenerator>();
        mockGenerator.SetupSequence(r => r.Next(0, SpinResult.HighestNumber + 1))
            .Returns(0)
            .Returns(1)
            .Returns(2);

        var wheel = new Wheel(mockGenerator.Object);
        for (int i = 0; i <= 2; i++)
        {
            wheel.Spin();
        }

        // Act
        var history = wheel.GetHistory(10);

        // Assert
        history.Should().HaveCount(3);
    }

    [Fact]
    public void Spin_MaxHistory()
    {
        var mockGenerator = new Mock<IRandomGenerator>();
        mockGenerator.SetupSequence(r => r.Next(0, SpinResult.HighestNumber + 1))
            .Returns(0)
            .Returns(1)
            .Returns(2)
            .Returns(3)
            .Returns(4)
            .Returns(5);

        var wheel = new Wheel(mockGenerator.Object, maxHistory: 3);
        int initialCapacity = wheel.GetHistoryQueueCapacity();
        for (int i = 0; i <= 3; i++)
        {
            wheel.Spin();
        }

        // Wheel is spun 4 times, but history should only 3
        wheel.GetHistory(10).Should().HaveCount(3);
        wheel.GetHistory(10).First().Number.Should().Be(3);

        // Now spin again
        wheel.Spin();
        wheel.GetHistory(10).Should().HaveCount(3);
        wheel.GetHistory(10).First().Number.Should().Be(4);

        // One more time
        wheel.Spin();
        wheel.GetHistory(10).Should().HaveCount(3);
        wheel.GetHistory(10).First().Number.Should().Be(5);

        // Verify capacity
        int finalCapacity = wheel.GetHistoryQueueCapacity();
        initialCapacity.Should().Be(finalCapacity);
    }
}