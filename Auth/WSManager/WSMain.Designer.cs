namespace WSManager
{
    partial class WSMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WSMain));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.comService = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbStatus = new System.Windows.Forms.ComboBox();
            this.btnUnInstall = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnReboot = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtftppath = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtip = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtwcfport = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtsocketport = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAuthConstr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCheckConn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.txtpl = new System.Windows.Forms.TextBox();
            this.comService.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(252, 489);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 34);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "保存";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 489);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 34);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "退出";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // comService
            // 
            this.comService.Controls.Add(this.groupBox2);
            this.comService.Controls.Add(this.groupBox1);
            this.comService.Location = new System.Drawing.Point(4, 22);
            this.comService.Name = "comService";
            this.comService.Padding = new System.Windows.Forms.Padding(3);
            this.comService.Size = new System.Drawing.Size(714, 456);
            this.comService.TabIndex = 0;
            this.comService.Text = "通信服务";
            this.comService.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbStatus);
            this.groupBox2.Controls.Add(this.btnUnInstall);
            this.groupBox2.Controls.Add(this.btnInstall);
            this.groupBox2.Controls.Add(this.txtServerName);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btnReboot);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Controls.Add(this.btnRefresh);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(6, 307);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(702, 146);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "服务管理";
            // 
            // cbStatus
            // 
            this.cbStatus.Enabled = false;
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Items.AddRange(new object[] {
            "未安装",
            "已启动",
            "已停止"});
            this.cbStatus.Location = new System.Drawing.Point(436, 27);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(167, 20);
            this.cbStatus.TabIndex = 19;
            // 
            // btnUnInstall
            // 
            this.btnUnInstall.Location = new System.Drawing.Point(309, 96);
            this.btnUnInstall.Name = "btnUnInstall";
            this.btnUnInstall.Size = new System.Drawing.Size(100, 35);
            this.btnUnInstall.TabIndex = 18;
            this.btnUnInstall.Text = "卸载";
            this.btnUnInstall.UseVisualStyleBackColor = true;
            this.btnUnInstall.Click += new System.EventHandler(this.btnUnInstall_Click);
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(64, 96);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(100, 35);
            this.btnInstall.TabIndex = 17;
            this.btnInstall.Text = "安装";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(164, 24);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(167, 21);
            this.txtServerName.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(72, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "服务名称:";
            // 
            // btnReboot
            // 
            this.btnReboot.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnReboot.Location = new System.Drawing.Point(183, 96);
            this.btnReboot.Name = "btnReboot";
            this.btnReboot.Size = new System.Drawing.Size(100, 35);
            this.btnReboot.TabIndex = 13;
            this.btnReboot.Text = "重启";
            this.btnReboot.UseVisualStyleBackColor = true;
            this.btnReboot.Click += new System.EventHandler(this.btnReboot_Click);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(433, 96);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 35);
            this.btnStop.TabIndex = 15;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRefresh.Location = new System.Drawing.Point(556, 96);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 35);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(344, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "服务状态:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtpl);
            this.groupBox1.Controls.Add(this.lbl2);
            this.groupBox1.Controls.Add(this.lbl1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txtftppath);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtip);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtwcfport);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtsocketport);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtAuthConstr);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCheckConn);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 295);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器信息";
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.ForeColor = System.Drawing.Color.Red;
            this.lbl2.Location = new System.Drawing.Point(20, 147);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(41, 12);
            this.lbl2.TabIndex = 26;
            this.lbl2.Text = "Window";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.ForeColor = System.Drawing.Color.Red;
            this.lbl1.Location = new System.Drawing.Point(20, 116);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(65, 12);
            this.lbl1.TabIndex = 25;
            this.lbl1.Text = "SQL Server";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(584, 178);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 40);
            this.button1.TabIndex = 24;
            this.button1.Text = "测试数据库连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnCheckConn_Click);
            // 
            // txtftppath
            // 
            this.txtftppath.Location = new System.Drawing.Point(161, 248);
            this.txtftppath.Name = "txtftppath";
            this.txtftppath.Size = new System.Drawing.Size(144, 21);
            this.txtftppath.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(78, 248);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 12);
            this.label10.TabIndex = 23;
            this.label10.Text = "终端日志路径:";
            // 
            // txtip
            // 
            this.txtip.Location = new System.Drawing.Point(161, 186);
            this.txtip.Name = "txtip";
            this.txtip.Size = new System.Drawing.Size(144, 21);
            this.txtip.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(72, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 21;
            this.label4.Text = "WCF/Socket IP:";
            // 
            // txtwcfport
            // 
            this.txtwcfport.Location = new System.Drawing.Point(389, 186);
            this.txtwcfport.Name = "txtwcfport";
            this.txtwcfport.Size = new System.Drawing.Size(144, 21);
            this.txtwcfport.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(330, 186);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "WCF端口:";
            // 
            // txtsocketport
            // 
            this.txtsocketport.Location = new System.Drawing.Point(389, 245);
            this.txtsocketport.Name = "txtsocketport";
            this.txtsocketport.Size = new System.Drawing.Size(144, 21);
            this.txtsocketport.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(312, 251);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "Socket端口:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "平台数据库:";
            // 
            // txtAuthConstr
            // 
            this.txtAuthConstr.Location = new System.Drawing.Point(103, 15);
            this.txtAuthConstr.Multiline = true;
            this.txtAuthConstr.Name = "txtAuthConstr";
            this.txtAuthConstr.Size = new System.Drawing.Size(581, 38);
            this.txtAuthConstr.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "认证数据库:";
            // 
            // btnCheckConn
            // 
            this.btnCheckConn.Location = new System.Drawing.Point(584, 235);
            this.btnCheckConn.Name = "btnCheckConn";
            this.btnCheckConn.Size = new System.Drawing.Size(100, 40);
            this.btnCheckConn.TabIndex = 4;
            this.btnCheckConn.Text = "测试WCF连接";
            this.btnCheckConn.UseVisualStyleBackColor = true;
            this.btnCheckConn.Click += new System.EventHandler(this.btnCheckConn_Click_1);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.comService);
            this.tabControl1.Location = new System.Drawing.Point(2, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(722, 482);
            this.tabControl1.TabIndex = 21;
            // 
            // txtpl
            // 
            this.txtpl.Location = new System.Drawing.Point(103, 66);
            this.txtpl.Multiline = true;
            this.txtpl.Name = "txtpl";
            this.txtpl.Size = new System.Drawing.Size(581, 38);
            this.txtpl.TabIndex = 28;
            // 
            // WSMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 532);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WSMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置工具";
            this.Load += new System.EventHandler(this.WSMain_Load);
            this.comService.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabPage comService;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbStatus;
        private System.Windows.Forms.Button btnUnInstall;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnReboot;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtftppath;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtip;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtwcfport;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtsocketport;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAuthConstr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCheckConn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.TextBox txtpl;
    }
}

