using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class Serializer
{
	private string hitmapPath = Application.persistentDataPath + "/" + "hitmap.dat";
	private string eyeDataPath = Application.persistentDataPath + "/" + "eyedata.dat";

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

	public void SerializeHitmap(List<HitmapEvent> o)
	{
		FileStream fs = new FileStream(hitmapPath, FileMode.Create);
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

	public List<HitmapEvent> DeserializeHitmap()
	{
		FileStream fs = new FileStream(hitmapPath, FileMode.Open);
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

	public void SerializeEyedata(List<EyeEvent> o)
	{
		FileStream fs = new FileStream(eyeDataPath, FileMode.Create);
		XmlSerializer formatter = new XmlSerializer(typeof(List<EyeEvent>));
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
	
	public List<EyeEvent> DeserializeEyedata()
	{
		FileStream fs = new FileStream(eyeDataPath, FileMode.Open);
		List<EyeEvent> o = null;
		try
		{
			XmlSerializer formatter = new XmlSerializer(typeof(List<EyeEvent>));
			
			// Deserialize the hashtable from the file and  
			// assign the reference to the local variable.
			o = (List<EyeEvent>)formatter.Deserialize(fs);
			
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

//	public void SerializeTest(object o)
//	{
//		FileStream fs = new FileStream(hitmapPath, FileMode.Create);
//		XmlSerializer formatter = new XmlSerializer(typeof(object));
//		try
//		{
//			formatter.Serialize(fs, o);
//		}
//		catch(XmlException e)
//		{
//			Debug.Log("Failed to serialize. Reason: " + e.Message);
//			throw;
//		}
//		finally
//		{
//			fs.Close();
//		}
//	}
//
//	public object DeserializeTest()
//	{
//		FileStream fs = new FileStream(hitmapPath, FileMode.Open);
//		object o = null;
//		try
//		{
//			XmlSerializer formatter = new XmlSerializer(typeof(object));
//			
//			// Deserialize the hashtable from the file and  
//			// assign the reference to the local variable.
//			o = formatter.Deserialize(fs);
//			
//		}
//		catch(XmlException e)
//		{
//			Debug.Log("Failed to deserialize. Reason: " + e.Message);
//			throw;
//		}
//		finally
//		{
//			fs.Close();
//		}
//		return o;
//	}


}
