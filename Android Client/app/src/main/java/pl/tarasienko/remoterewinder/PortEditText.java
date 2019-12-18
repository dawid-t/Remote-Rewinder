package pl.tarasienko.remoterewinder;

import android.content.Context;
import android.util.AttributeSet;
import android.view.KeyEvent;
import android.widget.EditText;

public class PortEditText extends EditText
{
	public PortEditText(Context context, AttributeSet attrs)
	{
		super(context, attrs);
	}

	@Override
	public boolean onKeyPreIme(int keyCode, KeyEvent event)
	{
		if(keyCode == event.KEYCODE_BACK)
		{
			clearFocus();
		}
		return false;
	}
}
