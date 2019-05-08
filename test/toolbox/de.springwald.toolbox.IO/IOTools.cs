using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace de.springwald.toolbox.IO
{
	public class IOTools
	{
		public static readonly Encoding ISO88591Encoding = Encoding.GetEncoding("ISO-8859-1");

		public static readonly Encoding ISO885915Encoding = Encoding.GetEncoding("ISO-8859-15");

		private IOTools()
		{
		}

		[Obsolete("Der Name der Methode ist falsch: Verwende CopyStreamChunked")]
		public static int CopyStream(Stream instream, Stream outstream)
		{
			return IOTools.CopyStreamChunked(instream, outstream);
		}

		public static int CopyStreamChunked(Stream instream, Stream outstream)
		{
			byte[] buffer = new byte[4096];
			int num = 0;
			int num2 = 0;
			while ((num = instream.Read(buffer, 0, 4096)) > 0)
			{
				outstream.Write(buffer, 0, num);
				num2 += num;
			}
			return num2;
		}

		public static int CopyStreamAtOnce(Stream instream, Stream outstream)
		{
			if (instream.CanSeek)
			{
				byte[] array = new byte[instream.Length];
				int num = instream.Read(array, 0, array.Length);
				outstream.Write(array, 0, num);
				array = null;
				return num;
			}
			return IOTools.CopyStreamChunked(instream, outstream);
		}

		public static int CopyStreamSelect(Stream instream, Stream outstream)
		{
			if (instream.CanSeek && instream.Length < 1048577)
			{
				return IOTools.CopyStreamAtOnce(instream, outstream);
			}
			return IOTools.CopyStreamChunked(instream, outstream);
		}

		[Obsolete("Der Name der Methode ist falsch: Verwende CopyStreamChunked")]
		public static long CopyStream(Stream instream, Stream outstream, long length)
		{
			return IOTools.CopyStreamChunked(instream, outstream, length);
		}

		public static long CopyStreamChunked(Stream instream, Stream outstream, long length)
		{
			byte[] buffer = new byte[4096];
			int num = 0;
			long num2 = 0L;
			while ((num = instream.Read(buffer, 0, (int)((4096 < length) ? 4096 : length))) > 0)
			{
				outstream.Write(buffer, 0, num);
				length -= num;
				num2 += num;
			}
			return num2;
		}

		public static long CopyStreamAtOnce(Stream instream, Stream outstream, long length)
		{
			if (instream.CanSeek)
			{
				byte[] array = new byte[Math.Min(instream.Length, length)];
				int num = instream.Read(array, 0, array.Length);
				outstream.Write(array, 0, num);
				array = null;
				return num;
			}
			return IOTools.CopyStreamChunked(instream, outstream, length);
		}

		public static byte[] StreamToBytesChunked(Stream instream)
		{
			byte[] array = new byte[4096];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = instream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				return memoryStream.ToArray();
			}
		}

		public static byte[] StreamToBytesAtOnce(Stream instream)
		{
			if (instream.CanSeek)
			{
				byte[] array = new byte[instream.Length];
				instream.Read(array, 0, (int)instream.Length);
				return array;
			}
			return IOTools.StreamToBytesChunked(instream);
		}

		[Obsolete("Der Name der Methode ist falsch: Verwende StreamToBytesAtOnce")]
		public static byte[] StreamToBytes(Stream instream)
		{
			return IOTools.StreamToBytesAtOnce(instream);
		}

		public static void CompressStreamGZipStream(Stream instream, Stream outstream)
		{
			GZipStream gZipStream = new GZipStream(outstream, CompressionMode.Compress, true);
			IOTools.CopyStreamChunked(instream, gZipStream);
			gZipStream.Close();
			gZipStream = null;
		}

		public static void DecompressGZipStream(Stream instream, Stream outstream)
		{
			GZipStream gZipStream = new GZipStream(instream, CompressionMode.Decompress, true);
			IOTools.CopyStreamChunked(gZipStream, outstream);
			gZipStream.Close();
			gZipStream = null;
		}

		public static void CompressStreamGZipStreamSelect(Stream instream, Stream outstream, bool datacheck)
		{
			Stream stream = datacheck ? ((Stream)new GZipStream(outstream, CompressionMode.Compress, true)) : ((Stream)new DeflateStream(outstream, CompressionMode.Compress, true));
			IOTools.CopyStreamSelect(instream, stream);
			stream.Close();
			stream = null;
		}

		public static void DecompressGZipStreamSelect(Stream instream, Stream outstream, bool datacheck)
		{
			Stream stream = datacheck ? ((Stream)new GZipStream(instream, CompressionMode.Decompress, true)) : ((Stream)new DeflateStream(instream, CompressionMode.Decompress, true));
			IOTools.CopyStreamSelect(stream, outstream);
			stream.Close();
			stream = null;
		}

		[Obsolete("Der Name der Methode ist falsch: Verwende CompressWithLength")]
		public static byte[] Compress(byte[] input)
		{
			return IOTools.CompressWithLength(input);
		}

		public static byte[] CompressWithLength(byte[] input)
		{
			MemoryStream memoryStream = new MemoryStream();
			GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
			IOTools.BytesToStream(input, gZipStream);
			gZipStream.Close();
			memoryStream.Position = 0L;
			MemoryStream memoryStream2 = new MemoryStream();
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Read(array, 0, array.Length);
			byte[] array2 = new byte[array.Length + 4];
			Buffer.BlockCopy(array, 0, array2, 4, array.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(input.Length), 0, array2, 0, 4);
			return array2;
		}

		[Obsolete("Der Name der Methode ist falsch: Verwende UncompressWithLength")]
		public static byte[] Uncompress(byte[] input)
		{
			return IOTools.UncompressWithLength(input);
		}

		public static byte[] UncompressWithLength(byte[] input)
		{
			MemoryStream memoryStream = new MemoryStream();
			int num = BitConverter.ToInt32(input, 0);
			memoryStream.Write(input, 4, input.Length - 4);
			byte[] array = new byte[num];
			memoryStream.Position = 0L;
			GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
			gZipStream.Read(array, 0, array.Length);
			return array;
		}

		public static string StreamISO88591ToString(Stream instream)
		{
			StreamReader streamReader = new StreamReader(instream, IOTools.ISO88591Encoding);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}

		public static string StreamToString(Stream instream, Encoding encoding)
		{
			StreamReader streamReader = new StreamReader(instream, encoding);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}

		public static void StreamToFileChunked(Stream instream, string filename)
		{
			DirectoryInfo directory = new FileInfo(filename).Directory;
			if (!directory.Exists)
			{
				directory.Create();
			}
			FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
			IOTools.CopyStreamChunked(instream, fileStream);
			fileStream.Close();
		}

		[Obsolete("Der Name der Methode ist falsch: Verwende StreamToFileChunked bzw. StreamToFileAtOnce")]
		public static void StreamToFile(Stream instream, string filename)
		{
			DirectoryInfo directory = new FileInfo(filename).Directory;
			if (!directory.Exists)
			{
				directory.Create();
			}
			FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
			IOTools.CopyStreamChunked(instream, fileStream);
			fileStream.Close();
		}

		public static void StreamToFileAtOnce(Stream instream, string filename)
		{
			DirectoryInfo directory = new FileInfo(filename).Directory;
			if (!directory.Exists)
			{
				directory.Create();
			}
			FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
			IOTools.CopyStreamAtOnce(instream, fileStream);
			fileStream.Close();
		}

		public static byte[] ReadBytesFromStream(Stream instream, int offset, int length)
		{
			if (offset + length > instream.Length)
			{
				throw new ApplicationException("Versuch über das Ende des Streams hinaus zu lesen.");
			}
			instream.Seek(offset, SeekOrigin.Begin);
			byte[] array = new byte[length];
			instream.Read(array, 0, array.Length);
			return array;
		}

		public static Stream ReadStreamFromStream(Stream instream, long offset, long length)
		{
			MemoryStream memoryStream = new MemoryStream();
			instream.Seek(offset, SeekOrigin.Begin);
			IOTools.CopyStreamChunked(instream, memoryStream, length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		public static void BytesToStream(byte[] srcdata, Stream outstream)
		{
			int num = 4096;
			byte[] array = new byte[num];
			for (int i = 0; i < srcdata.Length; i += num)
			{
				if (srcdata.Length - i < num)
				{
					num = srcdata.Length - i;
				}
				Buffer.BlockCopy(srcdata, i, array, 0, num);
				outstream.Write(array, 0, num);
			}
		}

		public static string ReadFileShared(string filename)
		{
			return IOTools.ReadFileShared(filename, IOTools.ISO88591Encoding);
		}

		public static string ReadFileShared(string filename, Encoding instreamencoding)
		{
			Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader streamReader = new StreamReader(stream, instreamencoding);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			stream.Close();
			return result;
		}

		public static byte[] ReadFileBytesShared(string filename)
		{
			Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] result = IOTools.StreamToBytes(stream);
			stream.Close();
			return result;
		}

		[Obsolete("Verwende ReadFileShared", true)]
		public static string LadeDateiText(string dateiname, Encoding dateiEncoding)
		{
			string result = default(string);
			using (StreamReader streamReader = new StreamReader(dateiname, dateiEncoding))
			{
				result = streamReader.ReadToEnd();
				streamReader.Close();
			}
			return result;
		}

		[Obsolete("Verwende ReadFileShared", true)]
		public static string LadeDateiText(string dateiname)
		{
			return IOTools.LadeDateiText(dateiname, IOTools.ISO88591Encoding);
		}

		public static void SchreibeDateiText(string dateiname, string dateitext, Encoding dateiEncoding)
		{
			DirectoryInfo directory = new FileInfo(dateiname).Directory;
			if (!directory.Exists)
			{
				directory.Create();
			}
			using (StreamWriter streamWriter = new StreamWriter(dateiname, false, dateiEncoding))
			{
				streamWriter.Write(dateitext);
				streamWriter.Close();
			}
		}

		public static void SchreibeDateiText(string dateiname, string dateitext)
		{
			IOTools.SchreibeDateiText(dateiname, dateitext, IOTools.ISO88591Encoding);
		}

		public static void SchreibeDateiText(string dateiname, string dateitext, Encoding dateiEncoding, DateTime creationTimeUtc, DateTime lastWriteTimeUtc)
		{
			IOTools.SchreibeDateiText(dateiname, dateitext, dateiEncoding);
			FileInfo fileInfo = new FileInfo(dateiname);
			fileInfo.CreationTimeUtc = creationTimeUtc;
			fileInfo.LastWriteTimeUtc = lastWriteTimeUtc;
		}

		public static void SchreibeDateiText(string dateiname, string dateitext, DateTime creationTimeUtc, DateTime lastWriteTimeUtc)
		{
			IOTools.SchreibeDateiText(dateiname, dateitext, IOTools.ISO88591Encoding, creationTimeUtc, lastWriteTimeUtc);
		}

		public static string PfadVerbinder(string basispfad, string relativerpfad)
		{
			if (basispfad == null)
			{
				basispfad = "";
			}
			if (relativerpfad == null)
			{
				relativerpfad = "";
			}
			basispfad.Replace("/", "\\");
			relativerpfad.Replace("/", "\\");
			if (relativerpfad.IndexOf(":") > -1 || relativerpfad.StartsWith("\\\\"))
			{
				basispfad = "";
			}
			if (basispfad != "" && !basispfad.EndsWith("\\"))
			{
				basispfad += "\\";
			}
			string text = basispfad + relativerpfad;
			text = text.Replace("\\.\\", "\\");
			while (text.IndexOf("\\..\\") > -1)
			{
				int num = text.IndexOf("\\..\\");
				text = text.Substring(0, text.Substring(0, num).LastIndexOf("\\") + 1) + text.Substring(num + 4);
			}
			while (text.LastIndexOf("\\->\\") > -1)
			{
				int num2 = text.LastIndexOf("\\->\\");
				text = text.Substring(0, num2) + text.Substring(text.IndexOf("\\", num2 + 4));
			}
			return text;
		}
	}
}
