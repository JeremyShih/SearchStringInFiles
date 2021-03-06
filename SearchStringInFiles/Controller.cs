﻿using SearchStringInFiles.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using SearchStringInFiles.Enum;
using SearchStringInFiles.Properties;
using System.Threading.Tasks;
using System.Linq;

namespace SearchStringInFiles.Controller
{
    public class Ctrl
    {
        public void SearchFolder(string[] filePaths, string targetString, out string[] results)
        {
            results = null;
            List<string> res = new List<string>();
            foreach (string filePath in filePaths)
            {
                if (findStringStream(filePath, targetString))
                {
                    res.Add(filePath);
                }
            }
            results = res.ToArray();
        }
        public List<string> SearchFolder(string folderPath, string targetString)
        {
            string[] folderFiles = Directory.GetFiles(folderPath);
            List<string> res = new List<string>();
            foreach (string filePath in folderFiles)
            {
                if (findStringStream(filePath, targetString))
                {
                    res.Add(filePath);
                }
            }

            return res;
        }
        public List<string> SearchFolder(List<string> files, string targetString)
        {
            List<string> res = new List<string>();
            foreach (string filePath in files)
            {
                if (findStringStream(filePath, targetString))
                {
                    res.Add(filePath);
                }
            }

            return res;
        }
        public async Task<List<string>> FindStringAsync(List<string> fileList, IProgress<int> progress, string targetString)
        {
            List<string> result = new List<string>();
            // await the processing and searching logic here
            await Task.Run(() =>
            {
                int tempCount = 0;
                foreach (var filePath in fileList)
                {
                    if (findStringStream(filePath, targetString))
                    {
                        result.Add(filePath);
                    }
                    if (progress != null)
                    {
                        progress.Report(tempCount + 1);
                    }
                    tempCount++;
                }
                return tempCount;
            });
            return result;
        }

        private bool findStringStream(string filePath, string targetString)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line = sr.ReadLine();
                do
                {
                    if (line.Contains(targetString))
                        return true;
                    line = sr.ReadLine();
                } while (!string.IsNullOrEmpty(line));
            }
            return false;
        }
        private bool findString(string filePath, string targetString)
        {
            string content = File.ReadAllText(filePath);
            if (content.Contains(targetString))
                return true;
            else
                return false;
        }
        public BindingList<ResultFileInfo> GetFileInfoList(string[] filePaths)
        {
            BindingList<ResultFileInfo> bindingList = new BindingList<ResultFileInfo>();

            foreach (string filePath in filePaths)
            {
                bindingList.Add(new ResultFileInfo(filePath));
            }
            return bindingList;
        }
        public string ReturnEx(Exception ex)
        {
            string retStr = ex.Message;
#if DEBUG
            retStr += "\n" + ex.ToString();
#endif
            return retStr;
        }
        public virtual DateTime GetFileTime(string filePath)
        {
            return File.GetLastWriteTime(filePath);
        }
        public bool InTimeRange(string filePath, TimeRange tr)
        {
            if (tr == TimeRange.All)
            {
                return true;
            }
            DateTime today = DateTime.Now;
            DateTime file = GetFileTime(filePath);
            double timeRange = today.Subtract(file).TotalDays;

            switch (tr)
            {
                case TimeRange.OneWeek:
                    if (timeRange > 7) return false;
                    break;
                case TimeRange.TwoWeek:
                    if (timeRange > 14) return false;
                    break;
                case TimeRange.OneMonth:
                    if (timeRange > 30) return false;
                    break;
            }
            return true;
        }
        public async Task<List<string>> FilesInTimeRangeAsync(string folderPath, TimeRange tr)
        {
            string[] files = Directory.GetFiles(folderPath);
            List<string> result = new List<string>();
            foreach (string filePath in files)
            {
                if (await Task.Run(() => InTimeRange(filePath, tr)))
                {
                    result.Add(filePath);
                }
            }
            return result;
        }
        public List<string> FilesInTimeRange(string folderPath, TimeRange tr)
        {
            string[] files = Directory.GetFiles(folderPath);
            var result = from path in files where InTimeRange(path, tr) select path;
            return result.ToList();
        }
        public void SaveSettings(string folderPath, string targetString)
        {
            Settings.Default.FolderPath = folderPath;
            Settings.Default.TargetString = targetString;
            Settings.Default.Save();
        }
        public TimeRange SetTimeSpan(string timeSpan)
        {
            switch (timeSpan)
            {
                default:
                    return TimeRange.All;
                case "一周內":
                    return TimeRange.OneWeek;
                case "兩周內":
                    return TimeRange.TwoWeek;
                case "一個月內":
                    return TimeRange.OneMonth;
            }
        }

    }
}
