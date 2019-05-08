using System;
using System.IO;
using System.Xml.Serialization;

namespace de.springwald.toolbox
{
	public static class GenericConverter<SourceType, DestType>
	{
		static GenericConverter()
		{
		}

		public static DestType ConvertTo(SourceType oSource)
		{
			MemoryStream memoryStream = new MemoryStream();
			object obj = null;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SourceType));
				xmlSerializer.Serialize(memoryStream, oSource);
				memoryStream.Position = 0L;
				xmlSerializer = new XmlSerializer(typeof(DestType));
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
			return (DestType)obj;
		}
	}
}
