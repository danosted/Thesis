﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class Serializer
{
	private string fileLogname = "filelog.xml";
	private string filepath = Application.persistentDataPath + "/";

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

	public void SerializeHitmap(List<GazeEvent> o, string filename)
	{
		FileStream fs = new FileStream(filepath + filename, FileMode.Create);
		XmlSerializer formatter = new XmlSerializer(typeof(List<GazeEvent>));
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

	public List<GazeEvent> DeserializeHitmap(string filename)
	{
		FileStream fs = new FileStream(filepath + filename, FileMode.Open);
		List<GazeEvent> o = null;
		try
		{
			XmlSerializer formatter = new XmlSerializer(typeof(List<GazeEvent>));
			o = (List<GazeEvent>)formatter.Deserialize(fs);
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

	public void SerializeFilenames(HashSet<string> o)
	{
		FileStream fs = new FileStream(filepath + fileLogname, FileMode.Create);
		XmlSerializer formatter = new XmlSerializer(typeof(HashSet<string>));
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
	
	public HashSet<string> DeserializeFilenames()
	{
		FileStream fs = new FileStream(filepath + fileLogname, FileMode.Open);
		HashSet<string> o = null;
		try
		{
			XmlSerializer formatter = new XmlSerializer(typeof(HashSet<string>));
			o = (HashSet<string>)formatter.Deserialize(fs);
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
