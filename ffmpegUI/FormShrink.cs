using System.Diagnostics;

namespace ffmpegUI
{
    public partial class FormShrink : Form
    {
        public FormShrink()
        {
            InitializeComponent();
        }

        public static void OpenCMD(Process process)
        {
            ProcessStartInfo ffmpegInfo = new ProcessStartInfo();
            ffmpegInfo.CreateNoWindow = false;  //true to hide console window, false to show
            ffmpegInfo.UseShellExecute = false;
            ffmpegInfo.FileName = "./ff.bat";
            ffmpegInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                using (process = Process.Start(ffmpegInfo))
                {
                    process.WaitForExit();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        static async Task writeAuto(string line0)
        {
            string[] lines =
            {
                    "cd ffmpeg\\bin ",
                    line0,
                    "exit"
                };

            await System.IO.File.WriteAllLinesAsync("ff.bat", lines);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var formManual = new FormManual();
            formManual.Tag = this;
            formManual.StartPosition = FormStartPosition.Manual;
            formManual.Location = this.Location;
            formManual.Closed += (s, args) => this.Close();
            formManual.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            var formClip = new FormClip();
            formClip.Tag = this;
            formClip.StartPosition = FormStartPosition.Manual;
            formClip.Location = this.Location;
            formClip.Closed += (s, args) => this.Close();
            formClip.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (cbFolder.Checked == false)
            {
                int size = -1;
                lblComplete.Visible = false;
                OpenFileDialog openFileDialog1 = new();
                openFileDialog1.Filter = "Video Files|*.mkv;*.mp4;*.ts;*.mov;*.avi;*.m4v;*.mp4a;";
                DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK) // Test result.
                {
                    string file = openFileDialog1.FileName;
                    txtFileIn.Text = file;
                    try
                    {
                        string text = openFileDialog1.FileName;
                        FileInfo fileInf = new FileInfo(text);
                        float fileSize = fileInf.Length;
                        string tmpSize = fileSize.ToString();
                        float fileSizeKB = fileSize / 1024;
                        float fileSizeMB = (fileSize / 1024) / 1024;
                        float fileSizeGB = (fileSizeMB / 1024);
                        size = text.Length;
                        if (txtFileOut.Text != "" && cbFolder.Checked == false)
                        {
                            btnConvert.Visible = true;
                        }
                        else if (txtFileOut.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
                        {
                            btnConvert.Visible = true;
                        }
                        else
                        {
                            btnConvert.Visible = false;
                        }

                        lblSize.Visible = true;

                        if (tmpSize.Length >= 9)
                        {
                            lblSize.Text = "Size: " + fileSizeGB.ToString("n2") + " GB";
                        }
                        else if (tmpSize.Length > 5 && tmpSize.Length < 9)
                        {
                            lblSize.Text = "Size: " + fileSizeMB.ToString("n2") + " MB";
                        }
                        else
                        {
                           
                            lblSize.Text = "Size: " + fileSizeKB.ToString("n2") + " KB";
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            else
            {
                FolderBrowserDialog folderBrowserDialog1 = new();
                DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK) // Test result.
                {
                    string outputDir = folderBrowserDialog1.SelectedPath;
                    txtFileIn.Text = outputDir;
                    try
                    {
                        if (outputDir != "")
                        {
                            btnConvert.Visible = true;
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        private void btnBrowseOut_Click(object sender, EventArgs e)
        {
            lblComplete.Visible = false;
            FolderBrowserDialog folderBrowserDialog1 = new();
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string outputDir = folderBrowserDialog1.SelectedPath;
                txtFileOut.Text = outputDir;
                try
                {
                    if (txtFileIn.Text != "" && cbFolder.Checked == false)
                    {
                        btnConvert.Visible = true;
                    }
                    else if (txtFileIn.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
                    {
                        btnConvert.Visible = true;
                    }
                    else
                    {
                        btnConvert.Visible = false;
                    }
                }
                catch (IOException)
                {
                }
            }
        }

        private void FormShrink_Load(object sender, EventArgs e)
        {
            string dir = "tmp";
            Directory.CreateDirectory(dir);
            lblSize.Text = "Size: " + txtFileIn.Text;
            rbCPUEncode.Checked = true;
            rbHEVC.Checked = true;
            pbProgress.Visible = false;
            cbExtension.SelectedIndex = 0;
            ToolTip tooltipAMD = new ToolTip();
            tooltipAMD.SetToolTip(rbAMDGPU, "Select this option if your pc has an AMD Graphics card installed.");
            ToolTip tooltipNVIDIA = new ToolTip();
            tooltipNVIDIA.SetToolTip(rbNVIDIAGPU, "Select this option if your pc has a NVIDIA Graphics card installed.");
            ToolTip tooltipCPU = new ToolTip();
            tooltipCPU.SetToolTip(rbCPUEncode, "Select this option if Graphics Card is unknown, or if you simply prefer to encode with the CPU.");
            ToolTip tooltip264 = new ToolTip();
            tooltip264.SetToolTip(rb264, "Slightly less compressed than HEVC, but compatibile with most devices and playback software.");
            ToolTip tooltipHEVC = new ToolTip();
            tooltipHEVC.SetToolTip(rbHEVC, "Offers more compression without quality loss, but some devices may require an extension or a 3rd party media player application.");
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            string fileIn = txtFileIn.Text;
            string safeName = System.IO.Path.GetFileNameWithoutExtension(fileIn);
            string fileName = System.IO.Path.GetFileName(fileIn);
            string path = txtFileOut.Text + "\\";
            string dir = System.IO.Directory.GetCurrentDirectory();
            string exn = cbExtension.Text.ToString();
            if (cbFolder.Checked == false)
            {
                if (rbCPUEncode.Checked == true)
                {
                    if (rb264.Checked == true)
                    {
                        string cmd0 = $"ffmpeg.exe -y -hide_banner -i \"{fileIn}\" -fflags +genpts -c:v libx264 -crf 24 -crf_max 24 -preset veryfast -c:a copy -movflags +faststart -fps_mode vfr \"{path}{safeName}.mp4\"";
                        writeAuto(cmd0);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                    }
                    else
                    {
                        string cmd0 = $"ffmpeg.exe -y -hide_banner -i \"{fileIn}\" -fflags +genpts -c:v libx265 -crf 28 -preset veryfast -tag:v hvc1 -c:a copy -movflags +faststart -fps_mode vfr \"{path}{safeName}.mp4\"";
                        writeAuto(cmd0);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                    }
                }
                if (rbAMDGPU.Checked == true)
                {
                    if (rb264.Checked == true)
                    {
                        string cmd0 = $"ffmpeg.exe -y -hide_banner -hwaccel d3d11va -i \"{fileIn}\" -fflags +genpts -c:v h264_amf -vf mpdecimate -rc cqp -qp_i 24 -qp_p 24 -qp_b 24 -q:v 24 -c:a copy -movflags +faststart -fps_mode vfr \"{path}{safeName}.mp4\" >> log.txt";
                        writeAuto(cmd0);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                    }
                    else
                    {
                        string cmd0 = $"ffmpeg.exe -y -hide_banner -hwaccel d3d11va -i \"{fileIn}\" -fflags +genpts -c:v hevc_amf -vf mpdecimate -rc cqp -qp_i 28 -qp_p 28 -q:v 28 -tag:v hvc1 -c:a copy -movflags +faststart -fps_mode vfr \"{path}{safeName}.mp4\" >> log.txt";
                        writeAuto(cmd0);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                    }
                }
                if (rbNVIDIAGPU.Checked == true)
                {
                    if (rb264.Checked == true)
                    {
                        string cmd0 = $"cd ffmpeg/bin && ffmpeg.exe -y -hide_banner -hwaccel d3d11va -i \"{fileIn}\" -fflags +genpts -c:v h264_nvenc -rc constqp -qp 24 -preset fast -vf mpdecimate -c:a copy -movflags faststart -fps_mode vfr \"{path}{safeName}.mp4\"";
                        writeAuto(cmd0);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                    }
                    else
                    {
                        string cmd0 = $"cd ffmpeg/bin && ffmpeg.exe -y -hide_banner -hwaccel d3d11va -i \"{fileIn}\" -fflags +genpts -c:v hevc_nvenc -rc constqp -qp 24 -preset fast -vf mpdecimate -tag:v hvc1 -c:a copy -movflags faststart -fps_mode vfr \"{path}{safeName}.mp4\"";
                        writeAuto(cmd0);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                    }
                }
            }
            else if (cbFolder.Checked == true)
            {
                string getfold = "*" + cbExtension.Text.ToString();
                List<string> cmds = new List<string>();
                DirectoryInfo directory = new DirectoryInfo(txtFileIn.Text);
                foreach (var file in directory.GetFiles(getfold))
                {
                    string file1 = file.ToString();
                    string safeEach = Path.GetFileNameWithoutExtension(file1);
                    if (rbCPUEncode.Checked == true)
                    {
                       cmds.Add($"ffmpeg.exe -y -hide_banner -i \"{file}\" -fflags +genpts -c:v libx264 -crf 24 -crf_max 24 -preset veryfast -c:a copy -movflags +faststart -fps_mode vfr \"{path}{safeEach}.mp4\"");
                        writeAuto(cmds[0]);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                        cmds.Clear();
                    }
                    if (rbAMDGPU.Checked == true)
                    {
                        cmds.Add($"ffmpeg.exe -y -hide_banner -hwaccel d3d11va -i \"{file}\" -fflags +genpts -c:v h264_amf -vf mpdecimate -rc cqp -qp_i 24 -qp_p 24 -qp_b 24 -q:v 24 -c:a copy -movflags +faststart -fps_mode vfr \"{path}{safeEach}.mp4\"");
                        writeAuto(cmds[0]);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                        cmds.Clear();
                    }
                    if (rbNVIDIAGPU.Checked == true)
                    {
                        cmds.Add($"cd ffmpeg/bin && ffmpeg.exe -y -hide_banner -hwaccel d3d11va -i \"{file}\" -fflags +genpts -c:v h264_nvenc -rc constqp -qp 24 -preset fast -vf mpdecimate -c:a copy -movflags faststart -fps_mode vfr \"{path}{safeEach}.mp4\"");
                        writeAuto(cmds[0]);
                        Process ffmpeg = new Process();
                        pbProgress.Visible = true;
                        lblProgress.Visible = true;
                        btnConvert.Visible = false;
                        OpenCMD(ffmpeg);
                        ffmpeg.Close();
                        cmds.Clear();
                    }
                }
            }
            Application.EnableVisualStyles();
            while (true)
            {
                System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName("ffmpeg.exe");
                if (procs.Count() == 0)
                {
                    pbProgress.Visible = false;
                    lblProgress.Visible = false;
                    lblComplete.Visible = true;
                    break;
                }
                pbProgress.Visible = true;
                lblProgress.Visible = true;
                btnConvert.Visible = false;

            }
            //this.Close();
        }
    
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtFileIn_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFolder.Checked)
            {
                cbExtension.Visible = true;
                lblSize.Visible = false;
                lblExn.Visible = true;
            }
            else
            {
                cbExtension.Visible = false;
                lblSize.Visible = true;
                lblExn.Visible = false;
            }
            lblComplete.Visible = false;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbExtension_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbExtension.SelectedIndex != 0 && txtFileIn.Text != "" && txtFileOut.Text != "")
            {
                btnConvert.Visible = true;
            }
            else 
            {
                btnConvert.Visible = false;
            }
            lblComplete.Visible = false;
        }

        private void rbAMDGPU_CheckedChanged(object sender, EventArgs e)
        {
            lblComplete.Visible = false;
            if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == false)
            {
                btnConvert.Visible = true;
            }
            else if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
            {
                btnConvert.Visible = true;
            }
            else 
            {
                btnConvert.Visible = false;
            }
        }

        private void rbNVIDIAGPU_CheckedChanged(object sender, EventArgs e)
        {
            lblComplete.Visible = false;
            if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == false)
            {
                btnConvert.Visible = true;
            }
            else if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
            {
                btnConvert.Visible = true;
            }
            else
            {
                btnConvert.Visible = false;
            }
        }

        private void rbCPUEncode_CheckedChanged(object sender, EventArgs e)
        {
            lblComplete.Visible = false;
            if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == false)
            {
                btnConvert.Visible = true;
            }
            else if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
            {
                btnConvert.Visible = true;
            }
            else
            {
                btnConvert.Visible = false;
            }
        }

        private void rb264_CheckedChanged(object sender, EventArgs e)
        {
            lblComplete.Visible = false;
            if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == false)
            {
                btnConvert.Visible = true;
            }
            else if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
            {
                btnConvert.Visible = true;
            }
            else
            {
                btnConvert.Visible = false;
            }
        }

        private void rbHEVC_CheckedChanged(object sender, EventArgs e)
        {
            lblComplete.Visible = false;
            if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == false)
            {
                btnConvert.Visible = true;
            }
            else if (txtFileIn.Text != "" && txtFileOut.Text != "" && cbFolder.Checked == true && cbExtension.SelectedIndex != 0)
            {
                btnConvert.Visible = true;
            }
            else
            {
                btnConvert.Visible = false;
            }
        }
    }
}