using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Commerce
{
    /*
     * <Note>
     * In this debugging challenge, the goal is to remedy the described problem while changing as
     * little code as possible. You're not trying to minimize the number of keystrokes (or something
     * equally silly), but you should only change parts that are incorrect and could be contributing
     * to the described problem.
     * 
     * Select your choice of language in the code editor to receive the initial (buggy) code. Since
     * you're dealing with existing code, the language choice is limited to four of the most common
     * OO languages.
     * </Note>
     * 
     * A company has an e-commerce app through which they sell their products. Recently, the business
     * calculated how much money they <em>should</em> have received over the last few months, based on
     * the retail price of the items sold, the business rules for calculating shipping charges on those
     * items, and the legally mandated sales taxes. This total was found to be different from the total
     * amount of money actually taken in by the business and charged to customers over the same period
     * of time. There haven't been any sales, promotions, or any other reason why these sums should
     * differ.
     * 
     * After carefully examining and double-checking records, people at the company are coming to the
     * conclusion that there might be a error in the logic that calculates the final price to charge
     * customers. The final price has three components: the base retail price of the item, the shipping
     * charges, and the sales tax. The code for determining the base retail price is solid, and consists
     * of nothing but simple database lookups. It's become apparent that the flaw must be in the logic
     * for calculating shipping charges and sales tax.
     * 
     * The shipping charges and sales tax should be based entirely on the address of the recipient and
     * the value of their order.
     * 
     * For shipping charges, the business rules are to charge an amount that depends on zip (postal)
     * code. The business ships from the West coast, so:
     * 
     * <ul>
     *   <li>Zip codes higher than 75000 will pay a flat rate of $10 for shipping.</li>
     *   <li>Zip codes between 25000 and 75000, inclusive, will pay a flat rate of $20 for shipping.</li>
     *   <li>All other zip codes will pay a flat rate of $30.</li>
     * </ul>
     * 
     * The rules for sales taxes are as follows (use these rates for this problem). The taxes are
     * charged as a percentage of the base retail price; there's no tax on shipping charges.
     * 
     * <ul>
     *   <li>Arizona residents pay 5% tax.</li>
     *   <li>Washington residents pay 9% tax.</li>
     *   <li>California residents pay 6% tax.</li>
     *   <li>Delaware residents pay no tax.</li>
     *   <li>All other residents pay 7% tax.</li>
     * </ul>
     * 
     * You try a few test cases, shown below. None of the results you get differ from the expected
     * results, though. Can you figure out what's wrong and fix the code?
     * 
     * Your Unit Test
     * 
     * Input:
     * 
     * 6
     * 1000  
     * 300 Dark Lane, Seattle, Washington 98100  
     * 500  
     * 2147 spooky dr., portland, oregon 97218  
     * 2300  
     * 7345 main St., San Francisco, California 94100  
     * 800  
     * 300 Hedge Drive, Tuscon, Arizona 85700  
     * 2000  
     * 20 ranch avenue, albany, kentucky 42600  
     * 1000  
     * 2700 Axe Factory Road, Philadelphia, Pennsylvania 19152
     * 
     * Output:
     * 
     * 1100
     * 545
     * 2448
     * 850
     * 2160
     * 1100
     * 
     * Input
     * 
     * 2N lines follow, meant to be read in groups of 2. In each group of two lines, the first
     * line contains a single integer that represents the base retail price of the order in dollars.
     * The price is always a multiple of 100, so you can do all tax calculations as integer calculations.
     * The second line represents an address.
     * 
     * Though the addresses are based on user input, they have been normalized to a relatively sane
     * format. Each address has also been verified on a map, so you're not getting any invalid addresses.
     * Each address looks like this: "<street number> <street name>, <city name>, <state name> <zip code>"
     * (quotes and <> for clarity; see unit test for sample input).
     * 
     * Some addresses can have multiple parts to the street address,
     * e.g. "<street number> <street name>, <second address line text>, <city name>, <state name> <zip code>".
     * Other than that, none of the parts delimited by <> tags can contain commas.
     * 
     * Some state names may be specified by using their two-letter postal codes (you can look these up)
     * instead of the full state name.
     * 
     * Output
     * 
     * The output should be written to standard output. For each of the N cases, output on a separate
     * line the correct amount to charge, in dollars (output the answer for the i-th test case on the
     * i-th line).
     */

    /// <summary>
    /// Added first class CostCalculator to improve testability.
    /// </summary>
    public class CostCalculator
    {
        public CostCalculator(TextReader reader, TextWriter writer)
        {
            _writer = writer;
            Read(reader);
        }

        private int _count;

        private readonly TextWriter _writer;

        private static string PrepareText(string text)
        {
            // Could get more sophisticated here.
            return text.Trim();
        }

        private void Read(TextReader reader)
        {
            _count = int.Parse(PrepareText(reader.ReadLineAsync().Result));

            for (var i = 0; i < _count; i++)
            {
                var basePrice = int.Parse(PrepareText(reader.ReadLineAsync().Result));
                var addr = new Address(reader.ReadLineAsync().Result);

                var taxAmount = TaxCalculator.Calculate(basePrice, addr.State);
                var shippingAmount = ShippingCalculator.Calculate(addr.ZipCode);

                _writer.WriteLine(basePrice + taxAmount + shippingAmount);
            }
        }
    }

    public class Program
    {
        private static readonly string Scenario = @"6
1000
300 Dark Lane, Seattle, Washington 98100  
500
2147 spooky dr., portland, oregon 97218  
2300
7345 main St., San Francisco, California 94100  
800
300 Hedge Drive, Tuscon, Arizona 85700  
2000
20 ranch avenue, albany, kentucky 42600  
1000
2700 Axe Factory Road, Philadelphia, Pennsylvania 19152";

        public static void Main(string[] args)
        {
            using (var reader = new StringReader(Scenario))
            {
                new CostCalculator(reader, Console.Out);
            }

            //new CostCalculator(Console.In, Console.Out);
        }
    }

    /// <summary>
    /// When .NET is at your disposal, there is no excuse to not use properties instead of calculated "accessor" methods.
    /// </summary>
    internal class Address
    {
        public string AddressLine { get; private set; }

        public string StreetAddress { get; private set; }

        public string SecondLine { get; private set; }

        public string CityName { get; private set; }

        public string State { get; private set; }

        public int ZipCode { get; private set; }

        public Address(string addressLine)
        {
            AddressLine = addressLine;
            ParseAddress(addressLine);
        }

        /// <summary>
        /// Pattern: streetaddr, city, state zip
        /// </summary>
        private const string Pattern = "^(?<streetaddr>.*),\\s+(?<city>.*),\\s+(?<state>.*)\\s+(?<zip>\\d{5})$";

        /// <summary>
        /// Pattern2: streetaddr, secondline, city, state zip
        /// </summary>
        private const string Pattern2 = "^(?<streetaddr>.*),\\s+(?<secondline>.*),\\s+(?<city>.*),\\s+(?<state>.*)\\s+(?<zip>\\d{5})$";

        /// <summary>
        /// Compiled regular expression for purposes of supporting address parsing.
        /// </summary>
        private static readonly Regex PatternRegex = new Regex(Pattern, RegexOptions.Compiled);

        /// <summary>
        /// Compiled regular expression for purposes of supporting address parsing.
        /// </summary>
        private static readonly Regex Pattern2Regex = new Regex(Pattern2, RegexOptions.Compiled);

        private void ParseAddress(string text)
        {
            // Parse the address one way or the other.
            if (TryParseAddress2(text)) return;
            TryParseAddress(text);
        }

        private bool TryParseAddress(string text)
        {
            var matches = PatternRegex.Matches(text.Trim());

            if (matches.Count != 1) return false;

            var match = matches[0];

            StreetAddress = match.Groups["streetaddr"].Value;
            CityName = match.Groups["city"].Value;
            State = match.Groups["state"].Value;
            ZipCode = int.Parse(match.Groups["zip"].Value);

            return true;
        }

        private bool TryParseAddress2(string text)
        {
            var matches = Pattern2Regex.Matches(text.Trim());

            if (matches.Count != 1) return false;

            var match = matches[0];

            StreetAddress = match.Groups["streetaddr"].Value;
            SecondLine = match.Groups["secondline"].Value;
            CityName = match.Groups["city"].Value;
            State = match.Groups["state"].Value;
            ZipCode = int.Parse(match.Groups["zip"].Value);

            return true;
        }
    }

    internal class TaxCalculator
    {
        public static int Calculate(int orderAmount, string state)
        {
            const StringComparison comparisonType = StringComparison.CurrentCultureIgnoreCase;

            if (new[] {"Arizona", "AZ"}.Any(s => s.Equals(state, comparisonType)))
            {
                return orderAmount/100*5;
            }

            if (new[] {"Washington", "WA"}.Any(s => s.Equals(state, comparisonType)))
            {
                return orderAmount/100*9;
            }

            if (new[] {"California", "CA"}.Any(s => s.Equals(state, comparisonType)))
            {
                return orderAmount/100*6;
            }

            if (new[] {"Delaware", "DE"}.Any(s => s.Equals(state, comparisonType)))
            {
                return 0;
            }
            return orderAmount/100*7;
        }
    }

    internal class ShippingCalculator
    {
        public static int Calculate(int zipCode)
        {
            if (zipCode >= 75000)
            {
                return 10;
            }
            else if (zipCode >= 25000)
            {
                return 20;
            }
            else
            {
                return 30;
            }
        }
    }
}
