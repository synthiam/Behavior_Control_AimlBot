using System.IO;

namespace de.springwald.toolbox.file
{
	public class DateiChecksumme
	{
		private long[] pTable = new long[256];

		private long Poly = 3988292384L;

		public DateiChecksumme()
		{
			for (int i = 0; i < 256; i++)
			{
				long num = i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) != 1) ? (num >> 1) : (num >> 1 ^ this.Poly));
				}
				this.pTable[i] = num;
			}
		}

		public uint GetCRC32(string FileName)
		{
			int num = 4096;
			FileStream fileStream = new FileStream(FileName, FileMode.Open);
			long num2 = fileStream.Length;
			long num3 = 4294967295L;
			while (num2 > 0)
			{
				if (num2 < num)
				{
					num = (int)num2;
				}
				byte[] array = new byte[num];
				fileStream.Read(array, 0, num);
				for (int i = 0; i < num; i++)
				{
					num3 = (((num3 & 4294967040u) / 256 & 0xFFFFFF) ^ this.pTable[array[i] ^ (num3 & 0xFF)]);
				}
				num2 -= num;
			}
			fileStream.Close();
			num3 = -num3 - 1;
			return (uint)num3;
		}
	}
}
