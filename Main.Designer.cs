namespace DtcRemover
{
    partial class DtcRemover
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.gbControl = new System.Windows.Forms.GroupBox();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbRemoveDtc = new System.Windows.Forms.TextBox();
            this.btnRemoveDtc = new System.Windows.Forms.Button();
            this.gpbMain = new System.Windows.Forms.GroupBox();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.gpbAvailableCodes = new System.Windows.Forms.GroupBox();
            this.dgvAvailableCodes = new System.Windows.Forms.DataGridView();
            this.btnOpenDtc = new System.Windows.Forms.Button();
            this.btnCloseFile = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.gbControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gpbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.gpbAvailableCodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableCodes)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.gbControl, 0, 0);
            this.tlpMain.Controls.Add(this.groupBox1, 1, 0);
            this.tlpMain.Controls.Add(this.gpbMain, 0, 1);
            this.tlpMain.Controls.Add(this.gpbAvailableCodes, 1, 1);
            this.tlpMain.Location = new System.Drawing.Point(12, 12);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.14554F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.85446F));
            this.tlpMain.Size = new System.Drawing.Size(776, 426);
            this.tlpMain.TabIndex = 0;
            // 
            // gbControl
            // 
            this.gbControl.Controls.Add(this.btnCloseFile);
            this.gbControl.Controls.Add(this.btnSaveFile);
            this.gbControl.Controls.Add(this.btnOpenFile);
            this.gbControl.Location = new System.Drawing.Point(3, 3);
            this.gbControl.Name = "gbControl";
            this.gbControl.Size = new System.Drawing.Size(382, 50);
            this.gbControl.TabIndex = 0;
            this.gbControl.TabStop = false;
            this.gbControl.Text = "Control";
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Enabled = false;
            this.btnSaveFile.Location = new System.Drawing.Point(87, 19);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(75, 23);
            this.btnSaveFile.TabIndex = 1;
            this.btnSaveFile.Text = "Save File";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(6, 19);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOpenDtc);
            this.groupBox1.Controls.Add(this.tbRemoveDtc);
            this.groupBox1.Controls.Add(this.btnRemoveDtc);
            this.groupBox1.Location = new System.Drawing.Point(391, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 50);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "P-Code";
            // 
            // tbRemoveDtc
            // 
            this.tbRemoveDtc.Location = new System.Drawing.Point(6, 23);
            this.tbRemoveDtc.Name = "tbRemoveDtc";
            this.tbRemoveDtc.Size = new System.Drawing.Size(100, 20);
            this.tbRemoveDtc.TabIndex = 3;
            this.tbRemoveDtc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRemoveDtc_KeyPress);
            // 
            // btnRemoveDtc
            // 
            this.btnRemoveDtc.Enabled = false;
            this.btnRemoveDtc.Location = new System.Drawing.Point(112, 21);
            this.btnRemoveDtc.Name = "btnRemoveDtc";
            this.btnRemoveDtc.Size = new System.Drawing.Size(81, 23);
            this.btnRemoveDtc.TabIndex = 2;
            this.btnRemoveDtc.Text = "Remove DTC";
            this.btnRemoveDtc.UseVisualStyleBackColor = true;
            this.btnRemoveDtc.Click += new System.EventHandler(this.btnRemoveDtc_Click);
            // 
            // gpbMain
            // 
            this.gpbMain.Controls.Add(this.dgvMain);
            this.gpbMain.Location = new System.Drawing.Point(3, 59);
            this.gpbMain.Name = "gpbMain";
            this.gpbMain.Size = new System.Drawing.Size(382, 364);
            this.gpbMain.TabIndex = 4;
            this.gpbMain.TabStop = false;
            this.gpbMain.Text = "Removed Codes";
            // 
            // dgvMain
            // 
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Location = new System.Drawing.Point(6, 19);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.Size = new System.Drawing.Size(370, 339);
            this.dgvMain.TabIndex = 2;
            // 
            // gpbAvailableCodes
            // 
            this.gpbAvailableCodes.Controls.Add(this.dgvAvailableCodes);
            this.gpbAvailableCodes.Location = new System.Drawing.Point(391, 59);
            this.gpbAvailableCodes.Name = "gpbAvailableCodes";
            this.gpbAvailableCodes.Size = new System.Drawing.Size(382, 364);
            this.gpbAvailableCodes.TabIndex = 5;
            this.gpbAvailableCodes.TabStop = false;
            this.gpbAvailableCodes.Text = "Available Codes in Software";
            // 
            // dgvAvailableCodes
            // 
            this.dgvAvailableCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAvailableCodes.Location = new System.Drawing.Point(6, 19);
            this.dgvAvailableCodes.Name = "dgvAvailableCodes";
            this.dgvAvailableCodes.Size = new System.Drawing.Size(370, 339);
            this.dgvAvailableCodes.TabIndex = 4;
            // 
            // btnOpenDtc
            // 
            this.btnOpenDtc.Enabled = false;
            this.btnOpenDtc.Location = new System.Drawing.Point(199, 21);
            this.btnOpenDtc.Name = "btnOpenDtc";
            this.btnOpenDtc.Size = new System.Drawing.Size(86, 23);
            this.btnOpenDtc.TabIndex = 4;
            this.btnOpenDtc.Text = "Open DTC List";
            this.btnOpenDtc.UseVisualStyleBackColor = true;
            this.btnOpenDtc.Click += new System.EventHandler(this.btnOpenDtc_Click);
            // 
            // btnCloseFile
            // 
            this.btnCloseFile.Enabled = false;
            this.btnCloseFile.Location = new System.Drawing.Point(168, 19);
            this.btnCloseFile.Name = "btnCloseFile";
            this.btnCloseFile.Size = new System.Drawing.Size(75, 23);
            this.btnCloseFile.TabIndex = 2;
            this.btnCloseFile.Text = "Close File";
            this.btnCloseFile.UseVisualStyleBackColor = true;
            this.btnCloseFile.Click += new System.EventHandler(this.btnCloseFile_Click);
            // 
            // DtcRemover
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tlpMain);
            this.MaximumSize = new System.Drawing.Size(816, 489);
            this.MinimumSize = new System.Drawing.Size(816, 489);
            this.Name = "DtcRemover";
            this.Text = "DTC Remover by Erick Bastiaannet";
            this.tlpMain.ResumeLayout(false);
            this.gbControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gpbMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.gpbAvailableCodes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableCodes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbRemoveDtc;
        private System.Windows.Forms.Button btnRemoveDtc;
        private System.Windows.Forms.GroupBox gpbMain;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.GroupBox gpbAvailableCodes;
        private System.Windows.Forms.DataGridView dgvAvailableCodes;
        private System.Windows.Forms.Button btnOpenDtc;
        private System.Windows.Forms.Button btnCloseFile;
    }
}

