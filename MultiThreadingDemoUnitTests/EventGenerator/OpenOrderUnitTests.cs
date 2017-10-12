using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MultiThreadingDemo.EventGenerator;

namespace MultiThreadingDemoUnitTests.EventGenerator
{
    [TestClass]
    public class OpenOrderUnitTests
    {
        [TestMethod]
        public void TestEventOutput()
        {
            var openOrder = OpenOrder("Ted", "SPY", 10.0m, 200, OpenOrderUnitTests, 1337);
        }
    }
}
