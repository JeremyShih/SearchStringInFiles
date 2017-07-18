using SearchStringInFiles.Controller;
using SearchStringInFiles.Enum;
using SearchStringInFiles.Model;
using SearchStringInFiles.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchStringInFiles
{
    public partial class Form1 : Form
    {
        private Ctrl ctrl;
        private TimeRange timeSpan;
        public Form1()
        {
            InitializeComponent();
            ctrl = new Ctrl();
            btnSearch.Text = "Search";
            ReadSetting();
            comboBox1.Text = comboBox1.Items[0].ToString();
        }
        private void Start()
        {
            btnSearch.Enabled = false;
            //progressBar1.Style = ProgressBarStyle.Marquee;
            ctrl.SaveSettings(folderPath, targetString);
        }
        private void Finish()
        {
            btnSearch.Text = "Search";
            btnSearch.Enabled = true;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 0;
        }
        private string folderPath;
        private string targetString;
        private async void button1_Click(object sender, EventArgs e)
        {
            folderPath = txtPath.Text;
            targetString = txtString.Text;
            timeSpan = ctrl.SetTimeSpan(comboBox1.Text);
            try
            {
                if (string.IsNullOrEmpty(targetString) || string.IsNullOrEmpty(folderPath))
                {
                    throw new Exception("請輸入字串或指定資料夾路徑");
                }
                if (!Directory.Exists(folderPath))
                {
                    throw new Exception("指定資料夾不存在");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Start();
            DateTime startTime = DateTime.Now;
            List<string> files = new List<string>();

            progressBar1.Style = ProgressBarStyle.Marquee;
            // 等待第一個await處理完後，會更新UI，再繼續後面的程式碼
            files = await Task.Run(() => ctrl.FilesInTimeRange(folderPath, timeSpan));
            //files = await ctrl.FilesInTimeRangeAsync(folderPath, timeSpan);
            btnSearch.Text = "搜尋" + files.Count + "個檔案中";
            progressBar1.Style = ProgressBarStyle.Blocks;

            // 這個方法會等這一行做完再繼續後面的動作，等待期間UI不會鎖死
            // 每完成一個檔案的檢查，都會更新progressBar的進度
            List<string> result = await ctrl.FindStringAsync(files,
                new Progress<int>(precent => progressBar1.Value = precent), targetString);

            // 搜尋結果用bindingList顯示在DataGridView中
            SetResultFileInfos(ctrl.GetFileInfoList(result.ToArray()));

            // 搜尋完成跳出訊息視窗
            DateTime finishTime = DateTime.Now;
            double spent = finishTime.Subtract(startTime).TotalSeconds;
            Finish();
            MessageBox.Show("搜尋完成，找到" + result.Count + "個檔案\n花費" + spent.ToString("F2") + "秒");
            WriteLog(result.Count, spent);
        }
        private BindingList<ResultFileInfo> _resultFileInfos;
        public void SetResultFileInfos(BindingList<ResultFileInfo> resultFileInfos)
        {
            _resultFileInfos = resultFileInfos;
            dataGridView1.DataSource = _resultFileInfos;
        }
        private void ReadSetting()
        {
            if (!string.IsNullOrEmpty(Settings.Default.FolderPath) &&
                !string.IsNullOrEmpty(Settings.Default.TargetString))
            {
                txtPath.Text = Settings.Default.FolderPath;
                txtString.Text = Settings.Default.TargetString;
            }
        }
        private void WriteLog(int found,double timeSpent)
        {
            using (StreamWriter sw = File.AppendText(@"\\vmware-host\Shared Folders\文件\SearchStringLog.txt"))
            {
                string msg = string.Format("{0}  搜尋位置\"{1}\" 搜尋字串\"{2}\"", DateTime.Now, folderPath, targetString);
                sw.WriteLine(msg);
                msg = string.Format("    找到{0}個檔案 花費{1}秒", found, timeSpent.ToString("F2"));
                sw.WriteLine(msg);
            }
        }
        // https://stackoverflow.com/questions/19980112/how-to-do-progress-reporting-using-async-await
    }
}
