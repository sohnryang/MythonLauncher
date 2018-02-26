using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.IO.Compression;
using System.Diagnostics;
using System.Text;

namespace Mython_Launcher
{
    public partial class MainForm : Form
    {
        static string MythonDir = Environment.CurrentDirectory + @"\Mython";
        static string serverDir = MythonDir + @"\Minecraft Tools\server\server.properties";
        static string Mython_URL = "https://www.dropbox.com/s/r7hdlyff4870j12/Mython.zip?dl=1";
        static string Mython_Path = MythonDir + ".zip";

        String[] cb1 = { "true", "false" };
        String[] cb2 = { "DEFAULT", "FLAT", "LARGEBIOMES", "AMPLIFIED" };
        String[] cb3 = { "peaceful", "easy", "normal", "difficult" };

        bool isOpen = false;
        bool isDonwload = false;

        public MainForm()
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 500;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(cb1);
            comboBox2.Items.AddRange(cb2);
            comboBox3.Items.AddRange(cb3);
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
            comboBox3.SelectedIndex = 0;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DirectoryInfo DI = new DirectoryInfo(MythonDir);
            if (DI.Exists)
            {
                if (isDonwload != true)
                {
                    statusLbl.ForeColor = System.Drawing.Color.LimeGreen;
                    statusLbl.Text = "폴더 존재";
                }
            }
            else
            {
                btnOpen.Enabled = false;
                btnLaunch.Enabled = false;
                btnConfig.Enabled = false;
                btnInstall.Enabled = false;

                if (isDonwload != true)
                {
                    statusLbl.ForeColor = System.Drawing.Color.Red;
                    statusLbl.Text = "폴더 없음";
                }
                if (isOpen == false)
                {
                    isOpen = true;

                    if (MessageBox.Show("폴더가 존재하지 않습니다. 파일을 다운받으시겠습니까?",
                        "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        // 파일 다운로드
                        DI.Create();
                        fileDownload();
                        isOpen = false;
                    }
                }
            }
        }

        void fileDownload()
        {
            WebClient wc = new WebClient();
            wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(downCompleted);
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(progressChanged);
            wc.DownloadFileAsync(new Uri(Mython_URL), Mython_Path);
        }

        private void progressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            isDonwload = true;
            progressBar.Value = e.ProgressPercentage;
            statusLbl.ForeColor = System.Drawing.Color.Black;
            statusLbl.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void downCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ZipFile.ExtractToDirectory(Mython_Path, MythonDir);
            statusLbl.Text = "압축 푸는중...";
            statusLbl.ForeColor = System.Drawing.Color.Black;
            FileInfo fi = new FileInfo(Mython_Path);
            DirectoryInfo di = new DirectoryInfo(MythonDir);
            fi.Delete();
            if (di.Exists == true)
            {
                MessageBox.Show("다운로드가 완료되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            btnOpen.Enabled = true;
            btnLaunch.Enabled = true;
            btnConfig.Enabled = true;
            btnInstall.Enabled = true;
            isDonwload = false;
        }

        private void OpenFile(string dir)
        {
            try
            {
                Process.Start(dir);
            }
            catch (Win32Exception win32)
            {
                MessageBox.Show(win32.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Mython 폴더를 지우고 다시 실행해 주세요", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenFile(string dir, string arg)
        {
            Process ps = new Process();
            try
            {
                ps.StartInfo.FileName = dir;
                ps.StartInfo.RedirectStandardInput = true;
                ps.StartInfo.RedirectStandardOutput = true;
                ps.StartInfo.CreateNoWindow = true;
                ps.StartInfo.UseShellExecute = false;
                ps.Start();
                ps.StandardInput.WriteLine(arg);
                ps.StandardInput.Close();
                ps.WaitForExit();
            }
            catch (Win32Exception win32)
            {
                MessageBox.Show(win32.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Mython 폴더를 지우고 다시 실행해 주세요", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFile("CMD.exe", @"cd Mython\Minecraft Tools\server && start start.bat"); // Server
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            OpenFile("CMD.exe", @"cd Mython && start run.bat"); // Minecraft
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            OpenFile(MythonDir + @"\Minecraft Tools\server\server.properties"); // Server config
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            OpenFile(MythonDir + @"\Python\WinPython Interpreter"); // Python
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // http://infodbbase.tistory.com/113 참조
            try
            {
                FileHandler("online-mode=", comboBox1.Text);
            }
            catch { }
        }

        private void FileHandler(string key, string value)
        {
            string[] textValue = File.ReadAllLines(serverDir);
            string temp = "";
            foreach (string s in textValue)
            {
                string t = s;
                if (t.Contains(key))
                {
                    t = key + value;
                }
                temp += t + Environment.NewLine;
            }
            File.WriteAllText(serverDir, temp, Encoding.Default);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FileHandler("level-type=", comboBox2.Text);
            }
            catch { }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == cb3[0])
                    FileHandler("difficulty=", "0");
                else if (comboBox3.Text == cb3[1])
                    FileHandler("difficulty=", "1");
                else if (comboBox3.Text == cb3[2])
                    FileHandler("difficulty=", "2");
                else if (comboBox3.Text == cb3[3])
                    FileHandler("difficulty=", "3");
            }
            catch { }
        }
    }
}
