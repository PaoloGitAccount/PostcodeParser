using System;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Postcode.Tests
{
    //    [Theory]
    //    [InlineData(null)]
    //    [InlineData("")]
    //    [InlineData("B")]
    //    [InlineData("BN")]
    //    [InlineData("BN6")]
    //    [InlineData("BN6 8")]
    //    [InlineData("BN6 8B")]
    //    [InlineData("BN66 8B")]
    //    [InlineData("B66 8B")]
    //    [InlineData("B6 8B")]
    //    [InlineData("B6 8BAB")]
    //    [InlineData("M287JP")]
    //    [InlineData("M27JP")]
    //    public void TestInvalid(string s)
    //    {
    //        Assert.False(Postcode.TryParse(s, out var postcode));
    //        Assert.Equal(default(Postcode), postcode);
    //    }
 
    public class PostcodeTests
    {
        [Theory]
        [InlineData("M28 7JP", "M", "28", 7, "JP")]
        [InlineData("WC2H 7DE", "WC", "2H", 7, "DE")]
        [InlineData("CT21 4LR", "CT", "21", 4, "LR")]
        [InlineData("N3 3DP", "N", "3", 3, "DP")]
        [InlineData("M1 1AA", "M", "1", 1, "AA")]
        [InlineData("M60 1NW", "M", "60", 1, "NW")]
        [InlineData("W1A 1HQ", "W", "1A", 1, "HQ")]
        [InlineData("CR2 6XH", "CR", "2", 6, "XH")]
        [InlineData("DN55 1PT", "DN", "55", 1, "PT")]
        [InlineData("W1P 1BB", "W", "1P", 1, "BB")]
        [InlineData("EC1A 1BB", "EC", "1A", 1, "BB")]
        [InlineData("M11AA", "M", "1", 1, "AA")]
        [InlineData("M601NW", "M", "60", 1, "NW")]
        [InlineData("CR26XH", "CR", "2", 6, "XH")]
        [InlineData("DN551PT", "DN", "55", 1, "PT")]
        [InlineData("W1P1BB", "W", "1P", 1, "BB")]
        [InlineData("EC1A1BB", "EC", "1A", 1, "BB")]
    //    [InlineData("BN6 8", "", "", 1, "")]
        public void TestValid(string postcode, string area, string district, int sector, string unit)
        {
            TestParsing(postcode, area, district, sector, unit);
        }

        [Theory]
        //[InlineData(null)]
        //[InlineData("")]
        //[InlineData("B", "", "", 1, "")]
        //[InlineData("BN6", "", "", 1, "")]
        //[InlineData("BN6 8", "", "", 1, "")]
        //[InlineData("BN6 8B", "", "", 1, "")]
        [InlineData("B6 8BAB")]
        //[InlineData("M287JP")]
        //[InlineData("M27JP")]
        public void TestInvalid(string postcode)//, string area, string district, int sector, string unit)
        {
            var result = Postcode.IsValid(postcode);
            Assert.False(result);

            //TestParsing(postcode, area, district, sector, unit);
        }

        private static void TestParsing(string postcode, string area, string district, int sector, string unit)
        {
            var result = Postcode.Destructure(postcode);
            Assert.Equal(area, result.Outward.Area);
            Assert.Equal(district, result.Outward.District);
            Assert.Equal(sector, result.Inward.Sector);
            Assert.Equal(unit, result.Inward.Unit);
        }

        [Theory]
        [InlineData("BS98 1TL", "BS", "98", 1, "TL")]
        [InlineData("W1N 4DJ", "W", "1N", 4, "DJ")]
        public void SpecialPostcodes(string postcode, string area, string district, int sector, string unit)
        {
            TestParsing(postcode, area, district, sector, unit);
        }

        [Fact]
        public void ThrowsOnInvalidCode()
        {
            Assert.Throws<ArgumentException>(() => Postcode.Destructure("111 111"));
        }

        [Fact]
        public void TestAllValids_fromFile()
        {
            foreach (var line in File.ReadLines(Directory.GetCurrentDirectory() + @"..\..\..\..\Data\postcodes.txt"))
            {
                var postcode = Postcode.Destructure(line);
                Assert.Equal(line, postcode.ToString());
            }
        }

        [Fact]
        public void FailsNullPostcode()
        {
            var result = Postcode.IsValid(null);
            Assert.False(result);
        }

        [Fact]
        public void FailEmptyPostcode()
        {
            var result = Postcode.IsValid("");
            Assert.False(result);
        }

        [Fact]
        public void FailsWhitespacePostcode()
        {
            var result = Postcode.IsValid(null);
            Assert.False(result);
        }

        [Fact]
        public void FailsAllNumbers()
        {
            var result = Postcode.IsValid("111 111");
            Assert.False(result);
        }

        [Fact]
        public void FailsAllText()
        {
            var result = Postcode.IsValid("AAA AAA");
            Assert.False(result);
        }

        [Fact]
        public void FailsInvalidArea()
        {
            var result = Postcode.IsValid("11W 0NY");
            Assert.False(result);
        }

        [Fact]
        public void FailsInvalidDistrict()
        {
            var result = Postcode.IsValid("SWWW 0NY");
            Assert.False(result);
        }
    }
}
