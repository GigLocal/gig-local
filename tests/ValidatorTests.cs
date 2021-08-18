using System;
using Xunit;
using GigLocal;

namespace GigLocal.Tests
{
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
    }
}
