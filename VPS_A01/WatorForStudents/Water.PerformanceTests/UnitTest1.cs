using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Water.PerformanceTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1() {
            var random = new Random();
            var list = new Queue<int>();
            for (int i = 0; i < 1000000; i++) {
                list.Enqueue(random.Next(500 * 500));
            }
        }
    }
}
