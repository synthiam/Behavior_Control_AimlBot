using de.springwald.toolbox.IO;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace de.springwald.toolbox.Serialisierung
{
	public static class GenericSerializationHelperClass<T>
	{
		public class GenericBinder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{
				string[] array = typeName.Split('.');
				bool flag = array[0].ToString() == "System";
				string str = array[array.Length - 1];
				Type type = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
				if (!flag && type == null)
				{
					Assembly assembly = Assembly.GetAssembly(typeof(T));
					if (assembly == null)
					{
						throw new ArgumentException("Assembly for type '" + typeof(T).Name.ToString() + "' could not be loaded.");
					}
					string text = assembly.FullName.Split(',')[0];
					Type type2 = assembly.GetType(text + "." + str);
					if (type2 == null)
					{
						throw new ArgumentException("Type '" + typeName + "' could not be loaded from assembly '" + text + "'.");
					}
					type = type2;
				}
				return type;
			}
		}

		static GenericSerializationHelperClass()
		{
		}

		public static bool ToXmlFile(T cryo, string fileName, bool zipped)
		{
			if (zipped)
			{
				return GenericSerializationHelperClass<T>.ToXmlZipFile(cryo, fileName);
			}
			return GenericSerializationHelperClass<T>.ToXmlFile(cryo, fileName);
		}

		internal static bool ToXmlZipFile(T cryo, string fileName)
		{
			DirectoryInfo directory = new FileInfo(fileName).Directory;
			if (cryo != null && directory != null && directory.Exists)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					using (GZipStream stream2 = new GZipStream(stream, CompressionMode.Compress, true))
					{
						xmlSerializer.Serialize(stream2, cryo);
					}
				}
				return true;
			}
			return false;
		}

		internal static bool ToXmlFile(T cryo, string fileName)
		{
			DirectoryInfo directory = new FileInfo(fileName).Directory;
			if (cryo != null && directory != null && directory.Exists)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					xmlSerializer.Serialize(stream, cryo);
				}
				return true;
			}
			return false;
		}

		public static T FromXmlFile(string frozenObjectFileName, bool zipped)
		{
			if (zipped)
			{
				return GenericSerializationHelperClass<T>.FromGZipXmlFile(frozenObjectFileName);
			}
			return GenericSerializationHelperClass<T>.FromXmlFile(frozenObjectFileName);
		}

		internal static T FromXmlFile(string frozenObjectFileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			if (frozenObjectFileName != null && File.Exists(frozenObjectFileName))
			{
				using (FileStream stream = new FileStream(frozenObjectFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					return (T)xmlSerializer.Deserialize(stream);
				}
			}
			return default(T);
		}

		internal static T FromGZipXmlFile(string frozenObjectFileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			if (frozenObjectFileName != null && File.Exists(frozenObjectFileName))
			{
				using (FileStream stream = new FileStream(frozenObjectFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress))
					{
						return (T)xmlSerializer.Deserialize(stream2);
					}
				}
			}
			return default(T);
		}

		public static string ToXmlString(T cryo, bool zipped)
		{
			if (zipped)
			{
				return GenericSerializationHelperClass<T>.ToXmlZipString(cryo);
			}
			return GenericSerializationHelperClass<T>.ToXmlString(cryo);
		}

		public static string ToXmlString(T cryo)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			TextWriter textWriter = new StringWriter();
			try
			{
				xmlSerializer.Serialize(textWriter, cryo);
			}
			finally
			{
				textWriter.Flush();
				textWriter.Close();
			}
			return textWriter.ToString();
		}

		public static string ToUTF8XmlString(T cryo)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, uTF8Encoding);
			try
			{
				xmlSerializer.Serialize(xmlTextWriter, cryo);
				byte[] bytes = memoryStream.ToArray();
				return uTF8Encoding.GetString(bytes);
			}
			finally
			{
				memoryStream.Close();
				xmlTextWriter.Flush();
				xmlTextWriter.Close();
			}
		}

		internal static string ToXmlZipString(T cryo)
		{
			string s = GenericSerializationHelperClass<T>.ToXmlString(cryo);
			byte[] bytes = Encoding.Unicode.GetBytes(s);
			byte[] inArray = IOTools.Compress(bytes);
			return Convert.ToBase64String(inArray);
		}

		internal static string ToXmlZipStringBinary(T cryo)
		{
			byte[] input = GenericSerializationHelperClass<T>.ToBinary(cryo);
			byte[] inArray = IOTools.Compress(input);
			return Convert.ToBase64String(inArray);
		}

		internal static T FromXmlZipString(string frozen)
		{
			byte[] input = Convert.FromBase64String(frozen);
			byte[] bytes = IOTools.Uncompress(input);
			return GenericSerializationHelperClass<T>.FromXml(Encoding.Unicode.GetString(bytes));
		}

		internal static T FromXmlZipStringBinary(string frozen)
		{
			byte[] input = Convert.FromBase64String(frozen);
			byte[] frozen2 = IOTools.Uncompress(input);
			return GenericSerializationHelperClass<T>.FromBinary(frozen2);
		}

		public static T FromXml(string frozen, bool zipped)
		{
			if (zipped)
			{
				return GenericSerializationHelperClass<T>.FromXmlZipString(frozen);
			}
			return GenericSerializationHelperClass<T>.FromXml(frozen);
		}

		public static T FromXml(string frozen)
		{
			if (frozen.Length <= 0)
			{
				throw new ArgumentOutOfRangeException("frozenObject", "Cannot thaw a zero-length string");
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			TextReader textReader = new StringReader(frozen);
			object obj = default(T);
			try
			{
				obj = xmlSerializer.Deserialize(textReader);
			}
			finally
			{
				textReader.Close();
			}
			return (T)obj;
		}

		public static string ToXmlStringWithOutXmlns(T cryo)
		{
			return GenericSerializationHelperClass<T>.ToXmlStringWithOutXmlns(cryo, Encoding.GetEncoding("ISO-8859-15"));
		}

		public static string ToXmlStringWithOutXmlns(T cryo, Encoding encoding)
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("", "");
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlSerializer.Serialize(xmlTextWriter, cryo, xmlSerializerNamespaces);
				return encoding.GetString(memoryStream.ToArray());
			}
		}

		public static bool ToXmlFileWithOutXmlns(T cryo, string fileName)
		{
			return GenericSerializationHelperClass<T>.ToXmlFileWithOutXmlns(cryo, fileName, Encoding.GetEncoding("ISO-8859-15"));
		}

		public static bool ToXmlFileWithOutXmlns(T cryo, string fileName, Encoding encoding)
		{
			try
			{
				DirectoryInfo directory = new FileInfo(fileName).Directory;
				if (cryo != null && directory != null && directory.Exists)
				{
					string value = GenericSerializationHelperClass<T>.ToXmlStringWithOutXmlns(cryo, encoding);
					TextWriter textWriter = new StreamWriter(fileName, false, encoding);
					textWriter.Write(value);
					textWriter.Close();
					return true;
				}
				return false;
			}
			catch (DirectoryNotFoundException)
			{
				return false;
			}
		}

		public static bool ToXmlFileEncoded(T cryo, string fileName, Encoding encoding)
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("", "");
			DirectoryInfo directory = new FileInfo(fileName).Directory;
			if (cryo != null && directory != null && directory.Exists)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				using (Stream w = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					XmlTextWriter xmlWriter = new XmlTextWriter(w, encoding);
					xmlSerializer.Serialize(xmlWriter, cryo, xmlSerializerNamespaces);
				}
				return true;
			}
			return false;
		}

		public static XmlDocument ToXmlDocument(T cryo)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			XmlDocument xmlDocument = new XmlDocument();
			using (Stream stream = new MemoryStream())
			{
				xmlSerializer.Serialize(stream, cryo);
				stream.Position = 0L;
				xmlDocument.Load(stream);
			}
			return xmlDocument;
		}

		public static T FromXml(XmlDocument frozen)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			Stream stream = new MemoryStream();
			frozen.Save(stream);
			try
			{
				stream.Position = 0L;
				return (T)xmlSerializer.Deserialize(stream);
			}
			finally
			{
				stream.Close();
			}
		}

		public static bool ToBinaryFile(T cryo, string fileName, bool zipped)
		{
			if (zipped)
			{
				return GenericSerializationHelperClass<T>.ToBinaryZipFile(cryo, fileName);
			}
			return GenericSerializationHelperClass<T>.ToBinaryFile(cryo, fileName);
		}

		internal static bool ToBinaryZipFile(T cryo, string fileName)
		{
			DirectoryInfo directory = new FileInfo(fileName).Directory;
			if (cryo != null && directory != null && directory.Exists)
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Create))
				{
					using (GZipStream serializationStream = new GZipStream(stream, CompressionMode.Compress))
					{
						IFormatter formatter = new BinaryFormatter();
						formatter.Serialize(serializationStream, cryo);
					}
				}
				return true;
			}
			return false;
		}

		internal static bool ToBinaryFile(T cryo, string fileName)
		{
			DirectoryInfo directory = new FileInfo(fileName).Directory;
			if (cryo != null && directory != null && directory.Exists)
			{
				using (FileStream serializationStream = new FileStream(fileName, FileMode.Create))
				{
					IFormatter formatter = new BinaryFormatter();
					formatter.Serialize(serializationStream, cryo);
				}
				return true;
			}
			return false;
		}

		public static T FromBinaryFile(string frozenObjectFileName, bool zipped)
		{
			if (zipped)
			{
				return GenericSerializationHelperClass<T>.FromBinaryZipFile(frozenObjectFileName);
			}
			return GenericSerializationHelperClass<T>.FromBinaryFile(frozenObjectFileName);
		}

		internal static T FromBinaryFile(string frozenObjectFileName)
		{
			if (frozenObjectFileName != null && File.Exists(frozenObjectFileName))
			{
				using (FileStream serializationStream = new FileStream(frozenObjectFileName, FileMode.Open))
				{
					IFormatter formatter = new BinaryFormatter();
					return (T)formatter.Deserialize(serializationStream);
				}
			}
			return default(T);
		}

		internal static T FromBinaryZipFile(string frozenObjectFileName)
		{
			if (frozenObjectFileName != null && File.Exists(frozenObjectFileName))
			{
				using (FileStream stream = new FileStream(frozenObjectFileName, FileMode.Open))
				{
					using (GZipStream serializationStream = new GZipStream(stream, CompressionMode.Decompress))
					{
						IFormatter formatter = new BinaryFormatter();
						return (T)formatter.Deserialize(serializationStream);
					}
				}
			}
			return default(T);
		}

		public static byte[] ToByreArrayMitXmlSerializer(T cryo)
		{
			MemoryStream memoryStream = new MemoryStream();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			try
			{
				xmlSerializer.Serialize(memoryStream, cryo);
				memoryStream.Position = 0L;
				return memoryStream.ToArray();
			}
			finally
			{
				memoryStream.Close();
			}
		}

		public static T FromByteArrayMitXmlSerializer(byte[] frozen)
		{
			if (frozen == null && frozen.Length == 0)
			{
				throw new ArgumentOutOfRangeException("frozenObject", "Cannot thaw a zero-length Byte[] array");
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			Stream stream = new MemoryStream(frozen);
			try
			{
				stream.Position = 0L;
				return (T)xmlSerializer.Deserialize(stream);
			}
			finally
			{
				stream.Close();
			}
		}

		public static byte[] ToBinary(T cryo)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			try
			{
				binaryFormatter.Serialize(stream, cryo);
				byte[] array = new byte[stream.Length];
				stream.Seek(0L, SeekOrigin.Begin);
				stream.Read(array, 0, array.Length);
				return array;
			}
			finally
			{
				stream.Close();
			}
		}

		public static T FromBinary(byte[] frozen)
		{
			if (frozen.Length == 0)
			{
				throw new ArgumentOutOfRangeException("frozenObject", "Cannot thaw a zero-length Byte[] array");
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			Stream stream = new MemoryStream(frozen);
			try
			{
				return (T)binaryFormatter.Deserialize(stream);
			}
			finally
			{
				stream.Close();
			}
		}

		public static T FromBinary(byte[] frozen, SerializationBinder customBinder)
		{
			if (frozen.Length == 0)
			{
				throw new ArgumentOutOfRangeException("frozenObject", "Cannot thaw a zero-length Byte[] array");
			}
			if (customBinder == null)
			{
				throw new ArgumentNullException("customBinder", "SerializationBinder implementation cannot be null");
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Binder = customBinder;
			Stream stream = new MemoryStream(frozen);
			try
			{
				return (T)binaryFormatter.Deserialize(stream);
			}
			finally
			{
				stream.Close();
			}
		}

		public static SqlXml ToSqlXml(T cryo)
		{
			throw new NotImplementedException("SqlXml type not implemented");
		}

		public static T FromSqlXml(SqlXml frozen)
		{
			throw new NotImplementedException("SqlXml type not implemented");
		}

		public static T Clone(T oSource)
		{
			MemoryStream memoryStream = new MemoryStream();
			object obj = null;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				xmlSerializer.Serialize(memoryStream, oSource);
				memoryStream.Position = 0L;
				xmlSerializer = new XmlSerializer(typeof(T));
				obj = xmlSerializer.Deserialize(memoryStream);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
			return (T)obj;
		}

		public static MemoryStream ToBinaryZipStream(T cryo, string fileName)
		{
			MemoryStream memoryStream = new MemoryStream();
			GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(gZipStream, cryo);
			gZipStream.Close();
			memoryStream.Position = 0L;
			return memoryStream;
		}

		public static T FromBinaryZipStream(MemoryStream fs)
		{
			using (GZipStream serializationStream = new GZipStream(fs, CompressionMode.Decompress))
			{
				IFormatter formatter = new BinaryFormatter();
				return (T)formatter.Deserialize(serializationStream);
			}
		}
	}
}
