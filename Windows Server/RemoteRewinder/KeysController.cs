using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteRewinder
{
	public class KeysController
	{
		private bool minusKeyIsPressed;
		private string[] minusKeys = new string[3], plusKeys = new string[3];
		private RemoteRewinderForm form;

		public bool MinusKeyIsPressed { get { return minusKeyIsPressed; } set { minusKeyIsPressed = value; } }


		public KeysController(RemoteRewinderForm form, string[] minusKeys, string[] plusKeys)
		{
			this.form = form;
			if(minusKeys != null && plusKeys != null && minusKeys.Length == 3 && plusKeys.Length == 3) // Get loaded binds.
			{
				this.minusKeys[0] = minusKeys[0];
				this.plusKeys[0] = plusKeys[0];

				this.minusKeys[1] = minusKeys[1];
				this.plusKeys[1] = plusKeys[1];

				this.minusKeys[2] = minusKeys[2];
				this.plusKeys[2] = plusKeys[2];
			}
			else // Default binds.
			{
				this.minusKeys[0] = "{LEFT}";
				this.plusKeys[0] = "{RIGHT}";

				this.minusKeys[1] = "%{LEFT}";
				this.plusKeys[1] = "%{RIGHT}";

				this.minusKeys[2] = "{LEFT}";
				this.plusKeys[2] = " ";
			}
		}

		public void ChangeKeyBinding(string keyCode, int keyValue, string specialKey, bool keyCodeIsTranslated)
		{
			if(keyCodeIsTranslated)
			{
				keyCode = specialKey + keyCode;
			}
			else
			{
				keyCode = specialKey + TranslatePressedKey(keyCode, keyValue);
			}
			
			if(keyCode.EndsWith("Key is not supported!"))
			{
				// show message "Key is not supported!"
			}
			else
			{
				if(minusKeyIsPressed)
				{
					minusKeys[form.SelectedBind] = keyCode;
				}
				else
				{
					plusKeys[form.SelectedBind] = keyCode;
				}
			}
		}

		public void PressKey(bool invokeMinusKey)
		{
			try
			{
				if(invokeMinusKey)
				{
					SendKeys.SendWait(minusKeys[form.SelectedBind]);
				}
				else
				{
					SendKeys.SendWait(plusKeys[form.SelectedBind]);
				}
			}
			catch
			{
				Console.WriteLine("KeysController.PressKey() - SendKeys ERROR!");
			}
		}

		public string TranslatePressedKey(string keyCode, int keyValue) // Translate pressed KeyValue for SendKeys class.
		{
			if(keyValue >= 37 && keyValue <= 40) // Arrows.
			{
				keyCode = "{"+keyCode.ToUpper()+"}";
			}
			else if(keyValue >= 65 && keyValue <= 90) // Characters.
			{
				// Don't do anything with keyCode value.
			}
			else if(keyValue >= 48 && keyValue <= 57) // Numbers.
			{
				keyCode = keyCode[1].ToString();
			}
			else if(keyValue >= 96 && keyValue <= 105) // Numbers on NumPad.
			{
				keyCode = keyCode[6].ToString();
			}
			else if(keyValue >= 112 && keyValue <= 123) // F1 - F12 keys.
			{
				keyCode = "{"+keyCode+"}";
			}
			else if(keyValue == 188)
			{
				keyCode = ",";
			}
			else if(keyValue == 190)
			{
				keyCode = ".";
			}
			else if(keyValue == 191)
			{
				keyCode = "/";
			}
			else if(keyValue == 186)
			{
				keyCode = ";";
			}
			else if(keyValue == 222)
			{
				keyCode = "'";
			}
			else if(keyValue == 219)
			{
				keyCode = "[";
			}
			else if(keyValue == 221)
			{
				keyCode = "]";
			}
			else if(keyValue == 189)
			{
				keyCode = "-";
			}
			else if(keyValue == 187)
			{
				keyCode = "=";
			}
			else if(keyValue == 220)
			{
				keyCode = "\\";
			}
			else if(keyValue == 192)
			{
				keyCode = "`";
			}
			else if(keyValue == 110)
			{
				keyCode = ",";
			}
			else if(keyValue == 27)
			{
				keyCode = "{ESC}";
			}
			else if(keyValue == 13)
			{
				keyCode = "{ENTER}";
			}
			else if(keyValue == 32)
			{
				keyCode = " ";
			}
			else if(keyValue == 9)
			{
				keyCode = "{TAB}";
			}
			else if(keyValue == 20)
			{
				keyCode = "{CAPSLOCK}";
			}
			else if(keyValue == 8)
			{
				keyCode = "{BACKSPACE}";
			}
			else if(keyValue == 107)
			{
				keyCode = "{ADD}";
			}
			else if(keyValue == 109)
			{
				keyCode = "{SUBTRACT}";
			}
			else if(keyValue == 106)
			{
				keyCode = "{MULTIPLY}";
			}
			else if(keyValue == 111)
			{
				keyCode = "{DIVIDE}";
			}
			else if(keyValue == 36)
			{
				keyCode = "{HOME}";
			}
			else if(keyValue == 35)
			{
				keyCode = "{END}";
			}
			else if(keyValue == 45)
			{
				keyCode = "{INSERT}";
			}
			else if(keyValue == 33)
			{
				keyCode = "{PGUP}";
			}
			else if(keyValue == 34)
			{
				keyCode = "{PGDN}";
			}
			else if(keyValue == 46)
			{
				keyCode = "{DELETE}";
			}
			else if(keyValue == 144)
			{
				keyCode = "{NUMLOCK}";
			}
			else if(keyValue == 145)
			{
				keyCode = "{SCROLLLOCK}";
			}
			else
			{
				keyCode = "Key is not supported!";
			}

			return keyCode;
		}
	}
}
