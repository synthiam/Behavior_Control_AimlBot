using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace de.springwald.toolbox.Serialisierung
{
	public static class DeepCloner<T>
	{
		static DeepCloner()
		{
		}

		public static T Clone(T oSource)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, oSource);
				memoryStream.Position = 0L;
				T result = (T)binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
				return result;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
