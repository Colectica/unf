using System;

namespace Colectica.Unf.Tests
{
    public class PredefinedTest
    {
        [Theory]
        [InlineData(null, "cJ6AyISHokEeHuTfufIqhg==")]
        [InlineData(0d, "YUvj33xEHnzirIHQyZaHow==")]
        [InlineData(1d, "tv3XYCv524AfmlFyVOhuZg==")]
        [InlineData(-300d, "ZTXyg54FoMfRDWZl6oWmFQ==")]
        [InlineData(3.1415, "vOSZmXXXpKfQcqZ0Cuu5/w==")]
        [InlineData(0.00073, "qhw3qzg3fEK0NNfoVxk4jQ==")]
        [InlineData(1.23456789, "vcKELUSS4s4k1snF4OTB9A==")]
        [InlineData(1.23456789, "IKw+l4ywdwsJeDze8dplJA==", 9)]
        [InlineData(double.NaN, "GNcR8/UCnImaPpw47gdPNg==")]
        [InlineData(double.PositiveInfinity, "MdAI70WZdDHnu6qmkpqUQg==")]
        [InlineData(double.NegativeInfinity, "A7orv3pgAhljFnGjQVLCog==")]
        public void UnfPublishedNumbersShaTests(double? d, string truncatedSha256, int precision = 7)
        {
            var list = new List<double?>() { d };

            string result = Unf.CalculateSha(list, precision);
            Assert.Equal(truncatedSha256, result);
        }

        [Fact]
        public void UnfPublishedNumbersVectorShaTests()
        {
            var list = new List<double?> { 1.23456789, null, 0 };
            string result = Unf.CalculateSha(list);
            Assert.Equal("Do5dfAoOOFt4FSj0JcByEw==", result);
        }

        [Fact]
        public void UnfPublishedDateOffsetTimeShaTests()
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            TimeSpan offset = tzi.BaseUtcOffset;

            var list = new List<DateTimeOffset?> 
            { 
                new DateTimeOffset(2014,1,13,20,47,18, offset),
            };
            string result = Unf.CalculateSha(list);
            Assert.Equal("1Pku/Z/EIRtmpdEepAb1MA==", result);
        }
        [Fact]
        public void UnfPublishedDateTimeOffsetNormalizedTests()
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            TimeSpan offset = tzi.BaseUtcOffset;
            var result = Unf.GetNormalization(new DateTimeOffset(2014, 1, 13, 20, 47, 18, offset));
            Assert.Equal("2014-01-14T01:47:18Z\n\0", result);
        }

        [Fact]
        public void UnfPublishedDateTimeShaTests()
        {
            var list = new List<DateTime?>
            {
                new DateTime(2014,1,13,20,47,18,DateTimeKind.Unspecified),
            };
            string result = Unf.CalculateSha(list);
            Assert.Equal("eaMxex5EHi2LunomVc0SDw==", result);
        }
        [Fact]
        public void UnfPublishedDateTimeNormalizedTests()
        {
            var result = Unf.GetNormalization(new DateTime(2014, 1, 13, 20, 47, 18, DateTimeKind.Unspecified));
            Assert.Equal("2014-01-13T20:47:18\n\0", result);
        }

        [Theory]
        [InlineData("A quite long character string, so long that the number of characters in it happens to be more than the default cutoff limit of 128.", "/BoSlfcIlsmQ+GHu5gxwEw==")]
        [InlineData("på Færøerne", "KHM6bKVaVaxWDDsmyerfDA==")]
        [InlineData("", "ECtRuXZaVqPomffPDuOOUg==")]
        [InlineData(null, "cJ6AyISHokEeHuTfufIqhg==")]
        public void UnfPublishedStringsShaTests(string s, string truncatedSha256)
        {
            var list = new List<string>() { s };

            string result = Unf.CalculateSha(list);
            Assert.Equal(truncatedSha256, result);
        }

        [Theory]
        [InlineData(true, "tv3XYCv524AfmlFyVOhuZg==")]
        [InlineData(false, "YUvj33xEHnzirIHQyZaHow==")]
        [InlineData(null, "cJ6AyISHokEeHuTfufIqhg==")]
        public void UnfPublishedBooleanssShaTests(bool? b, string truncatedSha256)
        {
            var list = new List<bool?>() { b };

            string result = Unf.CalculateSha(list);
            Assert.Equal(truncatedSha256, result);
        }



        [Theory]
        [InlineData(null, "\0\0\0")]
        [InlineData(0d, "+0.e+\n\0")]
        [InlineData(-0.0, "-0.e+\n\0")]
        [InlineData(1d, "+1.e+\n\0")]
        [InlineData(-300d, "-3.e+2\n\0")]
        [InlineData(3.1415, "+3.1415e+\n\0")]
        [InlineData(0.00073, "+7.3e-4\n\0")]
        //[InlineData(1.2345675, "+1.234568e+1\n\0")]
        //[InlineData(1.2345685, "+1.234568e+1\n\0")]
        [InlineData(1.23456789, "+1.234568e+\n\0")]
        [InlineData(double.NaN, "+nan\n\0")]
        [InlineData(double.PositiveInfinity, "+inf\n\0")]
        [InlineData(double.NegativeInfinity, "-inf\n\0")]
        public void UnfPublishedNumbersNormalizedTests(double? d, string normalized)
        {
            var result = Unf.GetNormalization(d);
            Assert.Equal(normalized, result);
        }

        [Theory]
        [InlineData(null, "\0\0\0")]
        [InlineData("A character String", "A character String\n\0")]
        [InlineData("A quite long character string, so long that the number of characters in it happens to be more than the default cutoff limit of 128.",
            "A quite long character string, so long that the number of characters in it happens to be more than the default cutoff limit of 1\n\0")]
        [InlineData("", "\n\0")]
        public void UnfPublishedStringsNormalizedTests(string s, string normalized)
        {
            var result = Unf.GetNormalization(s);
            Assert.Equal(normalized, result);
        }

        [Theory]
        [InlineData(null, "\0\0\0")]
        [InlineData(true, "+1.e+\n\0")]
        [InlineData(false, "+0.e+\n\0")]
        public void UnfPublishedBooleansNormalizedTests(bool? b, string normalized)
        {
            var result = Unf.GetNormalization(b);
            Assert.Equal(normalized, result);
        }
    }
}