namespace GigLocal.Tests;

public class ValidatorTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(0, true)]
    [InlineData(-1, false)]
    public void FutureDateTest(int daysDiff, bool expected)
    {
        // Arrange
        var value = DateTime.Now.AddDays(daysDiff);
        var attrib = new FutureDate();

        // Act
        var result = attrib.IsValid(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Australia/Melbourne", true)]
    [InlineData("Australia/Adelaide", true)]
    [InlineData("America/Denver", false)]
    [InlineData("jlakjsldkjf", false)]
    public void AustralianTimeZoneTest(string timeZoneId, bool expected)
    {
        // Arrange
        var attrib = new AustralianTimeZone();

        // Act
        var result = attrib.IsValid(timeZoneId);

        // Assert
        Assert.Equal(expected, result);
    }
}
