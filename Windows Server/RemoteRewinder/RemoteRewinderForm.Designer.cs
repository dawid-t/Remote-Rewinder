namespace RemoteRewinder
{
	partial class RemoteRewinderForm
	{
		/// <summary>
		/// Wymagana zmienna projektanta.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Wyczyść wszystkie używane zasoby.
		/// </summary>
		/// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Kod generowany przez Projektanta formularzy systemu Windows

		/// <summary>
		/// Metoda wymagana do obsługi projektanta — nie należy modyfikować
		/// jej zawartości w edytorze kodu.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel_Main = new System.Windows.Forms.Panel();
			this.panel_Message = new System.Windows.Forms.Panel();
			this.textBox_Message = new System.Windows.Forms.TextBox();
			this.button_Message = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label_With = new System.Windows.Forms.Label();
			this.label_Status = new System.Windows.Forms.Label();
			this.panel_ChangeKey = new System.Windows.Forms.Panel();
			this.richTextBox_ChangeKeyPressed = new System.Windows.Forms.RichTextBox();
			this.label_ChangeKeyInfo = new System.Windows.Forms.Label();
			this.button_ChangeKeyCancel = new System.Windows.Forms.Button();
			this.button_ChangeKeyOK = new System.Windows.Forms.Button();
			this.button_Sound = new System.Windows.Forms.Button();
			this.label_plus = new System.Windows.Forms.Label();
			this.label_minus = new System.Windows.Forms.Label();
			this.button_RemoteKeyPlus = new System.Windows.Forms.Button();
			this.button_RemoteKeyMinus = new System.Windows.Forms.Button();
			this.label_Controls = new System.Windows.Forms.Label();
			this.button_On = new System.Windows.Forms.Button();
			this.textBox_Port = new System.Windows.Forms.TextBox();
			this.label_Port = new System.Windows.Forms.Label();
			this.panel_Main.SuspendLayout();
			this.panel_Message.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel_ChangeKey.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel_Main
			// 
			this.panel_Main.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel_Main.BackColor = System.Drawing.Color.Transparent;
			this.panel_Main.Controls.Add(this.panel_Message);
			this.panel_Main.Controls.Add(this.panel1);
			this.panel_Main.Controls.Add(this.panel_ChangeKey);
			this.panel_Main.Controls.Add(this.button_Sound);
			this.panel_Main.Controls.Add(this.label_plus);
			this.panel_Main.Controls.Add(this.label_minus);
			this.panel_Main.Controls.Add(this.button_RemoteKeyPlus);
			this.panel_Main.Controls.Add(this.button_RemoteKeyMinus);
			this.panel_Main.Controls.Add(this.label_Controls);
			this.panel_Main.Controls.Add(this.button_On);
			this.panel_Main.Controls.Add(this.textBox_Port);
			this.panel_Main.Controls.Add(this.label_Port);
			this.panel_Main.ForeColor = System.Drawing.SystemColors.ControlText;
			this.panel_Main.Location = new System.Drawing.Point(0, 0);
			this.panel_Main.Name = "panel_Main";
			this.panel_Main.Size = new System.Drawing.Size(330, 163);
			this.panel_Main.TabIndex = 0;
			// 
			// panel_Message
			// 
			this.panel_Message.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel_Message.BackColor = System.Drawing.Color.Transparent;
			this.panel_Message.Controls.Add(this.textBox_Message);
			this.panel_Message.Controls.Add(this.button_Message);
			this.panel_Message.Location = new System.Drawing.Point(78, 62);
			this.panel_Message.Name = "panel_Message";
			this.panel_Message.Size = new System.Drawing.Size(180, 74);
			this.panel_Message.TabIndex = 8;
			this.panel_Message.Visible = false;
			// 
			// textBox_Message
			// 
			this.textBox_Message.BackColor = System.Drawing.Color.White;
			this.textBox_Message.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_Message.Enabled = false;
			this.textBox_Message.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.textBox_Message.ForeColor = System.Drawing.Color.Black;
			this.textBox_Message.Location = new System.Drawing.Point(20, 10);
			this.textBox_Message.Multiline = true;
			this.textBox_Message.Name = "textBox_Message";
			this.textBox_Message.Size = new System.Drawing.Size(140, 32);
			this.textBox_Message.TabIndex = 2;
			this.textBox_Message.Text = "-";
			this.textBox_Message.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// button_Message
			// 
			this.button_Message.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button_Message.Location = new System.Drawing.Point(51, 48);
			this.button_Message.Name = "button_Message";
			this.button_Message.Size = new System.Drawing.Size(75, 23);
			this.button_Message.TabIndex = 0;
			this.button_Message.Text = "OK";
			this.button_Message.UseVisualStyleBackColor = true;
			this.button_Message.Click += new System.EventHandler(this.ButtonMessage);
			// 
			// panel1
			// 
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panel1.Controls.Add(this.label_With);
			this.panel1.Controls.Add(this.label_Status);
			this.panel1.Location = new System.Drawing.Point(102, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(140, 48);
			this.panel1.TabIndex = 13;
			// 
			// label_With
			// 
			this.label_With.ForeColor = System.Drawing.Color.White;
			this.label_With.Location = new System.Drawing.Point(55, 20);
			this.label_With.Name = "label_With";
			this.label_With.Size = new System.Drawing.Size(26, 13);
			this.label_With.TabIndex = 4;
			this.label_With.Text = "with";
			this.label_With.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label_With.Visible = false;
			// 
			// label_Status
			// 
			this.label_Status.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.label_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_Status.ForeColor = System.Drawing.Color.OrangeRed;
			this.label_Status.Location = new System.Drawing.Point(0, 5);
			this.label_Status.Name = "label_Status";
			this.label_Status.Size = new System.Drawing.Size(140, 17);
			this.label_Status.TabIndex = 3;
			this.label_Status.Text = "Offline";
			this.label_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel_ChangeKey
			// 
			this.panel_ChangeKey.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel_ChangeKey.BackColor = System.Drawing.Color.Transparent;
			this.panel_ChangeKey.Controls.Add(this.richTextBox_ChangeKeyPressed);
			this.panel_ChangeKey.Controls.Add(this.label_ChangeKeyInfo);
			this.panel_ChangeKey.Controls.Add(this.button_ChangeKeyCancel);
			this.panel_ChangeKey.Controls.Add(this.button_ChangeKeyOK);
			this.panel_ChangeKey.Location = new System.Drawing.Point(78, 62);
			this.panel_ChangeKey.Name = "panel_ChangeKey";
			this.panel_ChangeKey.Size = new System.Drawing.Size(180, 74);
			this.panel_ChangeKey.TabIndex = 9;
			this.panel_ChangeKey.Visible = false;
			// 
			// richTextBox_ChangeKeyPressed
			// 
			this.richTextBox_ChangeKeyPressed.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox_ChangeKeyPressed.Enabled = false;
			this.richTextBox_ChangeKeyPressed.Location = new System.Drawing.Point(10, 28);
			this.richTextBox_ChangeKeyPressed.Multiline = false;
			this.richTextBox_ChangeKeyPressed.Name = "richTextBox_ChangeKeyPressed";
			this.richTextBox_ChangeKeyPressed.ReadOnly = true;
			this.richTextBox_ChangeKeyPressed.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox_ChangeKeyPressed.Size = new System.Drawing.Size(160, 13);
			this.richTextBox_ChangeKeyPressed.TabIndex = 6;
			this.richTextBox_ChangeKeyPressed.Text = "-";
			// 
			// label_ChangeKeyInfo
			// 
			this.label_ChangeKeyInfo.AutoSize = true;
			this.label_ChangeKeyInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_ChangeKeyInfo.ForeColor = System.Drawing.Color.White;
			this.label_ChangeKeyInfo.Location = new System.Drawing.Point(19, 6);
			this.label_ChangeKeyInfo.Name = "label_ChangeKeyInfo";
			this.label_ChangeKeyInfo.Size = new System.Drawing.Size(142, 15);
			this.label_ChangeKeyInfo.TabIndex = 4;
			this.label_ChangeKeyInfo.Text = "Press any key to bind";
			this.label_ChangeKeyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_ChangeKeyCancel
			// 
			this.button_ChangeKeyCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button_ChangeKeyCancel.Location = new System.Drawing.Point(164, 0);
			this.button_ChangeKeyCancel.Name = "button_ChangeKeyCancel";
			this.button_ChangeKeyCancel.Size = new System.Drawing.Size(16, 16);
			this.button_ChangeKeyCancel.TabIndex = 3;
			this.button_ChangeKeyCancel.Text = "X";
			this.button_ChangeKeyCancel.UseVisualStyleBackColor = true;
			this.button_ChangeKeyCancel.Click += new System.EventHandler(this.ButtonChangeKeyCancel);
			// 
			// button_ChangeKeyOK
			// 
			this.button_ChangeKeyOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button_ChangeKeyOK.Location = new System.Drawing.Point(51, 48);
			this.button_ChangeKeyOK.Name = "button_ChangeKeyOK";
			this.button_ChangeKeyOK.Size = new System.Drawing.Size(75, 23);
			this.button_ChangeKeyOK.TabIndex = 0;
			this.button_ChangeKeyOK.Text = "OK";
			this.button_ChangeKeyOK.UseVisualStyleBackColor = true;
			this.button_ChangeKeyOK.Click += new System.EventHandler(this.ButtonChangeKeyOK);
			// 
			// button_Sound
			// 
			this.button_Sound.Image = global::RemoteRewinder.Properties.Resources.SoundOn;
			this.button_Sound.Location = new System.Drawing.Point(296, 5);
			this.button_Sound.Name = "button_Sound";
			this.button_Sound.Size = new System.Drawing.Size(24, 18);
			this.button_Sound.TabIndex = 12;
			this.button_Sound.UseVisualStyleBackColor = true;
			this.button_Sound.Click += new System.EventHandler(this.ButtonSound);
			// 
			// label_plus
			// 
			this.label_plus.AutoSize = true;
			this.label_plus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_plus.ForeColor = System.Drawing.Color.White;
			this.label_plus.Location = new System.Drawing.Point(264, 108);
			this.label_plus.Name = "label_plus";
			this.label_plus.Size = new System.Drawing.Size(13, 13);
			this.label_plus.TabIndex = 11;
			this.label_plus.Text = "+";
			this.label_plus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_minus
			// 
			this.label_minus.AutoSize = true;
			this.label_minus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_minus.ForeColor = System.Drawing.Color.White;
			this.label_minus.Location = new System.Drawing.Point(165, 108);
			this.label_minus.Name = "label_minus";
			this.label_minus.Size = new System.Drawing.Size(11, 13);
			this.label_minus.TabIndex = 10;
			this.label_minus.Text = "-";
			this.label_minus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_RemoteKeyPlus
			// 
			this.button_RemoteKeyPlus.AutoEllipsis = true;
			this.button_RemoteKeyPlus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button_RemoteKeyPlus.FlatAppearance.BorderSize = 0;
			this.button_RemoteKeyPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button_RemoteKeyPlus.ForeColor = System.Drawing.SystemColors.ControlText;
			this.button_RemoteKeyPlus.Location = new System.Drawing.Point(224, 122);
			this.button_RemoteKeyPlus.Name = "button_RemoteKeyPlus";
			this.button_RemoteKeyPlus.Size = new System.Drawing.Size(94, 23);
			this.button_RemoteKeyPlus.TabIndex = 7;
			this.button_RemoteKeyPlus.Text = "RIGHT";
			this.button_RemoteKeyPlus.UseVisualStyleBackColor = true;
			this.button_RemoteKeyPlus.Click += new System.EventHandler(this.ButtonRemoteKeyPlus);
			// 
			// button_RemoteKeyMinus
			// 
			this.button_RemoteKeyMinus.AutoEllipsis = true;
			this.button_RemoteKeyMinus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button_RemoteKeyMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button_RemoteKeyMinus.ForeColor = System.Drawing.SystemColors.ControlText;
			this.button_RemoteKeyMinus.Location = new System.Drawing.Point(124, 122);
			this.button_RemoteKeyMinus.Name = "button_RemoteKeyMinus";
			this.button_RemoteKeyMinus.Size = new System.Drawing.Size(94, 23);
			this.button_RemoteKeyMinus.TabIndex = 6;
			this.button_RemoteKeyMinus.Text = "LEFT";
			this.button_RemoteKeyMinus.UseVisualStyleBackColor = true;
			this.button_RemoteKeyMinus.Click += new System.EventHandler(this.ButtonRemoteKeyMinus);
			// 
			// label_Controls
			// 
			this.label_Controls.AutoSize = true;
			this.label_Controls.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_Controls.ForeColor = System.Drawing.Color.White;
			this.label_Controls.Location = new System.Drawing.Point(163, 93);
			this.label_Controls.Name = "label_Controls";
			this.label_Controls.Size = new System.Drawing.Size(114, 15);
			this.label_Controls.TabIndex = 5;
			this.label_Controls.Text = "Remote Controls";
			this.label_Controls.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_On
			// 
			this.button_On.BackColor = System.Drawing.Color.Transparent;
			this.button_On.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.button_On.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button_On.ForeColor = System.Drawing.SystemColors.ControlText;
			this.button_On.Location = new System.Drawing.Point(12, 122);
			this.button_On.Name = "button_On";
			this.button_On.Size = new System.Drawing.Size(75, 23);
			this.button_On.TabIndex = 4;
			this.button_On.Text = global::RemoteRewinder.Properties.Strings.On;
			this.button_On.UseVisualStyleBackColor = false;
			this.button_On.Click += new System.EventHandler(this.ButtonOn);
			// 
			// textBox_Port
			// 
			this.textBox_Port.Location = new System.Drawing.Point(43, 95);
			this.textBox_Port.MaxLength = 5;
			this.textBox_Port.Name = "textBox_Port";
			this.textBox_Port.Size = new System.Drawing.Size(43, 20);
			this.textBox_Port.TabIndex = 1;
			this.textBox_Port.Text = "6800";
			this.textBox_Port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBox_Port.TextChanged += new System.EventHandler(this.TextBox_Port_TextChanged);
			// 
			// label_Port
			// 
			this.label_Port.AutoSize = true;
			this.label_Port.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_Port.ForeColor = System.Drawing.Color.White;
			this.label_Port.Location = new System.Drawing.Point(9, 98);
			this.label_Port.Name = "label_Port";
			this.label_Port.Size = new System.Drawing.Size(34, 13);
			this.label_Port.TabIndex = 0;
			this.label_Port.Text = "Port:";
			// 
			// RemoteRewinderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
			this.ClientSize = new System.Drawing.Size(330, 161);
			this.Controls.Add(this.panel_Main);
			this.MaximizeBox = false;
			this.Name = "RemoteRewinderForm";
			this.Text = "Remote Rewinder";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RemoteRewinderFormClosing);
			this.panel_Main.ResumeLayout(false);
			this.panel_Main.PerformLayout();
			this.panel_Message.ResumeLayout(false);
			this.panel_Message.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel_ChangeKey.ResumeLayout(false);
			this.panel_ChangeKey.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel_Main;
		private System.Windows.Forms.Label label_Port;
		private System.Windows.Forms.TextBox textBox_Port;
		private System.Windows.Forms.Button button_On;
		private System.Windows.Forms.Label label_Controls;
		private System.Windows.Forms.Button button_RemoteKeyPlus;
		private System.Windows.Forms.Button button_RemoteKeyMinus;
		private System.Windows.Forms.Panel panel_Message;
		private System.Windows.Forms.Button button_Message;
		private System.Windows.Forms.TextBox textBox_Message;
		private System.Windows.Forms.Panel panel_ChangeKey;
		private System.Windows.Forms.Button button_ChangeKeyCancel;
		private System.Windows.Forms.Button button_ChangeKeyOK;
		private System.Windows.Forms.Label label_ChangeKeyInfo;
		private System.Windows.Forms.Label label_plus;
		private System.Windows.Forms.Label label_minus;
		private System.Windows.Forms.RichTextBox richTextBox_ChangeKeyPressed;
		private System.Windows.Forms.Button button_Sound;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label_With;
		private System.Windows.Forms.Label label_Status;
	}
}

