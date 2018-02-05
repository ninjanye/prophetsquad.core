using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsConverterTests
    {
        [Fact]
        public void ToFractional_NumberOver5_ReturnCorrectValue()
        {
            //Arrange
            decimal value = 12;
            string expected = "11/1";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToFractional_NumberOver5_MinusOneAndReturnOverOne()
        {
            //Arrange
            decimal value = 15;
            string expected = "14/1";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToFractional_NumberOver5AndHalfNumber_DoubleAndReturnOverTwo()
        {
            //Arrange
            decimal value = 5.5m;
            string expected = "9/2";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToFractional_WholeNumberOver2_ReturnOverOne()
        {
            //Arrange
            decimal value = 3;
            string expected = "2/1";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToFractional_WholeNumberIs2_ReturnEvens()
        {
            //Arrange
            decimal value = 2;
            string expected = "1/1";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToFractional_FractionOver2_ReturnCorrectFraction()
        {
            //Arrange
            decimal value = 3.75m;
            string expected = "11/4";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToFractional_ComplexFractionOver2_ReturnCorrectFraction()
        {
            //Arrange
            decimal value = 3.2m;
            string expected = "11/5";

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1.03, "1/33")][InlineData(1.04, "1/25")][InlineData(1.06, "1/16")][InlineData(1.07, "1/14")]
        [InlineData(1.08, "1/12")][InlineData(1.09, "1/11")][InlineData(1.11, "1/9")][InlineData(1.12, "1/8")]
        [InlineData(1.14, "1/7")][InlineData(1.16, "1/6")][InlineData(1.17, "1/6")][InlineData(1.2, "1/5")][InlineData(1.22, "2/9")]
        [InlineData(1.29, "2/7")][InlineData(1.33, "1/3")][InlineData(1.36, "4/11")][InlineData(1.44, "4/9")]
        [InlineData(1.53, "8/15")][InlineData(1.57, "4/7")][InlineData(1.62, "8/13")][InlineData(1.67, "4/6")]
        [InlineData(1.73, "8/11")][InlineData(1.83, "5/6")][InlineData(1.91, "10/11")][InlineData(2.38, "11/8")]
        [InlineData(2.5, "6/4")][InlineData(2.63, "13/8")][InlineData(2.86, "15/8")][InlineData(4.33, "10/3")]
        public void ToFractional_PreknownSubstitions_ReturnCorrectFraction(decimal value, string expected)
        {
            //Arrange

            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1.71, "7/10")][InlineData(1.39, "2/5")][InlineData(1.82, "5/6")][InlineData(1.97, "1/1")]
        [InlineData(1.63, "8/13")][InlineData(3.95, "3/1")][InlineData(3.96, "3/1")][InlineData(1.76, "3/4")]
        public void ToFractional_NoKnownSubDivisibleBy100_FindClosestMatch(decimal value, string expected)
        {
            //Arrange
            
            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(4.90, "4/1")][InlineData(4.91, "4/1")][InlineData(4.92, "4/1")][InlineData(4.93, "4/1")]
        [InlineData(4.94, "4/1")][InlineData(4.95, "4/1")][InlineData(5.1, "4/1")]
        public void ToFractional_DecimalBetween3And10_Create10RoundingSteps(decimal value, string expected)
        {
            //Arrange
                    
            //Act
            var result = OddsConverter.ToFractional(value);

            //Assert
            Assert.Equal(expected, result);
        }

    }    
}