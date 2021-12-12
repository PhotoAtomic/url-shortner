using api.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;

namespace api.Test
{
    [TestClass]
    public class SpecialEncodingTests
    {
        [TestMethod]
        public void SpecialEncodingTest()
        {
            UlongEncoding.ToSpecialBase16(0).Should().Be("");
            UlongEncoding.ToSpecialBase16(1).Should().Be("b");
            UlongEncoding.ToSpecialBase16(0x10).Should().Be("ba");
            UlongEncoding.ToSpecialBase16(0x11).Should().Be("bb");
            UlongEncoding.ToSpecialBase16(0x210).Should().Be("cba");
        }
    }
}
