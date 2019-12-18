package pl.tarasienko.remoterewinder;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;


public class NotificationButtonReceiver extends BroadcastReceiver
{
	@Override
	public void onReceive(Context context, Intent intent)
	{
		Log.d("onReceive", "NotificationButtonReceiver, CONTEXT: "+context);

		MainActivity mainActivity = MainActivity.getInstance();
		if(mainActivity != null)
		{
			mainActivity.finish();
		}

		Intent serviceIntent = new Intent(context, CommunicationService.class);
		context.stopService(serviceIntent);
	}
}
