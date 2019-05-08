using System.Collections.Generic;
using System.IO;

namespace de.springwald.toolbox.IO
{
	public interface IFileLeseZugriff
	{
		bool FileExists(string filepathRelative);

		Stream GetFile(string filepathRelative);

		FileInfoRelative GetFileInfo(string filepathRelative);

		string GetFileContentAsString(string filepathRelative);

		bool TryGetFile(string filepathRelative, out Stream filestream);

		bool DirectoryExist(string directoryPathRelative);

		IEnumerable<string> GetFiles(string directoryPathRelative, string searchPattern);

		IEnumerable<string> GetFiles(string directoryPathRelative);

		IEnumerable<FileInfoRelative> GetFileInfos(string directoryPathRelative, string searchPattern);

		IEnumerable<FileInfoRelative> GetFileInfos(string directoryPathRelative);
	}
}
