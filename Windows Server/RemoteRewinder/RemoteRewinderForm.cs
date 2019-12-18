using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteRewinder
{
	public partial class RemoteRewinderForm : Form
	{
		private bool soundOn = true;
		private int lastPressedKeyValue, selectedBind = 0;
		private string language, lastCorrectPort, lastPressedKeyCode, lastPressedSpecialKey;
		private string[] richTextBoxValues = new string[6]; // 0-1 first bind, 2-3 second bind, 4-5 third bind.
		private ToolTip toolTip;
		private KeysController keysController;
		private Communication communication;
		private SoundPlayer soundPlayer;
		private ConnectingStatus connectingStatus = ConnectingStatus.Offline;

		public int SelectedBind { get { return selectedBind; } }

		//public enum Language { English, Polish }
		public enum ConnectingStatus { Offline, Waiting, Online, }


		public RemoteRewinderForm()
		{
			InitializeComponent();
			Init();
		}

		private void Init()
		{
			lastCorrectPort = textBox_Port.Text;
			toolTip = new ToolTip();
			AddToolTips();
			richTextBox_ChangeKeyPressed.SelectionAlignment = HorizontalAlignment.Center;

			panel_ChangeKey.PreviewKeyDown += new PreviewKeyDownEventHandler(ChangeKeyPanelKeyDown);
			button_ChangeKeyOK.GotFocus += new EventHandler(ChangeKeyButtonGotFocus);
			button_ChangeKeyCancel.GotFocus += new EventHandler(ChangeKeyButtonGotFocus);

			// Load data here (will be expanded in the next update):
			string[] minusKeys = null, plusKeys = null;
			/*if(true) // Is file.
			{

			}
			else
			{

			}*/
			//ChangeLanguage();

			keysController = new KeysController(this, minusKeys, plusKeys);
			communication = new Communication(keysController, this, label_Status, textBox_Port);
			soundPlayer = new SoundPlayer();
		}

		#region Events:
		private void ChangeKeyPanelKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			panel_ChangeKey.Focus(); // Alt and f10 lose focus on the panel.
			if(e.KeyValue < 16 || e.KeyValue > 18) // Shift = 16, Ctrl = 17, Alt = 18.
			{
				string pressedKeys = @"{\colortbl ;\red220\green0\blue0;}";

				#region Special keys:
				lastPressedSpecialKey = ""; // Need when user click "OK" button.
				if(e.Shift)
				{
					lastPressedSpecialKey = "+";
					pressedKeys += @"SHIFT \cf1\b+\b0\cf0  ";
				}
				if(e.Control)
				{
					lastPressedSpecialKey += "^";
					pressedKeys += @"CTRL \cf1\b+\b0\cf0  ";
				}
				if(e.Alt) // Control + left Alt == right Alt.
				{
					lastPressedSpecialKey += "%";
					pressedKeys += @"ALT \cf1\b+\b0\cf0  ";
				}
				#endregion Special keys.

				lastPressedKeyCode = keysController.TranslatePressedKey(e.KeyCode.ToString(), e.KeyValue); // Need when user clicks "OK" button.
				lastPressedKeyValue = e.KeyValue; // Need when user clicks "OK" button.

				pressedKeys += lastPressedKeyCode.Replace("{", "").Replace("}", "").Replace(" ", "SPACE"); // Remove brackets to show user the correct pressed key.
				richTextBox_ChangeKeyPressed.Rtf = @"{\rtf1\ansi "+pressedKeys+"}";
				richTextBox_ChangeKeyPressed.SelectionAlignment = HorizontalAlignment.Center;
			}
		}

		private void ChangeKeyButtonGotFocus(object sender, EventArgs e)
		{
			panel_ChangeKey.Focus();
		}

		private void TextBox_Port_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex("^[0-9]{0,5}$");
			if(regex.IsMatch(textBox_Port.Text))
			{
				lastCorrectPort = textBox_Port.Text;
			}
			else
			{
				textBox_Port.Text = lastCorrectPort;
			}
		}

		private void RemoteRewinderFormClosing(object sender, FormClosingEventArgs e) // Save data while app is closing.
		{
			//SavedData.SaveData("");
		}
		#endregion Events.

		#region UI:
		public void ShowMessage(string message)
		{
			textBox_Message.Invoke((MethodInvoker)delegate
			{
				textBox_Message.Text = message;
				panel_ChangeKey.Visible = false;
				panel_Message.Visible = true;
				BlockUI(false);
			});
		}

		private void BlockUI(bool unblock)
		{
			button_On.Enabled = unblock;
			textBox_Port.Enabled = unblock;
			button_RemoteKeyMinus.Enabled = unblock;
			button_RemoteKeyPlus.Enabled = unblock;
		}

		public void ChangeButtonOnText(string text, bool readOnly)
		{
			button_On.Invoke((MethodInvoker)delegate
			{
				button_On.Text = text;
				toolTip.SetToolTip(button_On, text+" "+Properties.Strings.Server);
				textBox_Port.ReadOnly = readOnly;
			});
		}

		public void ChangeStatus(ConnectingStatus status)
		{
			label_Status.Invoke((MethodInvoker)delegate
			{
				switch(status)
				{
					case ConnectingStatus.Offline:
						label_Status.Text = Properties.Strings.Offline;
						label_Status.ForeColor = System.Drawing.Color.Red;
						break;
					case ConnectingStatus.Waiting:
						label_Status.Text = Properties.Strings.Waiting;
						label_Status.ForeColor = System.Drawing.Color.Gold;
						break;
					case ConnectingStatus.Online:
						label_Status.Text = Properties.Strings.Connected;
						label_Status.ForeColor = System.Drawing.Color.Green;
						break;
				}
				connectingStatus = status;
			});
		}
		
		private void ChangeLanguage()
		{
			string language = Thread.CurrentThread.CurrentUICulture.ToString();
			if(language.Equals("pl-PL"))
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
			}
			else
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
			}

			#region Refresh UI:
			switch(connectingStatus)
			{
				case ConnectingStatus.Offline:
					label_Status.Text = Properties.Strings.Offline;
					ChangeButtonOnText(Properties.Strings.On, false);
					break;
				case ConnectingStatus.Waiting:
					label_Status.Text = Properties.Strings.Waiting;
					ChangeButtonOnText(Properties.Strings.Off, true);
					break;
				case ConnectingStatus.Online:
					label_Status.Text = Properties.Strings.Connected;
					ChangeButtonOnText(Properties.Strings.Off, true);
					break;
			}
			AddToolTips();
			label_Controls.Text = Properties.Strings.RemoteControls;
			label_ChangeKeyInfo.Text = Properties.Strings.ChangeKeyInfo;
			#endregion Refresh UI.
		}

		private void AddToolTips()
		{
			toolTip.SetToolTip(label_Port, Properties.Strings.PortsRange);
			toolTip.SetToolTip(textBox_Port, Properties.Strings.PortsRange);
			toolTip.SetToolTip(button_On, Properties.Strings.On+" "+Properties.Strings.Server);
		}
		#endregion UI.

		#region Buttons:
		private void ButtonOn(object sender, EventArgs e)
		{
			if(communication.Socket == null)
			{
				bool isListenning = communication.StartListenning();
				if(isListenning)
				{
					ChangeButtonOnText(Properties.Strings.Off, true);
				}
			}
			else
			{
				communication.StopListenning();
				ChangeButtonOnText(Properties.Strings.On, false);
			}
		}

		private void ButtonRemoteKeyMinus(object sender, EventArgs e)
		{
			keysController.MinusKeyIsPressed = true;
			richTextBox_ChangeKeyPressed.Rtf = richTextBoxValues[selectedBind];
			richTextBox_ChangeKeyPressed.SelectionAlignment = HorizontalAlignment.Center;

			panel_ChangeKey.Visible = true;
			BlockUI(false);
		}

		private void ButtonRemoteKeyPlus(object sender, EventArgs e)
		{
			keysController.MinusKeyIsPressed = false;
			richTextBox_ChangeKeyPressed.Rtf = richTextBoxValues[selectedBind+1];
			richTextBox_ChangeKeyPressed.SelectionAlignment = HorizontalAlignment.Center;

			panel_ChangeKey.Visible = true;
			BlockUI(false);
		}

		private void ButtonSound(object sender, EventArgs e)
		{
			if(soundOn)
			{
				button_Sound.Image = Properties.Resources.SoundOff;
			}
			else
			{
				button_Sound.Image = Properties.Resources.SoundOn;
			}
			soundOn = !soundOn;
		}

		private void ButtonMessage(object sender, EventArgs e)
		{
			BlockUI(true);
			panel_Message.Visible = false;
		}

		private void ButtonChangeKeyOK(object sender, EventArgs e)
		{
			keysController.ChangeKeyBinding(lastPressedKeyCode, lastPressedKeyValue, lastPressedSpecialKey, true);
			if(keysController.MinusKeyIsPressed)
			{
				button_RemoteKeyMinus.Text = richTextBox_ChangeKeyPressed.Text;
				richTextBoxValues[selectedBind] = richTextBox_ChangeKeyPressed.Rtf; // Save rich text value for minus key.
			}
			else
			{
				button_RemoteKeyPlus.Text = richTextBox_ChangeKeyPressed.Text;
				richTextBoxValues[selectedBind+1] = richTextBox_ChangeKeyPressed.Rtf; // Save rich text value for plus key.
			}

			BlockUI(true);
			panel_ChangeKey.Visible = false;
		}

		private void ButtonChangeKeyCancel(object sender, EventArgs e)
		{
			BlockUI(true);
			panel_ChangeKey.Visible = false;
		}
		#endregion Buttons.

		public void PlaySound(bool playConnectedSound)
		{
			if(soundOn)
			{
				if(playConnectedSound)
				{
					soundPlayer.Stream = Properties.Resources.Connected;
				}
				else
				{
					soundPlayer.Stream = Properties.Resources.Disconnected;
				}
				soundPlayer.Play();
			}
		}
	}
}
