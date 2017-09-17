using SearchStringInFiles.Controller;
using System;
using System.IO;

namespace SearchStringInFilesTests.Stub
{
    class CtrlStub : Ctrl
    {
        public override DateTime GetFileTime(string filePath)
        {
            if (!File.Exists(filePath))
                return DateTime.Now.AddDays(-7);
            return base.GetFileTime(filePath);
        }
    }
}
