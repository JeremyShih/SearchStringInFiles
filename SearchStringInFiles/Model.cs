using System.ComponentModel;
using System.IO;

namespace SearchStringInFiles.Model
{
    public class ResultFileInfo
    {
        private FileInfo _fileInfo;
        public ResultFileInfo(string filePath)
        {
            _fileInfo = new FileInfo(filePath);
        }
        [DisplayName("檔名")]
        public string FileName
        {
            get
            {
                return _fileInfo.Name;
            }
        }
        [DisplayName("修改日期")]
        public string LastWriteTime
        {
            get
            {
                return _fileInfo.LastWriteTime.ToString();
            }
        }
        //public string FoundLine;
        [Browsable(false)]
        [DisplayName("")]
        public string FullPath
        {
            get
            {
                return _fileInfo.FullName;
            }
        }
    }
}
