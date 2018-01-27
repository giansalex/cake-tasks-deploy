using CustomLibrary.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomLibrary.Tests.Operations
{
    [TestClass]
    public class SumTests
    {
        [TestMethod]
        public void OperationTest()
        {
            var math = new Sum();

            Assert.AreEqual(2, math.Operation(1, 1));
        }

        [TestMethod]
        public void BadOperationTest()
        {
            var math = new Sum();

            Assert.AreNotEqual(3, math.Operation(1, 1));
        }
    }
}