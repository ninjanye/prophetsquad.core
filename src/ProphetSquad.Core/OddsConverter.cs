using System.Collections.Generic;
using System.Linq;

namespace ProphetSquad.Core
{
    public static class OddsConverter
    {
        private static readonly IDictionary<decimal, string> KnownFractions = new Dictionary<decimal, string>
            {
                {1.03m, "1/33"},{1.06m, "1/16"},{1.07m, "1/14"},{1.08m, "1/12"},{1.09m, "1/11"},
                {1.11m, "1/9"},{1.13m, "1/8"},{1.14m, "1/7"},{1.17m, "1/6"},{1.22m, "2/9"},
                {1.29m, "2/7"},{1.33m, "1/3"},{1.36m, "4/11"},{1.44m, "4/9"},{1.53m, "8/15"},
                {1.57m, "4/7"},{1.62m, "8/13"},{1.67m, "4/6"},{1.73m, "8/11"},{1.83m, "5/6"},
                {1.91m, "10/11"},{2.01m, "1/1"},{2.02m, "1/1"},{2.38m, "11/8"},{2.5m, "6/4"},
                {2.63m, "13/8"},{2.86m, "15/8"},{4.33m, "10/3"}
            }; 

        public static string ToFractional(decimal value)
        {
            int maxDenominator = 16;
            if (value > 3)
            {
                maxDenominator = 5;
            }
            var fraction = FindFraction(value);
            var denominator = fraction.GetDenominator();
            if (value > 1.1m && denominator > maxDenominator)
            {
                return FindNearestMatch(value, maxDenominator);
            }

            return fraction;
        }

        private static string FindFraction(decimal value)
        {
            if (KnownFractions.ContainsKey(value))
            {
                return KnownFractions[value];
            }

            var decimalPlace = (value % 1) * 100;
            int loopCount = 1;
            var multiplier = value - 1;
            decimal multipliedValue = multiplier;
            while (decimalPlace > 0)
            {
                loopCount++;
                multipliedValue += multiplier;
                decimalPlace = (multipliedValue % 1) * 100;
            }

            return string.Format("{0}/{1}", (multipliedValue).ToString("0"), loopCount);            
        }

        private static string FindNearestMatch(decimal value, int maxDenominator)
        {
            const decimal step = 0.01m;
            decimal increase = value;
            decimal decrease = value;

            var precisionRange = 5;
            if (value > 3)
            {
                precisionRange = 10;
            }

            for (int i = 1; i <= precisionRange; i++)
            {
                var displacement = step*i;
                var increaseFraction = FindFraction(increase + displacement);
                if (increaseFraction.GetDenominator() < maxDenominator)
                {
                    return increaseFraction;
                }
                
                var decreaseFraction = FindFraction(decrease - displacement);
                var decreasedDenominator = decreaseFraction.GetDenominator();
                if (decreasedDenominator < maxDenominator)
                {
                    return decreaseFraction;
                }
            }

            return FindFraction(value);
        }

        private static int GetDenominator(this string fraction)
        {
            var slashLocation = fraction.IndexOf('/');
            if (slashLocation < 0)
            {
                return 100;
            }

            string rightSide = fraction.Split('/').LastOrDefault();
            int rightSideNumber;
            bool isNumber = int.TryParse(rightSide, out rightSideNumber);
            if (isNumber)
            {
                return rightSideNumber;
            }

            return 100;
        }
    }
}