using System;
using Xunit;
using GigLocal;

namespace GigLocal.Tests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(20, 10, 10)]
        [InlineData(10, 10, 10)]
        [InlineData(5, 10, 5)]
        public void TruncateTests(int stringLength, int truncateLength, int expectedLength)
        {
            // Arrange
            var str = new string('a', stringLength);

            // Act
            var truncated = str.Truncate(truncateLength);

            // Assert
            Assert.Equal(expectedLength, truncated.Length);
            if (stringLength > truncateLength)
                Assert.EndsWith("...", truncated);
        }

        [Theory]
        [InlineData(1, "st")]
        [InlineData(2, "nd")]
        [InlineData(3, "rd")]
        [InlineData(4, "th")]
        [InlineData(5, "th")]
        [InlineData(6, "th")]
        [InlineData(7, "th")]
        [InlineData(8, "th")]
        [InlineData(9, "th")]
        [InlineData(10, "th")]
        [InlineData(11, "th")]
        [InlineData(12, "th")]
        [InlineData(13, "th")]
        [InlineData(14, "th")]
        [InlineData(15, "th")]
        [InlineData(16, "th")]
        [InlineData(17, "th")]
        [InlineData(18, "th")]
        [InlineData(19, "th")]
        [InlineData(20, "th")]
        [InlineData(21, "st")]
        [InlineData(22, "nd")]
        [InlineData(23, "rd")]
        [InlineData(24, "th")]
        [InlineData(25, "th")]
        [InlineData(26, "th")]
        [InlineData(27, "th")]
        [InlineData(28, "th")]
        [InlineData(29, "th")]
        [InlineData(30, "th")]
        [InlineData(31, "st")]
        public void ToDaySuffixTests(int day, string expectedSuffix)
        {
            // Act
            var suffix = day.ToDaySuffix();

            // Assert
            Assert.Equal(expectedSuffix, suffix);
        }

        [Theory]
        [InlineData(8, 8, 2021, "Sunday 8th August")]
        public void ToDayOfWeekDateMonthNameTests(int day, int month, int year, string expectedDate)
        {
            // Arrange
            var date = new DateTime(year, month, day);

            // Act
            string dateFormatted = date.ToDayOfWeekDateMonthName();

            // Assert
            Assert.Equal(expectedDate, dateFormatted);
        }

        [Theory]
        [InlineData(8, 8, 2021, 7, 30, 00, "7:30 AM")]
        [InlineData(8, 8, 2021, 19, 30, 00, "7:30 PM")]
        public void ToTimeHourMinuteAmPmTests(int day, int month, int year, int hour, int minute, int second, string expectedDate)
        {
            // Arrange
            var date = new DateTime(year, month, day, hour, minute, second);

            // Act
            string dateFormatted = date.ToTimeHourMinuteAmPm();

            // Assert
            Assert.Equal(expectedDate, dateFormatted);
        }
    }
}
