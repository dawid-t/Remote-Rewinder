package pl.tarasienko.remoterewinder;

import android.app.Notification;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.os.Binder;
import android.os.IBinder;

import android.support.v4.app.NotificationCompat;
import android.util.Log;
import android.view.Gravity;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetSocketAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;


public class CommunicationService extends Service
{
	private static Socket socket;
	private static Timer timer;
	
	private boolean pingReceived = true;
	private int udpPort, volume;
	private ArrayList<Map<String, String>> serversData = new ArrayList();
	private DatagramSocket udpSocket;
	private Toast toast;
	private MediaPlayer mediaPlayer;
	private VolumeButtonsReceiver volumeButtonsReceiver;
	private NotificationButtonReceiver notificationButtonReceiver;
	private IBinder binder;


	public class ServiceBinder extends Binder
	{
		public CommunicationService getService()
		{
			return CommunicationService.this;
		}
	}

	@Override
	public IBinder onBind(Intent arg0)
	{
		return binder;
	}

	@Override
	public void onCreate()
	{
		super.onCreate();

		init();
		createNotification();
		//saveVolumeLevel();
		//toast.setText("Service is started.");
		//toast.show();
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId)
	{
		int lastConnectedServerId = -1;
		if(intent != null) // When service is restarting then activity can doesn't exist anymore.
		{
			lastConnectedServerId = intent.getIntExtra("lastConnectedServerId", lastConnectedServerId);
			if(lastConnectedServerId != -1 && serversData.size() > lastConnectedServerId) // If selected server is on the list then put it on the first place.
			{
				Map<String, String> serverData = serversData.get(lastConnectedServerId);
				serversData.remove(lastConnectedServerId);
				serversData.add(0, serverData);
			}
		}

		//toast.setText("Service is still running.");
		//toast.show();
		return super.onStartCommand(intent, flags, startId);
	}

	@Override
	public void onDestroy()
	{
		closeConnectionWithPC();
		unregisterVolumeButtonsReceiver(); // Sometimes onDisconnect() is invoking too late and then IntentReceiver is leaked.
		unregisterNotificationButtonReceiver();
		SavedData.saveData(this, serversData, SavedData.dataType.SERVERS_LIST);

		//toast.setText("Service is stopped.");
		//toast.show();
		super.onDestroy();
	}

	private void init()
	{
		binder = new ServiceBinder();
		toast = Toast.makeText(this, "", Toast.LENGTH_SHORT);

		// region Load saved servers list:
		try
		{
			Object object = SavedData.loadData(this, SavedData.dataType.SERVERS_LIST);
			if(object != null)
			{
				serversData = (ArrayList<Map<String, String>>)object;
			}
		}
		catch(Exception e)
		{
			Log.d("CommunicationService", "Unable to load data - "+e.getMessage());
		}
		// endregion Load saved servers list.
	}

	public Socket getSocket()
	{
		return socket;
	}

	public void setUdpPort(int port)
	{
		udpPort = port;
	}

	public ArrayList<Map<String, String>> getServersData()
	{
		return serversData;
	}

	public void setServersData(ArrayList<Map<String, String>> serversData)
	{
		this.serversData = serversData;
	}

	public void clearServersDataList()
	{
		serversData.clear();
	}

	private void createNotification() // Later create channel for Android 8.0 and higher.
	{
		// MainActivity intent:
		Intent mainIntent = new Intent(this, MainActivity.class);
		mainIntent.setFlags(Intent.FLAG_ACTIVITY_BROUGHT_TO_FRONT);
		PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, mainIntent, PendingIntent.FLAG_UPDATE_CURRENT);

		// Broadcast intent for "EXIT" button:
		registerNotificationButtonReceiver();
		Intent exitIntent = new Intent("remoteRewinder.notificationButtonReceiver");
		PendingIntent exitPendingIntent = PendingIntent.getBroadcast(this, 0, exitIntent, 0);

