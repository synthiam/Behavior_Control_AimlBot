using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class ToolboxImaging
{
	public static bool ResizeImageAndUpload(FileStream newFile, string folderPathAndFilenameNoExtension, double maxHeight, double maxWidth, int qualitaet)
	{
		try
		{
			Image image = Image.FromStream(newFile);
			int num = image.Width;
			int num2 = image.Height;
			if ((double)num > maxWidth)
			{
				float num3 = (float)num / (float)maxWidth;
				num = (int)((float)num / num3);
				num2 = (int)((float)num2 / num3);
			}
			if ((double)num2 > maxHeight)
			{
				float num3 = (float)num2 / (float)maxHeight;
				num2 = (int)((float)num2 / num3);
				num = (int)((float)num / num3);
			}
			Bitmap bitmap = new Bitmap(num, num2);
			Graphics graphics = Graphics.FromImage(bitmap);
			SolidBrush solidBrush = new SolidBrush(Color.White);
			graphics.FillRectangle(solidBrush, 0, 0, bitmap.Width, bitmap.Height);
			graphics.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
			solidBrush.Dispose();
			graphics.Dispose();
			image.Dispose();
			string text = folderPathAndFilenameNoExtension;
			if (!text.ToLower().EndsWith(".jpg"))
			{
				text += ".jpg";
			}
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			ToolboxImaging.SaveImageAsJpg(bitmap, text, qualitaet);
			bitmap.Dispose();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void SaveImageAsJpg(Bitmap bmp, string filename, long qualitaet)
	{
		EncoderParameters encoderParameters = new EncoderParameters(1);
		encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, qualitaet);
		bmp.Save(filename, ToolboxImaging.GetEncoder(ImageFormat.Jpeg), encoderParameters);
	}

	public static void SaveImageAsJpg(Bitmap bmp, Stream stream, long qualitaet)
	{
		EncoderParameters encoderParameters = new EncoderParameters(1);
		encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, qualitaet);
		bmp.Save(stream, ToolboxImaging.GetEncoder(ImageFormat.Jpeg), encoderParameters);
	}

	public static ImageCodecInfo GetEncoder(ImageFormat format)
	{
		ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
		ImageCodecInfo[] array = imageDecoders;
		foreach (ImageCodecInfo imageCodecInfo in array)
		{
			if (imageCodecInfo.FormatID == format.Guid)
			{
				return imageCodecInfo;
			}
		}
		return null;
	}
}
