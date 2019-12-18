package pl.tarasienko.remoterewinder;


import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.hardware.display.DisplayManager;
import android.media.AudioManager;
import android.os.Build;
import android.os.PowerManager;
import android.view.Display;

public class VolumeButtonsReceiver extends BroadcastReceiver
{
	private static int maxVolume = 0;
	private static float volume = 0; // if newVolume == 0 then volume = 0.5f.


	@Override
	public void onReceive(Context context, Intent intent)
	{
		if(maxVolume == 0)
		{
			AudioManager audioManager = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);
			volume = intent.getIntExtra("android.media.EXTRA_PREV_VOLUME_STREAM_VALUE", 0);
			maxVolume = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
		}

		int newVolume = intent.getIntExtra("android.media.EXTRA_VOLUME_STREAM_VALUE", 0);
		if(volume != newVolume) // This receiver is invoking 2 times when user clicks the button only once when screen is on.
		{ 						// When screen is off then receiver is invoking only 1 time.
			if(volume > newVolume)
			{
				CommunicationService.sendDataToPC(true);
			}
			else
			{
				CommunicationService.sendDataToPC(false);
			}

			volume = newVolume;
			if(!isScreenOn(context)) // When screen is off then change min/max values immediately.
			{
				if(volume == 0 && newVolume == 0)
				{
					volume = 0.5f;
				}
				else if(volume == maxVolume && newVolume == maxVolume)
				{
					volume = maxVolume-0.5f;
				}
			}
		}
		else if(volume == 0 && newVolume == 0) // Can invoked only when screen is on.
		{
			volume = 0.5f;
		}
		else if(volume == maxVolume && newVolume == maxVolume) // Can invoked only when screen is on.
		{
			volume = maxVolume-0.5f;
		}
	}

	private boolean isScreenOn(Context context)
	{
		if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
		{
			DisplayManager dm = (DisplayManager)context.getSystemService(Context.DISPLAY_SERVICE);
			for(Display display : dm.getDisplays())
			{
				if(display.getState() != Display.STATE_OFF)
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			PowerManager powerManager = (PowerManager)context.getSystemService(Context.POWER_SERVICE);
			if(powerManager.isScreenOn())
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