		// Creating and starting notification:
		Notification notification = new NotificationCompat.Builder(this)
			.setSmallIcon(R.mipmap.ic_launcher)
			.setContentTitle("Remote Rewinder")
			.setContentText("Disconnected")
			.setPriority(NotificationCompat.PRIORITY_MAX)
			.setContentIntent(pendingIntent)
			.addAction(R.mipmap.ic_launcher, "Exit", exitPendingIntent)
			.build();
		startForeground(1, notification);
	}

	private void startMediaPlayer()
	{
		mediaPlayer = MediaPlayer.create(this, R.raw.dull_sound); // Need this when screen will be off for VolumeButtonsReceiver.
		mediaPlayer.setLooping(true);
		mediaPlayer.start();
	}

	private void stopMediaPlayer()
	{
		if(mediaPlayer != null)
		{
			mediaPlayer.stop();
			mediaPlayer.release();
			mediaPlayer = null;
		}
	}

	// region Broadcast Receivers:
	private void registerVolumeButtonsReceiver()
	{
		IntentFilter filter = new IntentFilter();
		filter.addAction("android.media.VOLUME_CHANGED_ACTION");
		volumeButtonsReceiver = new VolumeButtonsReceiver();
		registerReceiver(volumeButtonsReceiver, filter);
	}

	private void unregisterVolumeButtonsReceiver()
	{
		if(volumeButtonsReceiver != null)
		{
			try // This method is invoking in same time from 2 different threads so it must be in try/catch blocks.
			{
				unregisterReceiver(volumeButtonsReceiver);
				volumeButtonsReceiver = null;
			}
			catch(Exception e)
			{
				Log.d("CommunicationService", "Unregister volume buttons receiver exception - "+e.getMessage());
			}
		}
	}

	private void registerNotificationButtonReceiver()
	{
		IntentFilter filter = new IntentFilter();
		filter.addAction("remoteRewinder.notificationButtonReceiver");
		notificationButtonReceiver = new NotificationButtonReceiver();
		registerReceiver(notificationButtonReceiver, filter);
	}

	private void unregisterNotificationButtonReceiver()
	{
		if(notificationButtonReceiver != null)
		{
			unregisterReceiver(notificationButtonReceiver);
			notificationButtonReceiver = null;
		}
	}
	// endregion Broadcast Receivers.

	private void saveVolumeLevel() // Save the volume level when connected to the server.
	{
		AudioManager audioManager = (AudioManager)getSystemService(Context.AUDIO_SERVICE);
		volume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
	}

	private void loadVolumeLevel() // When disconnected form server then load the old volume level.
	{
		AudioManager audioManager = (AudioManager)getSystemService(Context.AUDIO_SERVICE);
		audioManager.setStreamVolume(AudioManager.STREAM_MUSIC, volume, 0);
	}

	// region TCP sockets:
	public void connectToPC(final int selectedServerId)
	{
		new Thread(new Runnable()
		{
			@Override
			public void run()
			{
				try
				{
					String[] serverAddress = serversData.get(selectedServerId).get("IP").split(":");
					String serverIP = serverAddress[0];
					int serverPort = Integer.parseInt(serverAddress[1]);

					socket = new Socket();
					socket.connect(new InetSocketAddress(serverIP, serverPort), 2000);
					receiveData(); // Waiting for ping from the server. When server (or we) will disconnected then this method will invoke closeConnectionWithPC().
					pingServer(); // Send ping to the server for every 5s.

					startMediaPlayer();
					registerVolumeButtonsReceiver();
					saveVolumeLevel();

					final MainActivity mainActivity = MainActivity.getInstance();
					mainActivity.runOnUiThread(new Runnable()
					{
						@Override
						public void run()
						{
							mainActivity.changeToOnlineStatus();
						}
					});
				}
				catch(Exception e)
				{
					if(socket != null)
					{
						try
						{
							socket.close();
						}
						catch(Exception e2)
						{
							Log.d("CommunicationService", "Connect to PC exception (can't close socket) - "+e2.getMessage());
						}
					}
					socket = null;
					final MainActivity mainActivity = MainActivity.getInstance();
					mainActivity.runOnUiThread(new Runnable()
					{
						@Override
						public void run()
						{
							mainActivity.changeToOfflineStatus();
						}
					});
					Log.d("CommunicationService", "Connect to PC exception - "+e.getMessage());
				}
			}
		}).start();
	}

	public static void sendDataToPC(final boolean volumeButtonMinus)
	{
		if(socket == null)
		{
			return;
		}

		new Thread(new Runnable()
		{
			@Override
			public void run()
			{
				try
				{
					DataOutputStream out = new DataOutputStream(socket.getOutputStream());
					if(volumeButtonMinus)
					{
						out.writeBytes("minus");
					}
					else
					{
						out.writeBytes("plus");
					}
					//out.flush();
				}
				catch(Exception e)
				{
					Log.d("CommunicationService", "Send data to PC exception - "+e.getMessage());
				}
			}
		}).start();
	}
	
	private void pingServer() // This is necessary if we want to detect a loss of server connection.
	{
		pingReceived = true;
		timer = new Timer();
		TimerTask timerTask = new TimerTask()
		{
			@Override
			public void run()
			{
				try
				{
					if(pingReceived)
					{
						DataOutputStream out = new DataOutputStream(socket.getOutputStream());
						out.writeBytes("ping");
						//out.flush();
						pingReceived = false;
						Log.d("CommunicationService", "PINGGGG!!!");
					}
					else
					{
						Log.d("CommunicationService", "PINGGGG not received!!!");
						closeConnectionWithPC();
					}
				}
				catch(Exception e)
				{
					Log.d("CommunicationService", "Ping server exception - "+e.getMessage());
				}
			}
		};
		timer.scheduleAtFixedRate(timerTask, 0,5000);
	}

	private void receiveData() // For receiving ping from the server (detect a loss of server connection) and detect server/client disconnection.
	{
		if(socket == null)
		{
			return;
		}

		new Thread(new Runnable()
		{
			@Override
			public void run()
			{
				try
				{
					DataInputStream in = new DataInputStream(socket.getInputStream());
					byte[] buffer = new byte[64];
					int bytesCount = in.read(buffer); // If server's socket will disconnected (or we will do it ourselves) then this will throw the exception.
					if(bytesCount > 0)
					{
						buffer = Arrays.copyOf(buffer, bytesCount);
						String message = new String(buffer, "ASCII");
						Log.d("Msg from bytes", ""+message);
						if(message.equals("pong")) // If the server will callback to our ping then we know we have a connection with it.
						{
							pingReceived = true;
						}
					}
					Log.d("On DC bytesCount", ""+bytesCount);
					receiveData();
				}
				catch(Exception e)
				{
					closeConnectionWithPC();

					stopMediaPlayer();
					unregisterVolumeButtonsReceiver();
					loadVolumeLevel();

					final MainActivity mainActivity = MainActivity.getInstance();
					if(mainActivity != null)
					{
						mainActivity.runOnUiThread(new Runnable()
						{
							@Override
							public void run()
							{
								mainActivity.changeToOfflineStatus();
							}
						});
					}
					Log.d("CommunicationService", "On disconnect exception - "+e.getMessage());
				}
			}
		}).start();
	}

	public void closeConnectionWithPC()
	{
		if(socket != null)
		{
			try
			{
				if(timer != null)
				{
					timer.cancel();
					timer.purge();
					timer = null;
				}
				socket.close();
			}
			catch(Exception e)
			{
				Log.d("CommunicationService", "Close connection with PC exception - "+e.getMessage());
			}
			finally
			{
				socket = null;
			}
		}
	}
	// endregion TCP sockets.

	// region UDP sockets:
	public void startReceivingServersData()
	{
		new Thread(new Runnable()
		{
			@Override
			public void run()
			{
				try
				{
					udpSocket = new DatagramSocket(udpPort);
					int counter = 0;
					while(true)
					{
						byte[] receiveBuffer = new byte[64];
						final DatagramPacket datagramPacket = new DatagramPacket(receiveBuffer, 64);
						udpSocket.receive(datagramPacket);

						new Thread(new Runnable()
						{
							@Override
							public void run()
							{
								String[] receivedData = new String(datagramPacket.getData()).trim().split("-");
								if(receivedData.length < 2) // If received data is not correct then don't save that server to the list.
								{
									return;
								}

								String serverName = "";
								for(int i = 0; i < receivedData.length-1; i++) // Server name can contains dashes that shared the received data.
								{
									serverName += receivedData[i]+"-";
								}
								serverName = serverName.substring(0, serverName.length()-1); // Delete last dash.
								String serverPort = receivedData[receivedData.length-1];
								String serverIp = datagramPacket.getAddress().getHostAddress();
								String serverIpAndPort = serverIp+":"+serverPort;
								for(int i = 0; i < serversData.size(); i++) // Searching server in list with the same IP.
								{
									if(serversData.get(i).get("IP").equals(serverIpAndPort)) // If server with this IP exist in the list then don't add it again.
									{
										return;
									}
								}

								Map<String, String> map = new HashMap(2);
								map.put("Server Name", serverName);
								map.put("IP", serverIpAndPort);
								serversData.add(map);
							}
						}).start();
						Log.d("Counter", ""+counter);
						counter++;
					}
				}
				catch(Exception e)
				{
					Log.d("CommunicationService", "Receiving servers data error - "+e.getMessage());
					for(int i = 0; i < serversData.size(); i++)
					{
						Log.d("Servers data", serversData.get(i).get("Server Name"));
					}
				}
			}
		}).start();
	}

	public void stopReceivingServersData()
	{
		if(udpSocket != null)
		{
			udpSocket.close();
		}
	}
	// endregion UDP sockets.

	// region Ports:
	public boolean checkUDPPort(EditText editText_Port)
	{
		if(udpPort < 1024) // Under port 1024 are system ports which we don't want to use. Change port to minimum available value when is too low.
		{
			editText_Port.setText(1024+"");
			editText_Port.setSelection(4); // Set the cursor position at the end.
			udpPort = 1024;
		}
		else if(udpPort > 65535) // Max port value is 65535. Change to this value when is too high.
		{
			editText_Port.setText(65535+"");
			editText_Port.setSelection(5); // Set the cursor position at the end.
			udpPort = 65535;
		}
		
		return IsUdpPortAvailable();
	}

	public boolean IsTcpPortAvailable(int selectedServerId)
	{
		ServerSocket socket = null;
		try
		{
			String[] serverAddress = serversData.get(selectedServerId).get("IP").split(":");
			int serverPort = Integer.parseInt(serverAddress[1]);
			socket = new ServerSocket();
			socket.bind(new InetSocketAddress(serverPort)); // Check free port by creating temporary TCP socket.
			return true;
		}
		catch(Exception e)
		{
			showPortUnavailabilityMsg(true);
			return false;
		}
		finally
		{
			if(socket != null)
			{
				try
				{
					socket.close();
				}
				catch(Exception e)
				{ }
			}
		}
	}

	private boolean IsUdpPortAvailable()
	{
		DatagramSocket socket = null;
		try
		{
			socket = new DatagramSocket(udpPort); // Check free port by creating temporary UDP socket.
			return true;
		}
		catch(Exception e)
		{
			showPortUnavailabilityMsg(false);
			return false;
		}
		finally
		{
			if(socket != null)
			{
				socket.close();
			}
		}
	}

	private void showPortUnavailabilityMsg(boolean tcpPort)
	{
		if(tcpPort)
		{
			toast.setText(R.string.tcp_port_unavailable);
			toast.setDuration(Toast.LENGTH_LONG);
		}
		else
		{
			toast.setText(R.string.udp_port_unavailable);
		}

		TextView textView_ToastMessage = (TextView)toast.getView().findViewById(android.R.id.message);
		if(textView_ToastMessage != null)
		{
			textView_ToastMessage.setGravity(Gravity.CENTER);
		}
		toast.show();
		toast.setDuration(Toast.LENGTH_SHORT);
	}
	// endregion Ports.
}
