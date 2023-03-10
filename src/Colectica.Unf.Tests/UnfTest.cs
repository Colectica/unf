using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colectica.Unf.Tests
{
    public class UnfTest
    {

        [Fact]
        public void DateOnlyShaTests()
        {
            var list = new List<DateOnly?>
            {
                new DateOnly(2014,1,13)
            };
            string result = Unf.CalculateSha(list);
            Assert.Equal("Xb7sRkDHto7SPwO+GzVbIw==", result);
        }

        [Fact]
        public void DateOnlyNormalizedTests()
        {
            string result = Unf.GetNormalization(new DateOnly(2014, 1, 13));
            Assert.Equal("2014-01-13\n\0", result);
        }

        [Fact]
        public void TimeOnlyShaTests()
        {
            var list = new List<TimeOnly?>
            {
                new TimeOnly(20,47,18)
            };
            string result = Unf.CalculateSha(list);
            Assert.Equal("8HpYq/i5SseEAMQw1h7Wbg==", result);
        }

        [Fact]
        public void TimeOnlyNormalizedTests()
        {
            string result = Unf.GetNormalization(new TimeOnly(20, 47, 18));
            Assert.Equal("20:47:18\n\0", result);

            result = Unf.GetNormalization(new TimeOnly(20, 47, 18, 999));
            Assert.Equal("20:47:18.999\n\0", result);

            result = Unf.GetNormalization(new TimeOnly(20, 47, 18, 999, 100));
            Assert.Equal("20:47:18.9991\n\0", result);

            result = Unf.GetNormalization(new TimeOnly(20, 47, 18, 0, 10));
            Assert.Equal("20:47:18.00001\n\0", result);

            result = Unf.GetNormalization(new TimeOnly(20, 47, 18, 0, 1));
            Assert.Equal("20:47:18\n\0", result);
        }

    }
}
