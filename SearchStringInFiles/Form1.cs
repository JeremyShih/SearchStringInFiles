using SearchStringInFiles.Controller;
using SearchStringInFiles.Enum;
using SearchStringInFiles.Model;
using SearchStringInFiles.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
            if (string.IsNullOrEmpty(targetString) || string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("請輸入字串或指定資料夾路徑");
                return;
            }
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("指定資料夾不存在");
                return;
            }

            Start();
            DateTime startTime = DateTime.Now;
            List<string> files;

            progressBar1.Style = ProgressBarStyle.Marquee;
            // 等待第一個await處理完後，會更新UI，再繼續後面的程式碼
            files = await Task.Run(() => ctrl.FilesInTimeRange(folderPath, timeSpan));
            btnSearch.Text = "搜尋" + files.Count + "個檔案中";
            progressBar1.Style = ProgressBarStyle.Blocks;

            // 這個方法會等這一行做完再繼續後面的動作，等待期間UI不會鎖死
            // 每完成一個檔案的檢查，都會更新progressBar的進度
            progressBar1.Minimum = 0;
            progressBar1.Maximum = files.Count;
            progressBar1.Value = 0;
            List<string> result = await ctrl.FindStringAsync(files,
                new Progress<int>(precent => progressBar1.Value = precent), targetString);

            // 搜尋結果用bindingList顯示在DataGridView中
            SetResultFileInfos(ctrl.GetFileInfoList(result.ToArray()));

            // 搜尋完成跳出訊息視窗
            DateTime finishTime = DateTime.Now;
            double spent = finishTime.Subtract(startTime).TotalSeconds;
            MessageBox.Show("搜尋完成，找到" + result.Count + "個檔案\n花費" + spent.ToString("F2") + "秒");
            Finish();
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
        private void WriteLog(int found, double timeSpent)
        {
            using (StreamWriter sw = File.AppendText(@"D:\000000\SearchStringLog.txt"))
            {
                string msg = string.Format("{0}  搜尋位置\"{1}\" 搜尋字串\"{2}\"", DateTime.Now, folderPath, targetString);
                sw.WriteLine(msg);
                msg = string.Format("    找到{0}個檔案 花費{1}秒", found, timeSpent.ToString("F2"));
                sw.WriteLine(msg);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string filePath = string.Format(@"{0}\{1}", txtPath.Text, dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                string argument = "/select, \"" + filePath + "\"";

                Process.Start("explorer.exe", argument);
            }
        }
        // https://stackoverflow.com/questions/19980112/how-to-do-progress-reporting-using-async-await
    }
}
