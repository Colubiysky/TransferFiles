namespace TransferFiles
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lst_Computers = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CheckOnline_btn = new System.Windows.Forms.Button();
            this.DropFile_btn = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lst_Computers
            // 
            this.lst_Computers.FormattingEnabled = true;
            this.lst_Computers.ItemHeight = 20;
            this.lst_Computers.Location = new System.Drawing.Point(15, 43);
            this.lst_Computers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lst_Computers.Name = "lst_Computers";
            this.lst_Computers.Size = new System.Drawing.Size(289, 424);
            this.lst_Computers.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Online devices";
            // 
            // CheckOnline_btn
            // 
            this.CheckOnline_btn.Location = new System.Drawing.Point(326, 43);
            this.CheckOnline_btn.Name = "CheckOnline_btn";
            this.CheckOnline_btn.Size = new System.Drawing.Size(176, 81);
            this.CheckOnline_btn.TabIndex = 4;
            this.CheckOnline_btn.Text = "Check Online";
            this.CheckOnline_btn.UseVisualStyleBackColor = true;
            this.CheckOnline_btn.Click += new System.EventHandler(this.CheckOnline_btn_Click);
            // 
            // DropFile_btn
            // 
            this.DropFile_btn.Location = new System.Drawing.Point(326, 131);
            this.DropFile_btn.Name = "DropFile_btn";
            this.DropFile_btn.Size = new System.Drawing.Size(176, 82);
            this.DropFile_btn.TabIndex = 5;
            this.DropFile_btn.Text = "Drop file";
            this.DropFile_btn.UseVisualStyleBackColor = true;
            this.DropFile_btn.Click += new System.EventHandler(this.DropFile_btn_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(569, 43);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(50, 20);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "label2";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 506);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.DropFile_btn);
            this.Controls.Add(this.CheckOnline_btn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lst_Computers);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lst_Computers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CheckOnline_btn;
        private System.Windows.Forms.Button DropFile_btn;
        private System.Windows.Forms.Label lblStatus;
    }
}

