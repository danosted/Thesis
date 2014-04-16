using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class Serializer
{
	private string filepath = Application.persistentDataPath + "/" + "hitmap.dat";

	private static Serializer instance;
	
	public static Serializer Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new Serializer();
			}
			return instance;
		}
	}

	public void Serialize(List<HitmapEvent> o)
	{
		FileStream fs = new FileStream(filepath, FileMode.Create);
		XmlSerializer formatter = new XmlSerializer(typeof(List<HitmapEvent>));
		try
		{
			formatter.Serialize(fs, o);
		}
		catch(XmlException e)
		{
			Debug.Log("Failed to serialize. Reason: " + e.Message);
			throw;
		}
		finally
		{
			fs.Close();
		}
	}

	public List<HitmapEvent> Deserialize()
	{
		FileStream fs = new FileStream(filepath, FileMode.Open);
		List<HitmapEvent> o = null;
		try
		{
			XmlSerializer formatter = new XmlSerializer(typeof(List<HitmapEvent>));
			
			// Deserialize the hashtable from the file and  
			// assign the reference to the local variable.
			o = (List<HitmapEvent>)formatter.Deserialize(fs);

		}
		catch(XmlException e)
		{
			Debug.Log("Failed to deserialize. Reason: " + e.Message);
			throw;
		}
		finally
		{
			fs.Close();
		}
		return o;
	}


}
