namespace ConvertDemo
{
    partial class ConvertSettings
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
            this.groupBoxLogLevel = new System.Windows.Forms.GroupBox();
            this.radioButtonInfo = new System.Windows.Forms.RadioButton();
            this.radioButtonFatal = new System.Windows.Forms.RadioButton();
            this.radioButtonDebug = new System.Windows.Forms.RadioButton();
            this.radioButtonWarning = new System.Windows.Forms.RadioButton();
            this.radioButtonPanic = new System.Windows.Forms.RadioButton();
            this.radioButtonVerbose = new System.Windows.Forms.RadioButton();
            this.radioButtonError = new System.Windows.Forms.RadioButton();
            this.radioButtonQuiet = new System.Windows.Forms.RadioButton();
            this.groupBoxThreadPriority = new System.Windows.Forms.GroupBox();
            this.radioButtonNormal = new System.Windows.Forms.RadioButton();
            this.radioButtonLowest = new System.Windows.Forms.RadioButton();
            this.radioButtonLower = new System.Windows.Forms.RadioButton();
            this.radioButtonIdle = new System.Windows.Forms.RadioButton();
            this.groupBoxLogPath = new System.Windows.Forms.GroupBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxLogPath = new System.Windows.Forms.TextBox();
            this.groupBoxThreadCount = new System.Windows.Forms.GroupBox();
            this.textBoxThreadCount = new System.Windows.Forms.TextBox();
            this.groupBoxOthers = new System.Windows.Forms.GroupBox();
            this.radioButtonPreview = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxLogLevel.SuspendLayout();
            this.groupBoxThreadPriority.SuspendLayout();
            this.groupBoxLogPath.SuspendLayout();
            this.groupBoxThreadCount.SuspendLayout();
            this.groupBoxOthers.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxLogLevel
            // 
            this.groupBoxLogLevel.Controls.Add(this.radioButtonInfo);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonFatal);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonDebug);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonWarning);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonPanic);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonVerbose);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonError);
            this.groupBoxLogLevel.Controls.Add(this.radioButtonQuiet);
            this.groupBoxLogLevel.Location = new System.Drawing.Point(12, 12);
            this.groupBoxLogLevel.Name = "groupBoxLogLevel";
            this.groupBoxLogLevel.Size = new System.Drawing.Size(202, 120);
            this.groupBoxLogLevel.TabIndex = 0;
            this.groupBoxLogLevel.TabStop = false;
            this.groupBoxLogLevel.Text = "Log Level";
            this.groupBoxLogLevel.Enter += new System.EventHandler(this.groupBoxLogSettings_Enter);
            // 
            // radioButtonInfo
            // 
            this.radioButtonInfo.AutoSize = true;
            this.radioButtonInfo.Checked = true;
            this.radioButtonInfo.Location = new System.Drawing.Point(62, 92);
            this.radioButtonInfo.Name = "radioButtonInfo";
            this.radioButtonInfo.Size = new System.Drawing.Size(43, 17);
            this.radioButtonInfo.TabIndex = 7;
            this.radioButtonInfo.TabStop = true;
            this.radioButtonInfo.Text = "Info";
            this.radioButtonInfo.UseVisualStyleBackColor = true;
            // 
            // radioButtonFatal
            // 
            this.radioButtonFatal.AutoSize = true;
            this.radioButtonFatal.Location = new System.Drawing.Point(6, 92);
            this.radioButtonFatal.Name = "radioButtonFatal";
            this.radioButtonFatal.Size = new System.Drawing.Size(48, 17);
            this.radioButtonFatal.TabIndex = 6;
            this.radioButtonFatal.Text = "Fatal";
            this.radioButtonFatal.UseVisualStyleBackColor = true;
            // 
            // radioButtonDebug
            // 
            this.radioButtonDebug.AutoSize = true;
            this.radioButtonDebug.Location = new System.Drawing.Point(132, 54);
            this.radioButtonDebug.Name = "radioButtonDebug";
            this.radioButtonDebug.Size = new System.Drawing.Size(57, 17);
            this.radioButtonDebug.TabIndex = 5;
            this.radioButtonDebug.Text = "Debug";
            this.radioButtonDebug.UseVisualStyleBackColor = true;
            // 
            // radioButtonWarning
            // 
            this.radioButtonWarning.AutoSize = true;
            this.radioButtonWarning.Location = new System.Drawing.Point(62, 54);
            this.radioButtonWarning.Name = "radioButtonWarning";
            this.radioButtonWarning.Size = new System.Drawing.Size(65, 17);
            this.radioButtonWarning.TabIndex = 4;
            this.radioButtonWarning.Text = "Warning";
            this.radioButtonWarning.UseVisualStyleBackColor = true;
            // 
            // radioButtonPanic
            // 
            this.radioButtonPanic.AutoSize = true;
            this.radioButtonPanic.Location = new System.Drawing.Point(6, 54);
            this.radioButtonPanic.Name = "radioButtonPanic";
            this.radioButtonPanic.Size = new System.Drawing.Size(52, 17);
            this.radioButtonPanic.TabIndex = 3;
            this.radioButtonPanic.Text = "Panic";
            this.radioButtonPanic.UseVisualStyleBackColor = true;
            // 
            // radioButtonVerbose
            // 
            this.radioButtonVerbose.AutoSize = true;
            this.radioButtonVerbose.Location = new System.Drawing.Point(132, 19);
            this.radioButtonVerbose.Name = "radioButtonVerbose";
            this.radioButtonVerbose.Size = new System.Drawing.Size(64, 17);
            this.radioButtonVerbose.TabIndex = 2;
            this.radioButtonVerbose.Text = "Verbose";
            this.radioButtonVerbose.UseVisualStyleBackColor = true;
            // 
            // radioButtonError
            // 
            this.radioButtonError.AutoSize = true;
            this.radioButtonError.Location = new System.Drawing.Point(62, 19);
            this.radioButtonError.Name = "radioButtonError";
            this.radioButtonError.Size = new System.Drawing.Size(47, 17);
            this.radioButtonError.TabIndex = 1;
            this.radioButtonError.Text = "Error";
            this.radioButtonError.UseVisualStyleBackColor = true;
            // 
            // radioButtonQuiet
            // 
            this.radioButtonQuiet.AutoSize = true;
            this.radioButtonQuiet.Location = new System.Drawing.Point(6, 19);
            this.radioButtonQuiet.Name = "radioButtonQuiet";
            this.radioButtonQuiet.Size = new System.Drawing.Size(50, 17);
            this.radioButtonQuiet.TabIndex = 0;
            this.radioButtonQuiet.Text = "Quiet";
            this.radioButtonQuiet.UseVisualStyleBackColor = true;
            // 
            // groupBoxThreadPriority
            // 
            this.groupBoxThreadPriority.Controls.Add(this.radioButtonNormal);
            this.groupBoxThreadPriority.Controls.Add(this.radioButtonLowest);
            this.groupBoxThreadPriority.Controls.Add(this.radioButtonLower);
            this.groupBoxThreadPriority.Controls.Add(this.radioButtonIdle);
            this.groupBoxThreadPriority.Location = new System.Drawing.Point(220, 12);
            this.groupBoxThreadPriority.Name = "groupBoxThreadPriority";
            this.groupBoxThreadPriority.Size = new System.Drawing.Size(142, 86);
            this.groupBoxThreadPriority.TabIndex = 1;
            this.groupBoxThreadPriority.TabStop = false;
            this.groupBoxThreadPriority.Text = "Thread Priority";
            // 
            // radioButtonNormal
            // 
            this.radioButtonNormal.AutoSize = true;
            this.radioButtonNormal.Location = new System.Drawing.Point(65, 54);
            this.radioButtonNormal.Name = "radioButtonNormal";
            this.radioButtonNormal.Size = new System.Drawing.Size(58, 17);
            this.radioButtonNormal.TabIndex = 3;
            this.radioButtonNormal.TabStop = true;
            this.radioButtonNormal.Text = "Normal";
            this.radioButtonNormal.UseVisualStyleBackColor = true;
            // 
            // radioButtonLowest
            // 
            this.radioButtonLowest.AutoSize = true;
            this.radioButtonLowest.Location = new System.Drawing.Point(6, 54);
            this.radioButtonLowest.Name = "radioButtonLowest";
            this.radioButtonLowest.Size = new System.Drawing.Size(59, 17);
            this.radioButtonLowest.TabIndex = 2;
            this.radioButtonLowest.TabStop = true;
            this.radioButtonLowest.Text = "Lowest";
            this.radioButtonLowest.UseVisualStyleBackColor = true;
            // 
            // radioButtonLower
            // 
            this.radioButtonLower.AutoSize = true;
            this.radioButtonLower.Checked = true;
            this.radioButtonLower.Location = new System.Drawing.Point(65, 19);
            this.radioButtonLower.Name = "radioButtonLower";
            this.radioButtonLower.Size = new System.Drawing.Size(54, 17);
            this.radioButtonLower.TabIndex = 1;
            this.radioButtonLower.TabStop = true;
            this.radioButtonLower.Text = "Lower";
            this.radioButtonLower.UseVisualStyleBackColor = true;
            // 
            // radioButtonIdle
            // 
            this.radioButtonIdle.AutoSize = true;
            this.radioButtonIdle.Location = new System.Drawing.Point(6, 19);
            this.radioButtonIdle.Name = "radioButtonIdle";
            this.radioButtonIdle.Size = new System.Drawing.Size(42, 17);
            this.radioButtonIdle.TabIndex = 0;
            this.radioButtonIdle.TabStop = true;
            this.radioButtonIdle.Text = "Idle";
            this.radioButtonIdle.UseVisualStyleBackColor = true;
            // 
            // groupBoxLogPath
            // 
            this.groupBoxLogPath.Controls.Add(this.buttonBrowse);
            this.groupBoxLogPath.Controls.Add(this.textBoxLogPath);
            this.groupBoxLogPath.Location = new System.Drawing.Point(12, 200);
            this.groupBoxLogPath.Name = "groupBoxLogPath";
            this.groupBoxLogPath.Size = new System.Drawing.Size(350, 47);
            this.groupBoxLogPath.TabIndex = 2;
            this.groupBoxLogPath.TabStop = false;
            this.groupBoxLogPath.Text = "Log Path";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(269, 18);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxLogPath
            // 
            this.textBoxLogPath.Location = new System.Drawing.Point(6, 19);
            this.textBoxLogPath.Name = "textBoxLogPath";
            this.textBoxLogPath.ReadOnly = true;
            this.textBoxLogPath.Size = new System.Drawing.Size(257, 20);
            this.textBoxLogPath.TabIndex = 0;
            this.textBoxLogPath.Text = "C:\\Log\\Log.txt";
            // 
            // groupBoxThreadCount
            // 
            this.groupBoxThreadCount.Controls.Add(this.textBoxThreadCount);
            this.groupBoxThreadCount.Location = new System.Drawing.Point(220, 104);
            this.groupBoxThreadCount.Name = "groupBoxThreadCount";
            this.groupBoxThreadCount.Size = new System.Drawing.Size(142, 51);
            this.groupBoxThreadCount.TabIndex = 3;
            this.groupBoxThreadCount.TabStop = false;
            this.groupBoxThreadCount.Text = "Thread Count";
            // 
            // textBoxThreadCount
            // 
            this.textBoxThreadCount.Location = new System.Drawing.Point(7, 19);
            this.textBoxThreadCount.Name = "textBoxThreadCount";
            this.textBoxThreadCount.Size = new System.Drawing.Size(116, 20);
            this.textBoxThreadCount.TabIndex = 0;
            this.textBoxThreadCount.Text = "1";
            // 
            // groupBoxOthers
            // 
            this.groupBoxOthers.Controls.Add(this.radioButtonPreview);
            this.groupBoxOthers.Location = new System.Drawing.Point(12, 139);
            this.groupBoxOthers.Name = "groupBoxOthers";
            this.groupBoxOthers.Size = new System.Drawing.Size(200, 55);
            this.groupBoxOthers.TabIndex = 4;
            this.groupBoxOthers.TabStop = false;
            this.groupBoxOthers.Text = "Others";
            // 
            // radioButtonPreview
            // 
            this.radioButtonPreview.AutoSize = true;
            this.radioButtonPreview.Location = new System.Drawing.Point(6, 19);
            this.radioButtonPreview.Name = "radioButtonPreview";
            this.radioButtonPreview.Size = new System.Drawing.Size(63, 17);
            this.radioButtonPreview.TabIndex = 0;
            this.radioButtonPreview.TabStop = true;
            this.radioButtonPreview.Text = "Preview";
            this.radioButtonPreview.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(64, 264);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(210, 264);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // ConvertSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 301);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxOthers);
            this.Controls.Add(this.groupBoxThreadCount);
            this.Controls.Add(this.groupBoxLogPath);
            this.Controls.Add(this.groupBoxThreadPriority);
            this.Controls.Add(this.groupBoxLogLevel);
            this.MaximizeBox = false;
            this.Name = "ConvertSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ConvertSettings";
            this.groupBoxLogLevel.ResumeLayout(false);
            this.groupBoxLogLevel.PerformLayout();
            this.groupBoxThreadPriority.ResumeLayout(false);
            this.groupBoxThreadPriority.PerformLayout();
            this.groupBoxLogPath.ResumeLayout(false);
            this.groupBoxLogPath.PerformLayout();
            this.groupBoxThreadCount.ResumeLayout(false);
            this.groupBoxThreadCount.PerformLayout();
            this.groupBoxOthers.ResumeLayout(false);
            this.groupBoxOthers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxLogLevel;
        private System.Windows.Forms.GroupBox groupBoxThreadPriority;
        private System.Windows.Forms.RadioButton radioButtonQuiet;
        private System.Windows.Forms.RadioButton radioButtonInfo;
        private System.Windows.Forms.RadioButton radioButtonFatal;
        private System.Windows.Forms.RadioButton radioButtonDebug;
        private System.Windows.Forms.RadioButton radioButtonWarning;
        private System.Windows.Forms.RadioButton radioButtonPanic;
        private System.Windows.Forms.RadioButton radioButtonVerbose;
        private System.Windows.Forms.RadioButton radioButtonError;
        private System.Windows.Forms.GroupBox groupBoxLogPath;
        private System.Windows.Forms.TextBox textBoxLogPath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.RadioButton radioButtonIdle;
        private System.Windows.Forms.RadioButton radioButtonNormal;
        private System.Windows.Forms.RadioButton radioButtonLowest;
        private System.Windows.Forms.RadioButton radioButtonLower;
        private System.Windows.Forms.GroupBox groupBoxThreadCount;
        private System.Windows.Forms.TextBox textBoxThreadCount;
        private System.Windows.Forms.GroupBox groupBoxOthers;
        private System.Windows.Forms.RadioButton radioButtonPreview;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}