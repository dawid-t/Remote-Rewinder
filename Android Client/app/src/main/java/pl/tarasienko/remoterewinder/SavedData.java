package pl.tarasienko.remoterewinder;


import android.content.Context;
import android.util.Log;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;

public class SavedData
{
	public static enum dataType { WORK_IN_BACKGROUND, SERVERS_LIST, SELECTED_SERVER, LAST_CONNECTED_SERVER, PORT, LANGUAGE }


	public SavedData()
	{ }

	public static Object loadData(Context context, dataType type)
	{
		try
		{
			String path = "/data";
			File file = new File(context.getFilesDir().getPath()+path); // Path to directory.
			if(file.isDirectory()) // If directory exists then try load file.
			{
				path = getPath(path, type); // Get path to the file with chosen type.
				file = new File(context.getFilesDir().getPath()+path); // Path to file.
				if(file.isFile()) // If file exists then load it.
				{
					ObjectInputStream ois = new ObjectInputStream(new FileInputStream(file));
					Object object = ois.readObject();
					ois.close();
					return object; // Return loaded file.
				}
			}
			return null; // If directory or file doesn't exists then return null.
		}
		catch(Exception e)
		{
			Log.d("SavedData", "Unable to load data - "+e.getMessage());
			return null;
		}
	}

	public static void saveData(Context context, Object object, dataType type)
	{
		try
		{
			String path = "/data";
			File file = new File(context.getFilesDir().getPath()+path); // Path to directory.
			file.mkdir(); // Create directory if not exists.

			path = getPath(path, type); // Get path to the file with chosen type.
			file = new File(context.getFilesDir().getPath()+path); // Path to file.

			ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream(file));
			oos.writeObject(object);
			oos.flush();
			oos.close();
		}
		catch(Exception e)
		{
			Log.d("SavedData", "Unable to save data - "+e.getMessage());
		}
	}

	private static String getPath(String path, dataType type)
	{
		switch(type)
		{
			case WORK_IN_BACKGROUND:
				path += "/workInBackground.dat";
				break;
			case SERVERS_LIST:
				path += "/serversList.dat";
				break;
			case SELECTED_SERVER:
				path += "/selectedServer.dat";
				break;
			case LAST_CONNECTED_SERVER:
				path += "/connectedServer.dat";
				break;
			case PORT:
				path += "/port.dat";
				break;
			case LANGUAGE:
				path += "/language.dat";
				break;
		}
		return path;
	}
}
