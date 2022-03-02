namespace SampleApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.videoStub = new System.Windows.Forms.Label();
            this.videoName = new System.Windows.Forms.Label();
            this.videoBps = new System.Windows.Forms.Label();
            this.videoTotal = new System.Windows.Forms.Label();
            this.videoPause = new System.Windows.Forms.Button();
            this.videoCurrent = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.audioStub = new System.Windows.Forms.Label();
            this.audioName = new System.Windows.Forms.Label();
            this.audioBps = new System.Windows.Forms.Label();
            this.audioTotal = new System.Windows.Forms.Label();
            this.audioPause = new System.Windows.Forms.Button();
            this.audioCurrent = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.biologicalBps = new System.Windows.Forms.Label();
            this.biologicalTotal = new System.Windows.Forms.Label();
            this.biologicalPause = new System.Windows.Forms.Button();
            this.biologicalCurrent = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.deflate1Compression = new System.Windows.Forms.Label();
            this.deflate1Bps = new System.Windows.Forms.Label();
            this.deflate1Total = new System.Windows.Forms.Label();
            this.deflate1Pause = new System.Windows.Forms.Button();
            this.deflate1Current = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.shuffleCompression = new System.Windows.Forms.Label();
            this.shuffleBps = new System.Windows.Forms.Label();
            this.shuffleTotal = new System.Windows.Forms.Label();
            this.shufflePause = new System.Windows.Forms.Button();
            this.shuffleCurrent = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.bufferTotal = new System.Windows.Forms.Label();
            this.bufferCurrent = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.testerPause = new System.Windows.Forms.Button();
            this.testerSuccessRate = new System.Windows.Forms.Label();
            this.testerBps = new System.Windows.Forms.Label();
            this.testerTotal = new System.Windows.Forms.Label();
            this.testerCurrent = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.hashCompression = new System.Windows.Forms.Label();
            this.hashBps = new System.Windows.Forms.Label();
            this.hashTotal = new System.Windows.Forms.Label();
            this.hashPause = new System.Windows.Forms.Button();
            this.hashCurrent = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.deflate2Compression = new System.Windows.Forms.Label();
            this.deflate2Bps = new System.Windows.Forms.Label();
            this.deflate2Total = new System.Windows.Forms.Label();
            this.deflate2Pause = new System.Windows.Forms.Button();
            this.deflate2Current = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxRandomExcursionsVariant = new System.Windows.Forms.CheckBox();
            this.checkBoxRandomExcursions = new System.Windows.Forms.CheckBox();
            this.checkBoxCumulativeSums = new System.Windows.Forms.CheckBox();
            this.checkBoxApproximateEntropy = new System.Windows.Forms.CheckBox();
            this.checkBoxSerial = new System.Windows.Forms.CheckBox();
            this.checkBoxLinearComplexity = new System.Windows.Forms.CheckBox();
            this.checkBoxMaurersUniversal = new System.Windows.Forms.CheckBox();
            this.checkBoxOverlappingTemplateMatchings = new System.Windows.Forms.CheckBox();
            this.checkBoxNonOverlappingTemplateMatchings = new System.Windows.Forms.CheckBox();
            this.checkBoxDft = new System.Windows.Forms.CheckBox();
            this.checkBoxBinaryMatrixRank = new System.Windows.Forms.CheckBox();
            this.checkBoxLongestRunOfOnes = new System.Windows.Forms.CheckBox();
            this.checkBoxRuns = new System.Windows.Forms.CheckBox();
            this.checkBoxBlockFrequency = new System.Windows.Forms.CheckBox();
            this.checkBoxFrequency = new System.Windows.Forms.CheckBox();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.lastResultFrequency = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lastResultBlockFrequency = new System.Windows.Forms.Label();
            this.lastResultRuns = new System.Windows.Forms.Label();
            this.lastResultOverlappingTemplateMatchings = new System.Windows.Forms.Label();
            this.lastResultMaurersUniversal = new System.Windows.Forms.Label();
            this.lastResultLinearComplexity = new System.Windows.Forms.Label();
            this.lastResultSerial = new System.Windows.Forms.Label();
            this.lastResultApproximateEntropy = new System.Windows.Forms.Label();
            this.lastResultCumulativeSums = new System.Windows.Forms.Label();
            this.lastResultRandomExcursions = new System.Windows.Forms.Label();
            this.lastResultRandomExcursionsVariant = new System.Windows.Forms.Label();
            this.lastResultLongestRunOfOnes = new System.Windows.Forms.Label();
            this.lastResultBinaryMatrixRank = new System.Windows.Forms.Label();
            this.lastResultDft = new System.Windows.Forms.Label();
            this.lastResultNonOverlappingTemplateMatchings = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lastTestReport = new System.Windows.Forms.RichTextBox();
            this.bufferCurrentBar = new SampleApp.VerticalProgressBar();
            this.testerCurrentBar = new SampleApp.VerticalProgressBar();
            this.hashCurrentBar = new SampleApp.VerticalProgressBar();
            this.deflate2CurrentBar = new SampleApp.VerticalProgressBar();
            this.deflate1CurrentBar = new SampleApp.VerticalProgressBar();
            this.biologicalCurrentBar = new SampleApp.VerticalProgressBar();
            this.shuffleCurrentBar = new SampleApp.VerticalProgressBar();
            this.audioCurrentBar = new SampleApp.VerticalProgressBar();
            this.videoCurrentBar = new SampleApp.VerticalProgressBar();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.type = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.generate = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.results = new System.Windows.Forms.RichTextBox();
            this.lowerBound = new System.Windows.Forms.TextBox();
            this.upperBound = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.count = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.videoStub);
            this.groupBox1.Controls.Add(this.videoName);
            this.groupBox1.Controls.Add(this.videoBps);
            this.groupBox1.Controls.Add(this.videoTotal);
            this.groupBox1.Controls.Add(this.videoPause);
            this.groupBox1.Controls.Add(this.videoCurrent);
            this.groupBox1.Controls.Add(this.videoCurrentBar);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(179, 141);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera";
            // 
            // videoStub
            // 
            this.videoStub.BackColor = System.Drawing.Color.LightGray;
            this.videoStub.Location = new System.Drawing.Point(6, 16);
            this.videoStub.Name = "videoStub";
            this.videoStub.Size = new System.Drawing.Size(173, 122);
            this.videoStub.TabIndex = 12;
            this.videoStub.Text = "Waiting...";
            this.videoStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // videoName
            // 
            this.videoName.AutoSize = true;
            this.videoName.Location = new System.Drawing.Point(6, 20);
            this.videoName.Name = "videoName";
            this.videoName.Size = new System.Drawing.Size(0, 13);
            this.videoName.TabIndex = 8;
            // 
            // videoBps
            // 
            this.videoBps.AutoSize = true;
            this.videoBps.Location = new System.Drawing.Point(5, 94);
            this.videoBps.Name = "videoBps";
            this.videoBps.Size = new System.Drawing.Size(108, 13);
            this.videoBps.TabIndex = 6;
            this.videoBps.Text = "Bytes per second: 0B";
            // 
            // videoTotal
            // 
            this.videoTotal.AutoSize = true;
            this.videoTotal.Location = new System.Drawing.Point(6, 72);
            this.videoTotal.Name = "videoTotal";
            this.videoTotal.Size = new System.Drawing.Size(78, 13);
            this.videoTotal.TabIndex = 5;
            this.videoTotal.Text = "Total bytes: 0B";
            // 
            // videoPause
            // 
            this.videoPause.Location = new System.Drawing.Point(6, 46);
            this.videoPause.Name = "videoPause";
            this.videoPause.Size = new System.Drawing.Size(75, 23);
            this.videoPause.TabIndex = 5;
            this.videoPause.Text = "Start";
            this.videoPause.UseVisualStyleBackColor = true;
            this.videoPause.Click += new System.EventHandler(this.videoPause_Click);
            // 
            // videoCurrent
            // 
            this.videoCurrent.Location = new System.Drawing.Point(117, 121);
            this.videoCurrent.Name = "videoCurrent";
            this.videoCurrent.Size = new System.Drawing.Size(56, 17);
            this.videoCurrent.TabIndex = 5;
            this.videoCurrent.Text = "0B";
            this.videoCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.audioStub);
            this.groupBox2.Controls.Add(this.audioName);
            this.groupBox2.Controls.Add(this.audioBps);
            this.groupBox2.Controls.Add(this.audioTotal);
            this.groupBox2.Controls.Add(this.audioPause);
            this.groupBox2.Controls.Add(this.audioCurrent);
            this.groupBox2.Controls.Add(this.audioCurrentBar);
            this.groupBox2.Location = new System.Drawing.Point(6, 153);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(179, 141);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Audio";
            // 
            // audioStub
            // 
            this.audioStub.BackColor = System.Drawing.Color.LightGray;
            this.audioStub.Location = new System.Drawing.Point(6, 16);
            this.audioStub.Name = "audioStub";
            this.audioStub.Size = new System.Drawing.Size(173, 122);
            this.audioStub.TabIndex = 13;
            this.audioStub.Text = "Waiting...";
            this.audioStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // audioName
            // 
            this.audioName.AutoSize = true;
            this.audioName.Location = new System.Drawing.Point(6, 20);
            this.audioName.Name = "audioName";
            this.audioName.Size = new System.Drawing.Size(0, 13);
            this.audioName.TabIndex = 7;
            // 
            // audioBps
            // 
            this.audioBps.AutoSize = true;
            this.audioBps.Location = new System.Drawing.Point(5, 94);
            this.audioBps.Name = "audioBps";
            this.audioBps.Size = new System.Drawing.Size(108, 13);
            this.audioBps.TabIndex = 6;
            this.audioBps.Text = "Bytes per second: 0B";
            // 
            // audioTotal
            // 
            this.audioTotal.AutoSize = true;
            this.audioTotal.Location = new System.Drawing.Point(6, 72);
            this.audioTotal.Name = "audioTotal";
            this.audioTotal.Size = new System.Drawing.Size(78, 13);
            this.audioTotal.TabIndex = 5;
            this.audioTotal.Text = "Total bytes: 0B";
            // 
            // audioPause
            // 
            this.audioPause.Location = new System.Drawing.Point(6, 46);
            this.audioPause.Name = "audioPause";
            this.audioPause.Size = new System.Drawing.Size(75, 23);
            this.audioPause.TabIndex = 5;
            this.audioPause.Text = "Start";
            this.audioPause.UseVisualStyleBackColor = true;
            this.audioPause.Click += new System.EventHandler(this.audioPause_Click);
            // 
            // audioCurrent
            // 
            this.audioCurrent.Location = new System.Drawing.Point(117, 121);
            this.audioCurrent.Name = "audioCurrent";
            this.audioCurrent.Size = new System.Drawing.Size(56, 17);
            this.audioCurrent.TabIndex = 5;
            this.audioCurrent.Text = "0B";
            this.audioCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.biologicalBps);
            this.groupBox3.Controls.Add(this.biologicalTotal);
            this.groupBox3.Controls.Add(this.biologicalPause);
            this.groupBox3.Controls.Add(this.biologicalCurrent);
            this.groupBox3.Controls.Add(this.biologicalCurrentBar);
            this.groupBox3.Location = new System.Drawing.Point(6, 300);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(179, 141);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Biological";
            // 
            // biologicalBps
            // 
            this.biologicalBps.AutoSize = true;
            this.biologicalBps.Location = new System.Drawing.Point(5, 67);
            this.biologicalBps.Name = "biologicalBps";
            this.biologicalBps.Size = new System.Drawing.Size(108, 13);
            this.biologicalBps.TabIndex = 6;
            this.biologicalBps.Text = "Bytes per second: 0B";
            // 
            // biologicalTotal
            // 
            this.biologicalTotal.AutoSize = true;
            this.biologicalTotal.Location = new System.Drawing.Point(6, 45);
            this.biologicalTotal.Name = "biologicalTotal";
            this.biologicalTotal.Size = new System.Drawing.Size(78, 13);
            this.biologicalTotal.TabIndex = 5;
            this.biologicalTotal.Text = "Total bytes: 0B";
            // 
            // biologicalPause
            // 
            this.biologicalPause.Location = new System.Drawing.Point(6, 19);
            this.biologicalPause.Name = "biologicalPause";
            this.biologicalPause.Size = new System.Drawing.Size(75, 23);
            this.biologicalPause.TabIndex = 5;
            this.biologicalPause.Text = "Start";
            this.biologicalPause.UseVisualStyleBackColor = true;
            this.biologicalPause.Click += new System.EventHandler(this.biologicalPause_Click);
            // 
            // biologicalCurrent
            // 
            this.biologicalCurrent.Location = new System.Drawing.Point(117, 121);
            this.biologicalCurrent.Name = "biologicalCurrent";
            this.biologicalCurrent.Size = new System.Drawing.Size(56, 17);
            this.biologicalCurrent.TabIndex = 5;
            this.biologicalCurrent.Text = "0B";
            this.biologicalCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.deflate1Compression);
            this.groupBox4.Controls.Add(this.deflate1Bps);
            this.groupBox4.Controls.Add(this.deflate1Total);
            this.groupBox4.Controls.Add(this.deflate1Pause);
            this.groupBox4.Controls.Add(this.deflate1Current);
            this.groupBox4.Controls.Add(this.deflate1CurrentBar);
            this.groupBox4.Location = new System.Drawing.Point(191, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(179, 141);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Deflate";
            // 
            // deflate1Compression
            // 
            this.deflate1Compression.AutoSize = true;
            this.deflate1Compression.Location = new System.Drawing.Point(6, 91);
            this.deflate1Compression.Name = "deflate1Compression";
            this.deflate1Compression.Size = new System.Drawing.Size(102, 13);
            this.deflate1Compression.TabIndex = 7;
            this.deflate1Compression.Text = "Compression ratio: 0";
            // 
            // deflate1Bps
            // 
            this.deflate1Bps.AutoSize = true;
            this.deflate1Bps.Location = new System.Drawing.Point(5, 68);
            this.deflate1Bps.Name = "deflate1Bps";
            this.deflate1Bps.Size = new System.Drawing.Size(108, 13);
            this.deflate1Bps.TabIndex = 6;
            this.deflate1Bps.Text = "Bytes per second: 0B";
            // 
            // deflate1Total
            // 
            this.deflate1Total.AutoSize = true;
            this.deflate1Total.Location = new System.Drawing.Point(6, 46);
            this.deflate1Total.Name = "deflate1Total";
            this.deflate1Total.Size = new System.Drawing.Size(78, 13);
            this.deflate1Total.TabIndex = 5;
            this.deflate1Total.Text = "Total bytes: 0B";
            // 
            // deflate1Pause
            // 
            this.deflate1Pause.Location = new System.Drawing.Point(6, 20);
            this.deflate1Pause.Name = "deflate1Pause";
            this.deflate1Pause.Size = new System.Drawing.Size(75, 23);
            this.deflate1Pause.TabIndex = 5;
            this.deflate1Pause.Text = "Start";
            this.deflate1Pause.UseVisualStyleBackColor = true;
            this.deflate1Pause.Click += new System.EventHandler(this.deflate1Pause_Click);
            // 
            // deflate1Current
            // 
            this.deflate1Current.Location = new System.Drawing.Point(117, 121);
            this.deflate1Current.Name = "deflate1Current";
            this.deflate1Current.Size = new System.Drawing.Size(56, 17);
            this.deflate1Current.TabIndex = 5;
            this.deflate1Current.Text = "0B";
            this.deflate1Current.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.shuffleCompression);
            this.groupBox5.Controls.Add(this.shuffleBps);
            this.groupBox5.Controls.Add(this.shuffleTotal);
            this.groupBox5.Controls.Add(this.shufflePause);
            this.groupBox5.Controls.Add(this.shuffleCurrent);
            this.groupBox5.Controls.Add(this.shuffleCurrentBar);
            this.groupBox5.Location = new System.Drawing.Point(376, 153);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(179, 141);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Shuffle";
            // 
            // shuffleCompression
            // 
            this.shuffleCompression.AutoSize = true;
            this.shuffleCompression.Location = new System.Drawing.Point(6, 91);
            this.shuffleCompression.Name = "shuffleCompression";
            this.shuffleCompression.Size = new System.Drawing.Size(102, 13);
            this.shuffleCompression.TabIndex = 7;
            this.shuffleCompression.Text = "Compression ratio: 0";
            // 
            // shuffleBps
            // 
            this.shuffleBps.AutoSize = true;
            this.shuffleBps.Location = new System.Drawing.Point(5, 68);
            this.shuffleBps.Name = "shuffleBps";
            this.shuffleBps.Size = new System.Drawing.Size(108, 13);
            this.shuffleBps.TabIndex = 6;
            this.shuffleBps.Text = "Bytes per second: 0B";
            // 
            // shuffleTotal
            // 
            this.shuffleTotal.AutoSize = true;
            this.shuffleTotal.Location = new System.Drawing.Point(6, 46);
            this.shuffleTotal.Name = "shuffleTotal";
            this.shuffleTotal.Size = new System.Drawing.Size(78, 13);
            this.shuffleTotal.TabIndex = 5;
            this.shuffleTotal.Text = "Total bytes: 0B";
            // 
            // shufflePause
            // 
            this.shufflePause.Location = new System.Drawing.Point(6, 20);
            this.shufflePause.Name = "shufflePause";
            this.shufflePause.Size = new System.Drawing.Size(75, 23);
            this.shufflePause.TabIndex = 5;
            this.shufflePause.Text = "Start";
            this.shufflePause.UseVisualStyleBackColor = true;
            this.shufflePause.Click += new System.EventHandler(this.shufflePause_Click);
            // 
            // shuffleCurrent
            // 
            this.shuffleCurrent.Location = new System.Drawing.Point(117, 121);
            this.shuffleCurrent.Name = "shuffleCurrent";
            this.shuffleCurrent.Size = new System.Drawing.Size(56, 17);
            this.shuffleCurrent.TabIndex = 5;
            this.shuffleCurrent.Text = "0B";
            this.shuffleCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1175, 535);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1167, 509);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Graph";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.bufferTotal);
            this.groupBox9.Controls.Add(this.bufferCurrent);
            this.groupBox9.Controls.Add(this.bufferCurrentBar);
            this.groupBox9.Location = new System.Drawing.Point(931, 153);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(179, 141);
            this.groupBox9.TabIndex = 11;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Buffer";
            // 
            // bufferTotal
            // 
            this.bufferTotal.AutoSize = true;
            this.bufferTotal.Location = new System.Drawing.Point(6, 19);
            this.bufferTotal.Name = "bufferTotal";
            this.bufferTotal.Size = new System.Drawing.Size(78, 13);
            this.bufferTotal.TabIndex = 5;
            this.bufferTotal.Text = "Total bytes: 0B";
            // 
            // bufferCurrent
            // 
            this.bufferCurrent.Location = new System.Drawing.Point(117, 121);
            this.bufferCurrent.Name = "bufferCurrent";
            this.bufferCurrent.Size = new System.Drawing.Size(56, 17);
            this.bufferCurrent.TabIndex = 5;
            this.bufferCurrent.Text = "0B";
            this.bufferCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.testerPause);
            this.groupBox8.Controls.Add(this.testerSuccessRate);
            this.groupBox8.Controls.Add(this.testerBps);
            this.groupBox8.Controls.Add(this.testerTotal);
            this.groupBox8.Controls.Add(this.testerCurrent);
            this.groupBox8.Controls.Add(this.testerCurrentBar);
            this.groupBox8.Location = new System.Drawing.Point(746, 153);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(179, 141);
            this.groupBox8.TabIndex = 10;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Tester";
            // 
            // testerPause
            // 
            this.testerPause.Location = new System.Drawing.Point(9, 20);
            this.testerPause.Name = "testerPause";
            this.testerPause.Size = new System.Drawing.Size(75, 23);
            this.testerPause.TabIndex = 12;
            this.testerPause.Text = "Start";
            this.testerPause.UseVisualStyleBackColor = true;
            this.testerPause.Click += new System.EventHandler(this.testerPause_Click);
            // 
            // testerSuccessRate
            // 
            this.testerSuccessRate.AutoSize = true;
            this.testerSuccessRate.Location = new System.Drawing.Point(6, 91);
            this.testerSuccessRate.Name = "testerSuccessRate";
            this.testerSuccessRate.Size = new System.Drawing.Size(81, 13);
            this.testerSuccessRate.TabIndex = 7;
            this.testerSuccessRate.Text = "Success rate: 0";
            // 
            // testerBps
            // 
            this.testerBps.AutoSize = true;
            this.testerBps.Location = new System.Drawing.Point(5, 68);
            this.testerBps.Name = "testerBps";
            this.testerBps.Size = new System.Drawing.Size(108, 13);
            this.testerBps.TabIndex = 6;
            this.testerBps.Text = "Bytes per second: 0B";
            // 
            // testerTotal
            // 
            this.testerTotal.AutoSize = true;
            this.testerTotal.Location = new System.Drawing.Point(6, 46);
            this.testerTotal.Name = "testerTotal";
            this.testerTotal.Size = new System.Drawing.Size(78, 13);
            this.testerTotal.TabIndex = 5;
            this.testerTotal.Text = "Total bytes: 0B";
            // 
            // testerCurrent
            // 
            this.testerCurrent.Location = new System.Drawing.Point(117, 121);
            this.testerCurrent.Name = "testerCurrent";
            this.testerCurrent.Size = new System.Drawing.Size(56, 17);
            this.testerCurrent.TabIndex = 5;
            this.testerCurrent.Text = "0B";
            this.testerCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.hashCompression);
            this.groupBox7.Controls.Add(this.hashBps);
            this.groupBox7.Controls.Add(this.hashTotal);
            this.groupBox7.Controls.Add(this.hashPause);
            this.groupBox7.Controls.Add(this.hashCurrent);
            this.groupBox7.Controls.Add(this.hashCurrentBar);
            this.groupBox7.Location = new System.Drawing.Point(561, 153);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(179, 141);
            this.groupBox7.TabIndex = 9;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Hash";
            // 
            // hashCompression
            // 
            this.hashCompression.AutoSize = true;
            this.hashCompression.Location = new System.Drawing.Point(6, 91);
            this.hashCompression.Name = "hashCompression";
            this.hashCompression.Size = new System.Drawing.Size(102, 13);
            this.hashCompression.TabIndex = 7;
            this.hashCompression.Text = "Compression ratio: 0";
            // 
            // hashBps
            // 
            this.hashBps.AutoSize = true;
            this.hashBps.Location = new System.Drawing.Point(5, 68);
            this.hashBps.Name = "hashBps";
            this.hashBps.Size = new System.Drawing.Size(108, 13);
            this.hashBps.TabIndex = 6;
            this.hashBps.Text = "Bytes per second: 0B";
            // 
            // hashTotal
            // 
            this.hashTotal.AutoSize = true;
            this.hashTotal.Location = new System.Drawing.Point(6, 46);
            this.hashTotal.Name = "hashTotal";
            this.hashTotal.Size = new System.Drawing.Size(78, 13);
            this.hashTotal.TabIndex = 5;
            this.hashTotal.Text = "Total bytes: 0B";
            // 
            // hashPause
            // 
            this.hashPause.Location = new System.Drawing.Point(6, 20);
            this.hashPause.Name = "hashPause";
            this.hashPause.Size = new System.Drawing.Size(75, 23);
            this.hashPause.TabIndex = 5;
            this.hashPause.Text = "Start";
            this.hashPause.UseVisualStyleBackColor = true;
            this.hashPause.Click += new System.EventHandler(this.hashPause_Click);
            // 
            // hashCurrent
            // 
            this.hashCurrent.Location = new System.Drawing.Point(117, 121);
            this.hashCurrent.Name = "hashCurrent";
            this.hashCurrent.Size = new System.Drawing.Size(56, 17);
            this.hashCurrent.TabIndex = 5;
            this.hashCurrent.Text = "0B";
            this.hashCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.deflate2Compression);
            this.groupBox6.Controls.Add(this.deflate2Bps);
            this.groupBox6.Controls.Add(this.deflate2Total);
            this.groupBox6.Controls.Add(this.deflate2Pause);
            this.groupBox6.Controls.Add(this.deflate2Current);
            this.groupBox6.Controls.Add(this.deflate2CurrentBar);
            this.groupBox6.Location = new System.Drawing.Point(191, 153);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(179, 141);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Deflate";
            // 
            // deflate2Compression
            // 
            this.deflate2Compression.AutoSize = true;
            this.deflate2Compression.Location = new System.Drawing.Point(6, 91);
            this.deflate2Compression.Name = "deflate2Compression";
            this.deflate2Compression.Size = new System.Drawing.Size(102, 13);
            this.deflate2Compression.TabIndex = 7;
            this.deflate2Compression.Text = "Compression ratio: 0";
            // 
            // deflate2Bps
            // 
            this.deflate2Bps.AutoSize = true;
            this.deflate2Bps.Location = new System.Drawing.Point(5, 68);
            this.deflate2Bps.Name = "deflate2Bps";
            this.deflate2Bps.Size = new System.Drawing.Size(108, 13);
            this.deflate2Bps.TabIndex = 6;
            this.deflate2Bps.Text = "Bytes per second: 0B";
            // 
            // deflate2Total
            // 
            this.deflate2Total.AutoSize = true;
            this.deflate2Total.Location = new System.Drawing.Point(6, 46);
            this.deflate2Total.Name = "deflate2Total";
            this.deflate2Total.Size = new System.Drawing.Size(78, 13);
            this.deflate2Total.TabIndex = 5;
            this.deflate2Total.Text = "Total bytes: 0B";
            // 
            // deflate2Pause
            // 
            this.deflate2Pause.Location = new System.Drawing.Point(6, 20);
            this.deflate2Pause.Name = "deflate2Pause";
            this.deflate2Pause.Size = new System.Drawing.Size(75, 23);
            this.deflate2Pause.TabIndex = 5;
            this.deflate2Pause.Text = "Start";
            this.deflate2Pause.UseVisualStyleBackColor = true;
            this.deflate2Pause.Click += new System.EventHandler(this.deflate2Pause_Click);
            // 
            // deflate2Current
            // 
            this.deflate2Current.Location = new System.Drawing.Point(117, 121);
            this.deflate2Current.Name = "deflate2Current";
            this.deflate2Current.Size = new System.Drawing.Size(56, 17);
            this.deflate2Current.TabIndex = 5;
            this.deflate2Current.Text = "0B";
            this.deflate2Current.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lastTestReport);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.lastResultNonOverlappingTemplateMatchings);
            this.tabPage2.Controls.Add(this.lastResultDft);
            this.tabPage2.Controls.Add(this.lastResultBinaryMatrixRank);
            this.tabPage2.Controls.Add(this.lastResultLongestRunOfOnes);
            this.tabPage2.Controls.Add(this.lastResultRandomExcursionsVariant);
            this.tabPage2.Controls.Add(this.lastResultRandomExcursions);
            this.tabPage2.Controls.Add(this.lastResultCumulativeSums);
            this.tabPage2.Controls.Add(this.lastResultApproximateEntropy);
            this.tabPage2.Controls.Add(this.lastResultSerial);
            this.tabPage2.Controls.Add(this.lastResultLinearComplexity);
            this.tabPage2.Controls.Add(this.lastResultMaurersUniversal);
            this.tabPage2.Controls.Add(this.lastResultOverlappingTemplateMatchings);
            this.tabPage2.Controls.Add(this.lastResultRuns);
            this.tabPage2.Controls.Add(this.lastResultBlockFrequency);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.lastResultFrequency);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.checkBoxRandomExcursionsVariant);
            this.tabPage2.Controls.Add(this.checkBoxRandomExcursions);
            this.tabPage2.Controls.Add(this.checkBoxCumulativeSums);
            this.tabPage2.Controls.Add(this.checkBoxApproximateEntropy);
            this.tabPage2.Controls.Add(this.checkBoxSerial);
            this.tabPage2.Controls.Add(this.checkBoxLinearComplexity);
            this.tabPage2.Controls.Add(this.checkBoxMaurersUniversal);
            this.tabPage2.Controls.Add(this.checkBoxOverlappingTemplateMatchings);
            this.tabPage2.Controls.Add(this.checkBoxNonOverlappingTemplateMatchings);
            this.tabPage2.Controls.Add(this.checkBoxDft);
            this.tabPage2.Controls.Add(this.checkBoxBinaryMatrixRank);
            this.tabPage2.Controls.Add(this.checkBoxLongestRunOfOnes);
            this.tabPage2.Controls.Add(this.checkBoxRuns);
            this.tabPage2.Controls.Add(this.checkBoxBlockFrequency);
            this.tabPage2.Controls.Add(this.checkBoxFrequency);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1167, 509);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test results";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(8, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "Tests to perform";
            // 
            // checkBoxRandomExcursionsVariant
            // 
            this.checkBoxRandomExcursionsVariant.AutoSize = true;
            this.checkBoxRandomExcursionsVariant.Checked = true;
            this.checkBoxRandomExcursionsVariant.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRandomExcursionsVariant.Location = new System.Drawing.Point(8, 353);
            this.checkBoxRandomExcursionsVariant.Name = "checkBoxRandomExcursionsVariant";
            this.checkBoxRandomExcursionsVariant.Size = new System.Drawing.Size(154, 17);
            this.checkBoxRandomExcursionsVariant.TabIndex = 14;
            this.checkBoxRandomExcursionsVariant.Text = "Random excursions variant";
            this.checkBoxRandomExcursionsVariant.UseVisualStyleBackColor = true;
            this.checkBoxRandomExcursionsVariant.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxRandomExcursions
            // 
            this.checkBoxRandomExcursions.AutoSize = true;
            this.checkBoxRandomExcursions.Checked = true;
            this.checkBoxRandomExcursions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRandomExcursions.Location = new System.Drawing.Point(8, 330);
            this.checkBoxRandomExcursions.Name = "checkBoxRandomExcursions";
            this.checkBoxRandomExcursions.Size = new System.Drawing.Size(122, 17);
            this.checkBoxRandomExcursions.TabIndex = 13;
            this.checkBoxRandomExcursions.Text = "Random excursions ";
            this.checkBoxRandomExcursions.UseVisualStyleBackColor = true;
            this.checkBoxRandomExcursions.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxCumulativeSums
            // 
            this.checkBoxCumulativeSums.AutoSize = true;
            this.checkBoxCumulativeSums.Checked = true;
            this.checkBoxCumulativeSums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCumulativeSums.Location = new System.Drawing.Point(8, 307);
            this.checkBoxCumulativeSums.Name = "checkBoxCumulativeSums";
            this.checkBoxCumulativeSums.Size = new System.Drawing.Size(105, 17);
            this.checkBoxCumulativeSums.TabIndex = 12;
            this.checkBoxCumulativeSums.Text = "Cumulative sums";
            this.checkBoxCumulativeSums.UseVisualStyleBackColor = true;
            this.checkBoxCumulativeSums.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxApproximateEntropy
            // 
            this.checkBoxApproximateEntropy.AutoSize = true;
            this.checkBoxApproximateEntropy.Checked = true;
            this.checkBoxApproximateEntropy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxApproximateEntropy.Location = new System.Drawing.Point(8, 284);
            this.checkBoxApproximateEntropy.Name = "checkBoxApproximateEntropy";
            this.checkBoxApproximateEntropy.Size = new System.Drawing.Size(122, 17);
            this.checkBoxApproximateEntropy.TabIndex = 11;
            this.checkBoxApproximateEntropy.Text = "Approximate entropy";
            this.checkBoxApproximateEntropy.UseVisualStyleBackColor = true;
            this.checkBoxApproximateEntropy.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxSerial
            // 
            this.checkBoxSerial.AutoSize = true;
            this.checkBoxSerial.Checked = true;
            this.checkBoxSerial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSerial.Location = new System.Drawing.Point(8, 261);
            this.checkBoxSerial.Name = "checkBoxSerial";
            this.checkBoxSerial.Size = new System.Drawing.Size(52, 17);
            this.checkBoxSerial.TabIndex = 10;
            this.checkBoxSerial.Text = "Serial";
            this.checkBoxSerial.UseVisualStyleBackColor = true;
            this.checkBoxSerial.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxLinearComplexity
            // 
            this.checkBoxLinearComplexity.AutoSize = true;
            this.checkBoxLinearComplexity.Location = new System.Drawing.Point(8, 238);
            this.checkBoxLinearComplexity.Name = "checkBoxLinearComplexity";
            this.checkBoxLinearComplexity.Size = new System.Drawing.Size(107, 17);
            this.checkBoxLinearComplexity.TabIndex = 9;
            this.checkBoxLinearComplexity.Text = "Linear complexity";
            this.checkBoxLinearComplexity.UseVisualStyleBackColor = true;
            this.checkBoxLinearComplexity.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxMaurersUniversal
            // 
            this.checkBoxMaurersUniversal.AutoSize = true;
            this.checkBoxMaurersUniversal.Checked = true;
            this.checkBoxMaurersUniversal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMaurersUniversal.Location = new System.Drawing.Point(8, 215);
            this.checkBoxMaurersUniversal.Name = "checkBoxMaurersUniversal";
            this.checkBoxMaurersUniversal.Size = new System.Drawing.Size(169, 17);
            this.checkBoxMaurersUniversal.TabIndex = 8;
            this.checkBoxMaurersUniversal.Text = "Maurer\'s \"Universal statistical\"";
            this.checkBoxMaurersUniversal.UseVisualStyleBackColor = true;
            this.checkBoxMaurersUniversal.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxOverlappingTemplateMatchings
            // 
            this.checkBoxOverlappingTemplateMatchings.AutoSize = true;
            this.checkBoxOverlappingTemplateMatchings.Checked = true;
            this.checkBoxOverlappingTemplateMatchings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOverlappingTemplateMatchings.Location = new System.Drawing.Point(8, 192);
            this.checkBoxOverlappingTemplateMatchings.Name = "checkBoxOverlappingTemplateMatchings";
            this.checkBoxOverlappingTemplateMatchings.Size = new System.Drawing.Size(172, 17);
            this.checkBoxOverlappingTemplateMatchings.TabIndex = 7;
            this.checkBoxOverlappingTemplateMatchings.Text = "Overlapping template matching";
            this.checkBoxOverlappingTemplateMatchings.UseVisualStyleBackColor = true;
            this.checkBoxOverlappingTemplateMatchings.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxNonOverlappingTemplateMatchings
            // 
            this.checkBoxNonOverlappingTemplateMatchings.AutoSize = true;
            this.checkBoxNonOverlappingTemplateMatchings.Location = new System.Drawing.Point(8, 169);
            this.checkBoxNonOverlappingTemplateMatchings.Name = "checkBoxNonOverlappingTemplateMatchings";
            this.checkBoxNonOverlappingTemplateMatchings.Size = new System.Drawing.Size(193, 17);
            this.checkBoxNonOverlappingTemplateMatchings.TabIndex = 6;
            this.checkBoxNonOverlappingTemplateMatchings.Text = "Non-overlapping template matching";
            this.checkBoxNonOverlappingTemplateMatchings.UseVisualStyleBackColor = true;
            this.checkBoxNonOverlappingTemplateMatchings.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxDft
            // 
            this.checkBoxDft.AutoSize = true;
            this.checkBoxDft.Location = new System.Drawing.Point(8, 146);
            this.checkBoxDft.Name = "checkBoxDft";
            this.checkBoxDft.Size = new System.Drawing.Size(146, 17);
            this.checkBoxDft.TabIndex = 5;
            this.checkBoxDft.Text = "Discrete Fourier transform";
            this.checkBoxDft.UseVisualStyleBackColor = true;
            this.checkBoxDft.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxBinaryMatrixRank
            // 
            this.checkBoxBinaryMatrixRank.AutoSize = true;
            this.checkBoxBinaryMatrixRank.Checked = true;
            this.checkBoxBinaryMatrixRank.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBinaryMatrixRank.Location = new System.Drawing.Point(8, 123);
            this.checkBoxBinaryMatrixRank.Name = "checkBoxBinaryMatrixRank";
            this.checkBoxBinaryMatrixRank.Size = new System.Drawing.Size(109, 17);
            this.checkBoxBinaryMatrixRank.TabIndex = 4;
            this.checkBoxBinaryMatrixRank.Text = "Binary matrix rank";
            this.checkBoxBinaryMatrixRank.UseVisualStyleBackColor = true;
            this.checkBoxBinaryMatrixRank.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxLongestRunOfOnes
            // 
            this.checkBoxLongestRunOfOnes.AutoSize = true;
            this.checkBoxLongestRunOfOnes.Checked = true;
            this.checkBoxLongestRunOfOnes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLongestRunOfOnes.Location = new System.Drawing.Point(8, 100);
            this.checkBoxLongestRunOfOnes.Name = "checkBoxLongestRunOfOnes";
            this.checkBoxLongestRunOfOnes.Size = new System.Drawing.Size(120, 17);
            this.checkBoxLongestRunOfOnes.TabIndex = 3;
            this.checkBoxLongestRunOfOnes.Text = "Longest run of ones";
            this.checkBoxLongestRunOfOnes.UseVisualStyleBackColor = true;
            this.checkBoxLongestRunOfOnes.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxRuns
            // 
            this.checkBoxRuns.AutoSize = true;
            this.checkBoxRuns.Checked = true;
            this.checkBoxRuns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRuns.Location = new System.Drawing.Point(8, 77);
            this.checkBoxRuns.Name = "checkBoxRuns";
            this.checkBoxRuns.Size = new System.Drawing.Size(51, 17);
            this.checkBoxRuns.TabIndex = 2;
            this.checkBoxRuns.Text = "Runs";
            this.checkBoxRuns.UseVisualStyleBackColor = true;
            this.checkBoxRuns.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxBlockFrequency
            // 
            this.checkBoxBlockFrequency.AutoSize = true;
            this.checkBoxBlockFrequency.Checked = true;
            this.checkBoxBlockFrequency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBlockFrequency.Location = new System.Drawing.Point(8, 54);
            this.checkBoxBlockFrequency.Name = "checkBoxBlockFrequency";
            this.checkBoxBlockFrequency.Size = new System.Drawing.Size(103, 17);
            this.checkBoxBlockFrequency.TabIndex = 1;
            this.checkBoxBlockFrequency.Text = "Block frequency";
            this.checkBoxBlockFrequency.UseVisualStyleBackColor = true;
            this.checkBoxBlockFrequency.CheckStateChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // checkBoxFrequency
            // 
            this.checkBoxFrequency.AutoSize = true;
            this.checkBoxFrequency.Checked = true;
            this.checkBoxFrequency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFrequency.Location = new System.Drawing.Point(8, 31);
            this.checkBoxFrequency.Name = "checkBoxFrequency";
            this.checkBoxFrequency.Size = new System.Drawing.Size(76, 17);
            this.checkBoxFrequency.TabIndex = 0;
            this.checkBoxFrequency.Text = "Frequency";
            this.checkBoxFrequency.UseVisualStyleBackColor = true;
            this.checkBoxFrequency.CheckedChanged += new System.EventHandler(this.testCheckBox_CheckedChanged);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // lastResultFrequency
            // 
            this.lastResultFrequency.AutoSize = true;
            this.lastResultFrequency.Location = new System.Drawing.Point(287, 35);
            this.lastResultFrequency.Name = "lastResultFrequency";
            this.lastResultFrequency.Size = new System.Drawing.Size(96, 13);
            this.lastResultFrequency.TabIndex = 16;
            this.lastResultFrequency.Text = "No tests performed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(286, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 20);
            this.label3.TabIndex = 17;
            this.label3.Text = "Last test";
            // 
            // lastResultBlockFrequency
            // 
            this.lastResultBlockFrequency.AutoSize = true;
            this.lastResultBlockFrequency.Location = new System.Drawing.Point(287, 58);
            this.lastResultBlockFrequency.Name = "lastResultBlockFrequency";
            this.lastResultBlockFrequency.Size = new System.Drawing.Size(96, 13);
            this.lastResultBlockFrequency.TabIndex = 18;
            this.lastResultBlockFrequency.Text = "No tests performed";
            // 
            // lastResultRuns
            // 
            this.lastResultRuns.AutoSize = true;
            this.lastResultRuns.Location = new System.Drawing.Point(287, 81);
            this.lastResultRuns.Name = "lastResultRuns";
            this.lastResultRuns.Size = new System.Drawing.Size(96, 13);
            this.lastResultRuns.TabIndex = 19;
            this.lastResultRuns.Text = "No tests performed";
            // 
            // lastResultOverlappingTemplateMatchings
            // 
            this.lastResultOverlappingTemplateMatchings.AutoSize = true;
            this.lastResultOverlappingTemplateMatchings.Location = new System.Drawing.Point(287, 196);
            this.lastResultOverlappingTemplateMatchings.Name = "lastResultOverlappingTemplateMatchings";
            this.lastResultOverlappingTemplateMatchings.Size = new System.Drawing.Size(96, 13);
            this.lastResultOverlappingTemplateMatchings.TabIndex = 20;
            this.lastResultOverlappingTemplateMatchings.Text = "No tests performed";
            // 
            // lastResultMaurersUniversal
            // 
            this.lastResultMaurersUniversal.AutoSize = true;
            this.lastResultMaurersUniversal.Location = new System.Drawing.Point(287, 219);
            this.lastResultMaurersUniversal.Name = "lastResultMaurersUniversal";
            this.lastResultMaurersUniversal.Size = new System.Drawing.Size(96, 13);
            this.lastResultMaurersUniversal.TabIndex = 21;
            this.lastResultMaurersUniversal.Text = "No tests performed";
            // 
            // lastResultLinearComplexity
            // 
            this.lastResultLinearComplexity.AutoSize = true;
            this.lastResultLinearComplexity.Location = new System.Drawing.Point(287, 242);
            this.lastResultLinearComplexity.Name = "lastResultLinearComplexity";
            this.lastResultLinearComplexity.Size = new System.Drawing.Size(96, 13);
            this.lastResultLinearComplexity.TabIndex = 22;
            this.lastResultLinearComplexity.Text = "No tests performed";
            // 
            // lastResultSerial
            // 
            this.lastResultSerial.AutoSize = true;
            this.lastResultSerial.Location = new System.Drawing.Point(287, 265);
            this.lastResultSerial.Name = "lastResultSerial";
            this.lastResultSerial.Size = new System.Drawing.Size(96, 13);
            this.lastResultSerial.TabIndex = 23;
            this.lastResultSerial.Text = "No tests performed";
            // 
            // lastResultApproximateEntropy
            // 
            this.lastResultApproximateEntropy.AutoSize = true;
            this.lastResultApproximateEntropy.Location = new System.Drawing.Point(287, 288);
            this.lastResultApproximateEntropy.Name = "lastResultApproximateEntropy";
            this.lastResultApproximateEntropy.Size = new System.Drawing.Size(96, 13);
            this.lastResultApproximateEntropy.TabIndex = 24;
            this.lastResultApproximateEntropy.Text = "No tests performed";
            // 
            // lastResultCumulativeSums
            // 
            this.lastResultCumulativeSums.AutoSize = true;
            this.lastResultCumulativeSums.Location = new System.Drawing.Point(287, 311);
            this.lastResultCumulativeSums.Name = "lastResultCumulativeSums";
            this.lastResultCumulativeSums.Size = new System.Drawing.Size(96, 13);
            this.lastResultCumulativeSums.TabIndex = 25;
            this.lastResultCumulativeSums.Text = "No tests performed";
            // 
            // lastResultRandomExcursions
            // 
            this.lastResultRandomExcursions.AutoSize = true;
            this.lastResultRandomExcursions.Location = new System.Drawing.Point(287, 334);
            this.lastResultRandomExcursions.Name = "lastResultRandomExcursions";
            this.lastResultRandomExcursions.Size = new System.Drawing.Size(96, 13);
            this.lastResultRandomExcursions.TabIndex = 26;
            this.lastResultRandomExcursions.Text = "No tests performed";
            // 
            // lastResultRandomExcursionsVariant
            // 
            this.lastResultRandomExcursionsVariant.AutoSize = true;
            this.lastResultRandomExcursionsVariant.Location = new System.Drawing.Point(287, 357);
            this.lastResultRandomExcursionsVariant.Name = "lastResultRandomExcursionsVariant";
            this.lastResultRandomExcursionsVariant.Size = new System.Drawing.Size(96, 13);
            this.lastResultRandomExcursionsVariant.TabIndex = 27;
            this.lastResultRandomExcursionsVariant.Text = "No tests performed";
            // 
            // lastResultLongestRunOfOnes
            // 
            this.lastResultLongestRunOfOnes.AutoSize = true;
            this.lastResultLongestRunOfOnes.Location = new System.Drawing.Point(287, 104);
            this.lastResultLongestRunOfOnes.Name = "lastResultLongestRunOfOnes";
            this.lastResultLongestRunOfOnes.Size = new System.Drawing.Size(96, 13);
            this.lastResultLongestRunOfOnes.TabIndex = 28;
            this.lastResultLongestRunOfOnes.Text = "No tests performed";
            // 
            // lastResultBinaryMatrixRank
            // 
            this.lastResultBinaryMatrixRank.AutoSize = true;
            this.lastResultBinaryMatrixRank.Location = new System.Drawing.Point(287, 127);
            this.lastResultBinaryMatrixRank.Name = "lastResultBinaryMatrixRank";
            this.lastResultBinaryMatrixRank.Size = new System.Drawing.Size(96, 13);
            this.lastResultBinaryMatrixRank.TabIndex = 29;
            this.lastResultBinaryMatrixRank.Text = "No tests performed";
            // 
            // lastResultDft
            // 
            this.lastResultDft.AutoSize = true;
            this.lastResultDft.Location = new System.Drawing.Point(287, 150);
            this.lastResultDft.Name = "lastResultDft";
            this.lastResultDft.Size = new System.Drawing.Size(96, 13);
            this.lastResultDft.TabIndex = 30;
            this.lastResultDft.Text = "No tests performed";
            // 
            // lastResultNonOverlappingTemplateMatchings
            // 
            this.lastResultNonOverlappingTemplateMatchings.AutoSize = true;
            this.lastResultNonOverlappingTemplateMatchings.Location = new System.Drawing.Point(287, 173);
            this.lastResultNonOverlappingTemplateMatchings.Name = "lastResultNonOverlappingTemplateMatchings";
            this.lastResultNonOverlappingTemplateMatchings.Size = new System.Drawing.Size(96, 13);
            this.lastResultNonOverlappingTemplateMatchings.TabIndex = 31;
            this.lastResultNonOverlappingTemplateMatchings.Text = "No tests performed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(416, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 20);
            this.label2.TabIndex = 32;
            this.label2.Text = "Last test report";
            // 
            // lastTestReport
            // 
            this.lastTestReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lastTestReport.Location = new System.Drawing.Point(420, 26);
            this.lastTestReport.Name = "lastTestReport";
            this.lastTestReport.Size = new System.Drawing.Size(732, 460);
            this.lastTestReport.TabIndex = 33;
            this.lastTestReport.Text = "";
            // 
            // bufferCurrentBar
            // 
            this.bufferCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.bufferCurrentBar.Name = "bufferCurrentBar";
            this.bufferCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.bufferCurrentBar.TabIndex = 1;
            // 
            // testerCurrentBar
            // 
            this.testerCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.testerCurrentBar.Name = "testerCurrentBar";
            this.testerCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.testerCurrentBar.TabIndex = 1;
            // 
            // hashCurrentBar
            // 
            this.hashCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.hashCurrentBar.Name = "hashCurrentBar";
            this.hashCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.hashCurrentBar.TabIndex = 1;
            // 
            // deflate2CurrentBar
            // 
            this.deflate2CurrentBar.Location = new System.Drawing.Point(148, 19);
            this.deflate2CurrentBar.Name = "deflate2CurrentBar";
            this.deflate2CurrentBar.Size = new System.Drawing.Size(25, 99);
            this.deflate2CurrentBar.TabIndex = 1;
            // 
            // deflate1CurrentBar
            // 
            this.deflate1CurrentBar.Location = new System.Drawing.Point(148, 19);
            this.deflate1CurrentBar.Name = "deflate1CurrentBar";
            this.deflate1CurrentBar.Size = new System.Drawing.Size(25, 99);
            this.deflate1CurrentBar.TabIndex = 1;
            // 
            // biologicalCurrentBar
            // 
            this.biologicalCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.biologicalCurrentBar.Name = "biologicalCurrentBar";
            this.biologicalCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.biologicalCurrentBar.TabIndex = 1;
            // 
            // shuffleCurrentBar
            // 
            this.shuffleCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.shuffleCurrentBar.Name = "shuffleCurrentBar";
            this.shuffleCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.shuffleCurrentBar.TabIndex = 1;
            // 
            // audioCurrentBar
            // 
            this.audioCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.audioCurrentBar.Name = "audioCurrentBar";
            this.audioCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.audioCurrentBar.TabIndex = 1;
            // 
            // videoCurrentBar
            // 
            this.videoCurrentBar.Location = new System.Drawing.Point(148, 19);
            this.videoCurrentBar.Name = "videoCurrentBar";
            this.videoCurrentBar.Size = new System.Drawing.Size(25, 99);
            this.videoCurrentBar.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.count);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.upperBound);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.lowerBound);
            this.tabPage3.Controls.Add(this.results);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.generate);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.type);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1167, 509);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Data";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // type
            // 
            this.type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type.FormattingEnabled = true;
            this.type.Items.AddRange(new object[] {
            "Bit",
            "Byte",
            "Int",
            "UInt",
            "Long",
            "Ulong"});
            this.type.Location = new System.Drawing.Point(12, 26);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(121, 21);
            this.type.TabIndex = 2;
            this.type.SelectedIndexChanged += new System.EventHandler(this.type_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(8, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Type";
            // 
            // generate
            // 
            this.generate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.generate.Location = new System.Drawing.Point(12, 209);
            this.generate.Name = "generate";
            this.generate.Size = new System.Drawing.Size(121, 33);
            this.generate.TabIndex = 4;
            this.generate.Text = "Generate";
            this.generate.UseVisualStyleBackColor = true;
            this.generate.Click += new System.EventHandler(this.generate_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(8, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 20);
            this.label7.TabIndex = 5;
            this.label7.Text = "Between";
            // 
            // results
            // 
            this.results.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.results.Location = new System.Drawing.Point(139, 26);
            this.results.Name = "results";
            this.results.Size = new System.Drawing.Size(153, 458);
            this.results.TabIndex = 6;
            this.results.Text = "";
            // 
            // lowerBound
            // 
            this.lowerBound.Location = new System.Drawing.Point(12, 85);
            this.lowerBound.Name = "lowerBound";
            this.lowerBound.Size = new System.Drawing.Size(100, 20);
            this.lowerBound.TabIndex = 7;
            this.lowerBound.Text = "10";
            // 
            // upperBound
            // 
            this.upperBound.Location = new System.Drawing.Point(12, 134);
            this.upperBound.Name = "upperBound";
            this.upperBound.Size = new System.Drawing.Size(100, 20);
            this.upperBound.TabIndex = 9;
            this.upperBound.Text = "100";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(8, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "And";
            // 
            // count
            // 
            this.count.Location = new System.Drawing.Point(12, 183);
            this.count.Name = "count";
            this.count.Size = new System.Drawing.Size(100, 20);
            this.count.TabIndex = 11;
            this.count.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(8, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Count";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1175, 535);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "TrulyRandom sample app";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private VerticalProgressBar videoCurrentBar;
        private System.Windows.Forms.Button videoPause;
        private System.Windows.Forms.Label videoCurrent;
        private System.Windows.Forms.Label videoBps;
        private System.Windows.Forms.Label videoTotal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label audioBps;
        private System.Windows.Forms.Label audioTotal;
        private System.Windows.Forms.Button audioPause;
        private System.Windows.Forms.Label audioCurrent;
        private VerticalProgressBar audioCurrentBar;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label biologicalBps;
        private System.Windows.Forms.Label biologicalTotal;
        private System.Windows.Forms.Button biologicalPause;
        private System.Windows.Forms.Label biologicalCurrent;
        private VerticalProgressBar biologicalCurrentBar;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label deflate1Compression;
        private System.Windows.Forms.Label deflate1Bps;
        private System.Windows.Forms.Label deflate1Total;
        private System.Windows.Forms.Button deflate1Pause;
        private System.Windows.Forms.Label deflate1Current;
        private VerticalProgressBar deflate1CurrentBar;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label shuffleCompression;
        private System.Windows.Forms.Label shuffleBps;
        private System.Windows.Forms.Label shuffleTotal;
        private System.Windows.Forms.Button shufflePause;
        private System.Windows.Forms.Label shuffleCurrent;
        private VerticalProgressBar shuffleCurrentBar;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label deflate2Compression;
        private System.Windows.Forms.Label deflate2Bps;
        private System.Windows.Forms.Label deflate2Total;
        private System.Windows.Forms.Button deflate2Pause;
        private System.Windows.Forms.Label deflate2Current;
        private VerticalProgressBar deflate2CurrentBar;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label hashCompression;
        private System.Windows.Forms.Label hashBps;
        private System.Windows.Forms.Label hashTotal;
        private System.Windows.Forms.Button hashPause;
        private System.Windows.Forms.Label hashCurrent;
        private VerticalProgressBar hashCurrentBar;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label bufferTotal;
        private System.Windows.Forms.Label bufferCurrent;
        private VerticalProgressBar bufferCurrentBar;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label testerSuccessRate;
        private System.Windows.Forms.Label testerBps;
        private System.Windows.Forms.Label testerTotal;
        private System.Windows.Forms.Label testerCurrent;
        private VerticalProgressBar testerCurrentBar;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Label videoName;
        private System.Windows.Forms.Label audioName;
        private System.Windows.Forms.Label videoStub;
        private System.Windows.Forms.Label audioStub;
        private System.Windows.Forms.Button testerPause;
        private System.Windows.Forms.CheckBox checkBoxApproximateEntropy;
        private System.Windows.Forms.CheckBox checkBoxSerial;
        private System.Windows.Forms.CheckBox checkBoxLinearComplexity;
        private System.Windows.Forms.CheckBox checkBoxMaurersUniversal;
        private System.Windows.Forms.CheckBox checkBoxOverlappingTemplateMatchings;
        private System.Windows.Forms.CheckBox checkBoxNonOverlappingTemplateMatchings;
        private System.Windows.Forms.CheckBox checkBoxDft;
        private System.Windows.Forms.CheckBox checkBoxBinaryMatrixRank;
        private System.Windows.Forms.CheckBox checkBoxLongestRunOfOnes;
        private System.Windows.Forms.CheckBox checkBoxRuns;
        private System.Windows.Forms.CheckBox checkBoxBlockFrequency;
        private System.Windows.Forms.CheckBox checkBoxFrequency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxRandomExcursionsVariant;
        private System.Windows.Forms.CheckBox checkBoxRandomExcursions;
        private System.Windows.Forms.CheckBox checkBoxCumulativeSums;
        private System.Windows.Forms.RichTextBox lastTestReport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lastResultNonOverlappingTemplateMatchings;
        private System.Windows.Forms.Label lastResultDft;
        private System.Windows.Forms.Label lastResultBinaryMatrixRank;
        private System.Windows.Forms.Label lastResultLongestRunOfOnes;
        private System.Windows.Forms.Label lastResultRandomExcursionsVariant;
        private System.Windows.Forms.Label lastResultRandomExcursions;
        private System.Windows.Forms.Label lastResultCumulativeSums;
        private System.Windows.Forms.Label lastResultApproximateEntropy;
        private System.Windows.Forms.Label lastResultSerial;
        private System.Windows.Forms.Label lastResultLinearComplexity;
        private System.Windows.Forms.Label lastResultMaurersUniversal;
        private System.Windows.Forms.Label lastResultOverlappingTemplateMatchings;
        private System.Windows.Forms.Label lastResultRuns;
        private System.Windows.Forms.Label lastResultBlockFrequency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lastResultFrequency;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox count;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox upperBound;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox lowerBound;
        private System.Windows.Forms.RichTextBox results;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button generate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox type;
    }
}

