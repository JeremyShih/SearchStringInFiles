using NUnit.Framework;
using SearchStringInFiles.Controller;
using SearchStringInFiles.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchStringInFiles.Controller.Tests
{
    [TestFixture()]
    public class CtrlTests
    {
        Ctrl ctrl = new Ctrl();
        [Test()]
        public void InTimeRangeTest()
        {
            string filePath = @"";
            TimeRange tr = TimeRange.All;
            var expected = true;

            var actual = ctrl.InTimeRange(filePath, tr);

            Assert.AreEqual(expected, actual);
        }
    }
}