package pl.tarasienko.remoterewinder;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.graphics.Color;
import android.os.Build;
import android.os.IBinder;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.ListView;
import android.widget.RelativeLayout;
import android.widget.SimpleAdapter;
import android.widget.Switch;
import android.widget.TextView;

import java.util.Locale;
import java.util.Timer;
import java.util.TimerTask;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class MainActivity extends AppCompatActivity
{
	private static MainActivity instance;

	private boolean serviceIsBinded = false, workInBackground = false;
	private int selectedServerId = -1, connectedServerId = -1, lastConnectedServerId = -1;
	private final String grayColor = "#4f4f4f", blueColor = "#6cb6ff", greenColor = "#bfff00", yellowColor = "#ffff00";
	private String lastCorrectPort;
	private ConnectedStatus connectedStatus = ConnectedStatus.disconnected;
	private SimpleAdapter adapter;
	private CommunicationService service;
	private ServiceConnection serviceConnection;
	private RelativeLayout relativeLayout_MainActivity, relativeLayout_ConnectedStatus;
	private FrameLayout frameLayout_ServersList;
	private TextView textView_ConnectedStatus, textView_ConnectedWith, textView_ConnectedServerName;
	private EditText editText_Port;
	private Button button_Search, button_Connect;
	private Switch switch_WorkInBackground;
	private ListView listView_ServersList;
	private View selectedServerView, connectedServerView;
	private Menu menu;
	
	private enum ConnectedStatus { disconnected , connecting, connected }


	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		init();
		bindService();
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu)
	{
		getMenuInflater().inflate(R.menu.main, menu);
		this.menu = menu;
		
		// region Load saved language after creating the menu:
		Object object = SavedData.loadData(this, SavedData.dataType.LANGUAGE);
		if(object != null)
		{
			String language = Locale.getDefault().getLanguage();
			if(!language.equals(object.toString()))
			{
				changeLanguage();
			}
		}
		// endregion Load saved language after creating the menu.
		
		return super.onCreateOptionsMenu(menu);
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		int id = item.getItemId();
		
		switch(id)
		{
			case R.id.change_language:
				changeLanguage();
				break;
			
			default:
				return true;
		}
		return super.onOptionsItemSelected(item);
	}

	@Override
	protected void onDestroy()
	{
		super.onDestroy();

		// region Save data:
		SavedData.saveData(this, workInBackground, SavedData.dataType.WORK_IN_BACKGROUND);
		SavedData.saveData(this, lastConnectedServerId, SavedData.dataType.LAST_CONNECTED_SERVER);
		SavedData.saveData(this, editText_Port.getText().toString(), SavedData.dataType.PORT);
		SavedData.saveData(this, Locale.getDefault().getLanguage(), SavedData.dataType.LANGUAGE);
		// endregion Save data.

		if(connectedStatus == ConnectedStatus.connecting && service != null) // If we try connect to the server when app is closing then close the connection.
		{																	 // This is necessary when service is working in background.
			service.closeConnectionWithPC();
		}
		unbindService();
	}

	private void init()
	{
		// region Initialize variables:
		instance = this;

		relativeLayout_MainActivity = (RelativeLayout)findViewById(R.id.relativeLayout_MainActivity);
		relativeLayout_ConnectedStatus = (RelativeLayout)findViewById(R.id.relativeLayout_ConnectedStatus);
		frameLayout_ServersList = (FrameLayout)findViewById(R.id.frameLayout_ServersList);
		textView_ConnectedStatus = (TextView)findViewById(R.id.textView_ConnectedStatus);
		textView_ConnectedWith = (TextView)findViewById(R.id.textView_ConnectedWith);
		textView_ConnectedServerName = (TextView)findViewById(R.id.textView_ConnectedServerName);
		editText_Port = (EditText)findViewById(R.id.editText_Port);
		button_Search = (Button)findViewById(R.id.button_Search);
		button_Connect = (Button)findViewById(R.id.button_Connect);
		switch_WorkInBackground = (Switch)findViewById(R.id.switch_WorkInBackground);
		listView_ServersList = (ListView)findViewById(R.id.listView_ServersList);

		lastCorrectPort = editText_Port.getText()+"";
		// endregion Initialize variables.

		// region Create listeners:
		relativeLayout_MainActivity.setOnClickListener(new View.OnClickListener()
		{
			@Override
			public void onClick(View view)
			{
				removeFocus(editText_Port);
			}
		});

		editText_Port.setOnEditorActionListener(new TextView.OnEditorActionListener()
		{
			@Override
			public boolean onEditorAction(TextView textView, int actionId, KeyEvent keyEvent)
			{

				if(actionId == EditorInfo.IME_ACTION_DONE)
				{
					removeFocus(editText_Port);
				}
				return false;
			}
		});

		editText_Port.addTextChangedListener(new TextWatcher()
		{
			@Override
			public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2)
			{

			}

			@Override
			public void onTextChanged(CharSequence charSequence, int start, int before, int count)
			{
				Pattern pattern = Pattern.compile("^[0-9]{0,5}$");
				Matcher matcher = pattern.matcher(editText_Port.getText()+"");

				if(matcher.matches())
				{
					lastCorrectPort = editText_Port.getText()+"";
				}
				else
				{
					editText_Port.setText(lastCorrectPort);
					editText_Port.setSelection(start); // Set the old cursor position.
				}
			}

			@Override
			public void afterTextChanged(Editable editable)
			{

			}
		});
		// endregion Create listeners.

		// region Create servers listView listener:
		listView_ServersList.setOnItemClickListener(new AdapterView.OnItemClickListener()
		{
			@Override
			public void onItemClick(AdapterView<?> parent, View view, int position, long id)
			{
				if(connectedStatus != ConnectedStatus.connecting)
				{
					button_Connect.setEnabled(true);
				}

				if(selectedServerId != -1)
				{
					if(selectedServerId != connectedServerId)
					{
						selectedServerView.setBackgroundColor(Color.parseColor(grayColor)); // Old selected server.
					}
					else
					{
						selectedServerView.setBackgroundColor(Color.parseColor(greenColor)); // Old selected and connected server.
					}
				}

				selectedServerId = position;
				selectedServerView = view;
				if(selectedServerId != connectedServerId)
				{
					selectedServerView.setBackgroundColor(Color.parseColor(blueColor)); // New selected server.
				}
				else
				{
					selectedServerView.setBackgroundColor(Color.parseColor(yellowColor)); // New selected and connected server.
				}
			}
		});
		// endregion Create servers listView listener.

		// region Create ServiceConnection for binding the service:
		serviceConnection = new ServiceConnection()
		{
			@Override
			public void onServiceConnected(ComponentName componentName, IBinder iBinder)
			{
				CommunicationService.ServiceBinder binder = (CommunicationService.ServiceBinder)iBinder;
				service = binder.getService();

				// region Change servers list UI if servers data has been saved:
				if(lastConnectedServerId != -1) // If we were connected to the server last time then this server will be in the first place on the list.
				{
					selectedServerId = 0;
					if(service.getSocket() != null) // If service is working in background then we can be still connected.
					{
						connectedServerId = 0;
						changeToOnlineStatus();
					}
					lastConnectedServerId = 0;
					button_Connect.setEnabled(true);
				}
				if(service.getServersData().size() > 0)
				{
					frameLayout_ServersList.setBackground(getResources().getDrawable(R.drawable.list_view_border_not_empty));
				}
				// endregion Change servers list UI if servers data has been saved.

				adapter = new SimpleAdapter(MainActivity.this, service.getServersData(), R.layout.server_button,
					new String[] { "Server Name", "IP" }, new int[] { R.id.textView_ServerName, R.id.textView_ServerIP })
				{
					@Override
					public View getView(int position, View convertView, ViewGroup parent)
					{
						convertView = super.getView(position, convertView, parent);

						Log.d("selectedServerId", selectedServerId+", connectedServerId: "+connectedServerId);
						if(selectedServerId == position)
						{
							if(selectedServerId != connectedServerId)
							{
								convertView.setBackgroundColor(Color.parseColor(blueColor));
								selectedServerView = convertView;
							}
							else
							{
								convertView.setBackgroundColor(Color.parseColor(yellowColor));
								selectedServerView = convertView;
								connectedServerView = convertView;
							}
						}
						else if(connectedServerId == position)
						{
							convertView.setBackgroundColor(Color.parseColor(greenColor));
							connectedServerView = convertView;
						}
						else
						{
							convertView.setBackgroundColor(Color.parseColor(grayColor));
						}

						return convertView;
					}
				};
				listView_ServersList.setAdapter(adapter);
				button_Search.setEnabled(true);
				serviceIsBinded = true;
			}

			@Override
			public void onServiceDisconnected(ComponentName componentName)
			{
				service = null;
				button_Search.setEnabled(false);
				serviceIsBinded = false;
			}
		};
		// endregion Create ServiceConnection for binding the service.

		// region Load data:
		try
		{
			Object object = SavedData.loadData(this, SavedData.dataType.WORK_IN_BACKGROUND);
			if(object != null)
			{
				workInBackground = (boolean)object;
				switch_WorkInBackground.setChecked(workInBackground);
			}

			object = SavedData.loadData(this, SavedData.dataType.LAST_CONNECTED_SERVER);
			if(object != null)
			{
				lastConnectedServerId = (int)object;
			}

			object = SavedData.loadData(this, SavedData.dataType.PORT);
			if(object != null)
			{
				editText_Port.setText(object.toString());
				lastCorrectPort = editText_Port.getText()+"";
			}
		}
		catch(Exception e)
		{
			Log.d("MainActivity", "Unable to load data - "+e.getMessage());
		}
		// endregion Load data.

		// region Check system version:
		if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) // Android Oreo or higher doesn't support services in background.
		{
			workInBackground = false;
			switch_WorkInBackground.setChecked(false);
			switch_WorkInBackground.setVisibility(View.INVISIBLE);
		}
		// endregion Check system version.
	}

	public static MainActivity getInstance()
	{
		return instance;
	}

	// region UI:
	private void changeToConnectingStatus()
	{
		connectedStatus = ConnectedStatus.connecting;
		textView_ConnectedStatus.setText(R.string.connecting);
		textView_ConnectedStatus.setTextColor(Color.YELLOW);
		relativeLayout_ConnectedStatus.setBackground(getResources().getDrawable(R.drawable.status_connecting));

		button_Connect.setText(R.string.connecting);
		button_Connect.setEnabled(false);
	}

	public void changeToOnlineStatus()
	{
		connectedStatus = ConnectedStatus.connected;
		textView_ConnectedStatus.setText(R.string.connected);
		textView_ConnectedStatus.setTextColor(Color.GREEN);
		textView_ConnectedWith.setVisibility(View.VISIBLE);
		textView_ConnectedServerName.setText(service.getServersData().get(selectedServerId).get("Server Name"));
		textView_ConnectedServerName.setVisibility(View.VISIBLE);
		relativeLayout_ConnectedStatus.setBackground(getResources().getDrawable(R.drawable.status_online));

		button_Connect.setText(R.string.disconnect);
		button_Connect.setEnabled(true);

		if(selectedServerView != null) // If service is working in background and we are connected when activity is recreating then it will be a null.
		{							   // This is need only when we are connecting to the server.
			connectedServerId = selectedServerId;
			lastConnectedServerId = selectedServerId;
			connectedServerView = selectedServerView;
			selectedServerView.setBackgroundColor(Color.parseColor(yellowColor));
		}
	}

	public void changeToOfflineStatus()
	{
		connectedStatus = ConnectedStatus.disconnected;
		textView_ConnectedStatus.setText(R.string.disconnected);
		textView_ConnectedStatus.setTextColor(Color.RED);
		textView_ConnectedWith.setVisibility(View.INVISIBLE);
		textView_ConnectedServerName.setText("-");
		textView_ConnectedServerName.setVisibility(View.INVISIBLE);
		relativeLayout_ConnectedStatus.setBackground(getResources().getDrawable(R.drawable.status_offline));

		button_Connect.setText(R.string.connect_to_pc);
		if(selectedServerId == -1) // Before disconnected client can refresh servers list and then it can be empty.
		{
			button_Connect.setEnabled(false);
		}
		else
		{
			button_Connect.setEnabled(true);
		}

		if(connectedServerView != null) // This is null when we try connect to the server and connecting is timeout.
		{
			if(selectedServerId != connectedServerId)
			{
				connectedServerView.setBackgroundColor(Color.parseColor(grayColor));
			}
			else
			{
				connectedServerView.setBackgroundColor(Color.parseColor(blueColor));
			}
			connectedServerId = -1;
			connectedServerView = null;
		}
	}
	
	private void changeLanguage()
	{
		// region Change language:
		String language = Locale.getDefault().getLanguage(); // Language used now.
		language = language.equals("en") ? "pl" : "en"; // New language.
		
		Locale locale = new Locale(language);
		Locale.setDefault(locale);
		
		Resources resources = getResources();
		Configuration configuration = resources.getConfiguration();
		configuration.locale = locale;
		resources.updateConfiguration(configuration, resources.getDisplayMetrics());
		// endregion Change language.
		
		// region "Refresh" UI:
		menu.findItem(R.id.change_language).setTitle(R.string.change_language);
		
		switch(connectedStatus)
		{
			case disconnected:
				textView_ConnectedStatus.setText(R.string.disconnected);
				break;
			case connecting:
				textView_ConnectedStatus.setText(R.string.connecting);
				break;
			case connected:
				textView_ConnectedStatus.setText(R.string.connected);
				break;
		}
		
		textView_ConnectedWith.setText(R.string.with);
		button_Search.setText(R.string.search_pc);
		button_Connect.setText(R.string.connect_to_pc);
		switch_WorkInBackground.setText(R.string.work_in_background);
		// endregion "Refresh" UI.
	}

	public void removeFocus(View view) // Hide keyboard and clear view focus.
	{
		InputMethodManager imm = (InputMethodManager) getSystemService(Activity.INPUT_METHOD_SERVICE);
		imm.hideSoftInputFromWindow(view.getWindowToken(), InputMethodManager.HIDE_NOT_ALWAYS);
		view.clearFocus();
	}
	// endregion UI.

	// region Service:
	private void bindService()
	{
		Intent intent = new Intent(this, CommunicationService.class);
		intent.putExtra("lastConnectedServerId", lastConnectedServerId);
		startService(intent);
		bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE);
	}

	private void unbindService()
	{
		if(serviceIsBinded)
		{
			unbindService(serviceConnection);
		}

		if(!workInBackground)
		{
			Intent intent = new Intent(this, CommunicationService.class);
			stopService(intent);
		}
	}
	// endregion Service.

	// region Buttons:
	public void buttonSearch(View v)
	{
		if(service != null)
		{
			service.setUdpPort(Integer.parseInt(editText_Port.getText().toString()));
			if(!service.checkUDPPort(editText_Port)) // If UDP port isn't available then don't search the servers.
			{
				return;
			}

			selectedServerId = -1;
			connectedServerId = -1;
			lastConnectedServerId = -1;
			button_Search.setEnabled(false);
			if(service.getSocket() == null)
			{
				button_Connect.setEnabled(false);
			}
			//service.setUdpPort(Integer.parseInt(editText_Port.getText().toString()));
			service.clearServersDataList();
			adapter.notifyDataSetChanged(); // Refresh listView after clear the servers data list.
			service.startReceivingServersData(); // Search servers in LAN.
			frameLayout_ServersList.setBackground(getResources().getDrawable(R.drawable.list_view_border_empty));
			Timer timer = new Timer();
			TimerTask timerTask = new TimerTask()
			{
				@Override
				public void run()
				{
					service.stopReceivingServersData();

					MainActivity.this.runOnUiThread(new Runnable()
					{
						@Override
						public void run()
						{
							adapter.notifyDataSetChanged(); // Refresh listView after searching.
							button_Search.setEnabled(true);
							if(listView_ServersList.getCount() > 0)
							{
								frameLayout_ServersList.setBackground(getResources().getDrawable(R.drawable.list_view_border_not_empty));
							}
						}
					});
				}
			};
			timer.schedule(timerTask, 2000);
		}
	}

	public void buttonConnect(View v)
	{
		if(service != null)
		{
			if(service.getSocket() == null)
			{
				if(!service.IsTcpPortAvailable(selectedServerId))
				{
					return;
				}

				changeToConnectingStatus();
				service.connectToPC(selectedServerId);
			}
			else
			{
				service.closeConnectionWithPC();
			}
		}
	}

	public void buttonWorkInBackground(View v)
	{
		workInBackground = !workInBackground;
	}
	// endregion Buttons.
}
