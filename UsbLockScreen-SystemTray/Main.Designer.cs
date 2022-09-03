namespace UsbStartupKey_SystemTray
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.pictureBox_logo = new System.Windows.Forms.PictureBox();
            this.textBox_serial = new System.Windows.Forms.TextBox();
            this.textBox_pin = new System.Windows.Forms.TextBox();
            this.label_Serial = new System.Windows.Forms.Label();
            this.label_Pin = new System.Windows.Forms.Label();
            this.btn_save = new System.Windows.Forms.Button();
            this.notifyIcon_main = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_logo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_logo
            // 
            this.pictureBox_logo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_logo.Image")));
            this.pictureBox_logo.Location = new System.Drawing.Point(132, 30);
            this.pictureBox_logo.Name = "pictureBox_logo";
            this.pictureBox_logo.Size = new System.Drawing.Size(120, 130);
            this.pictureBox_logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_logo.TabIndex = 5;
            this.pictureBox_logo.TabStop = false;
            // 
            // textBox_serial
            // 
            this.textBox_serial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_serial.Font = new System.Drawing.Font("Roboto Slab", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_serial.Location = new System.Drawing.Point(42, 360);
            this.textBox_serial.Name = "textBox_serial";
            this.textBox_serial.Size = new System.Drawing.Size(300, 29);
            this.textBox_serial.TabIndex = 4;
            this.textBox_serial.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_pin
            // 
            this.textBox_pin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_pin.Font = new System.Drawing.Font("Roboto Slab", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_pin.Location = new System.Drawing.Point(42, 275);
            this.textBox_pin.Name = "textBox_pin";
            this.textBox_pin.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBox_pin.Size = new System.Drawing.Size(300, 29);
            this.textBox_pin.TabIndex = 3;
            this.textBox_pin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_pin.UseSystemPasswordChar = true;
            // 
            // label_Serial
            // 
            this.label_Serial.AutoSize = true;
            this.label_Serial.Font = new System.Drawing.Font("Roboto Slab", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Serial.ForeColor = System.Drawing.Color.White;
            this.label_Serial.Location = new System.Drawing.Point(38, 336);
            this.label_Serial.Name = "label_Serial";
            this.label_Serial.Size = new System.Drawing.Size(61, 21);
            this.label_Serial.TabIndex = 2;
            this.label_Serial.Text = "Serial";
            // 
            // label_Pin
            // 
            this.label_Pin.AutoSize = true;
            this.label_Pin.Font = new System.Drawing.Font("Roboto Slab", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Pin.ForeColor = System.Drawing.Color.White;
            this.label_Pin.Location = new System.Drawing.Point(38, 251);
            this.label_Pin.Name = "label_Pin";
            this.label_Pin.Size = new System.Drawing.Size(40, 21);
            this.label_Pin.TabIndex = 1;
            this.label_Pin.Text = "PIN";
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(177)))), ((int)(((byte)(136)))));
            this.btn_save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_save.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_save.Font = new System.Drawing.Font("Roboto Slab", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btn_save.ForeColor = System.Drawing.Color.White;
            this.btn_save.Location = new System.Drawing.Point(42, 470);
            this.btn_save.Margin = new System.Windows.Forms.Padding(0);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(300, 50);
            this.btn_save.TabIndex = 0;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // notifyIcon_main
            // 
            this.notifyIcon_main.BalloonTipTitle = "USB Lock Screen";
            this.notifyIcon_main.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_main.Icon")));
            this.notifyIcon_main.Text = "USB Lock Screen";
            this.notifyIcon_main.Visible = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(49)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(384, 561);
            this.Controls.Add(this.pictureBox_logo);
            this.Controls.Add(this.textBox_serial);
            this.Controls.Add(this.textBox_pin);
            this.Controls.Add(this.label_Serial);
            this.Controls.Add(this.label_Pin);
            this.Controls.Add(this.btn_save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "USB Lock Screen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox_logo;
        private System.Windows.Forms.TextBox textBox_serial;
        private System.Windows.Forms.TextBox textBox_pin;
        private System.Windows.Forms.Label label_Serial;
        private System.Windows.Forms.Label label_Pin;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.NotifyIcon notifyIcon_main;
    }
}

