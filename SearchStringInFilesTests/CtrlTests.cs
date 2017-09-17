using NUnit.Framework;
using SearchStringInFiles.Controller;
using SearchStringInFiles.Enum;
using SearchStringInFilesTests.Stub;
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
        public void InTimeRangeTest_TimeRangeAll()
        {
            string filePath = @"\\vmware-host\Shared Folders\文件\JMS-Feedback\feedback_24600.xml";
            TimeRange tr = TimeRange.All;
            var expected = true;

            var actual = ctrl.InTimeRange(filePath, tr);

            Assert.AreEqual(expected, actual);
        }
        
        [Test()]
        public void InTimeRangeTest_TimeRangeOneMonth()
        {
            string filePath = @"";
            TimeRange tr = TimeRange.OneMonth;
            var expected = true;

            ctrl = new CtrlStub();
            var actual = ctrl.InTimeRange(filePath, tr);

            Assert.AreEqual(expected, actual);
        }
    }
}