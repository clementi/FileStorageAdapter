namespace FileStorageAdapter
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public interface IStoreFiles
	{
		string GetDownloadUrl(string path);
		string GetDownloadUrl(string path, string fileName);
		void Download(string remotePath, string localPath);
		void Upload(string localPath, string remotePath);

		bool Exists(string pathOrLocation);
		void Rename(string source, string destination);
		void Delete(string path);

		IEnumerable<string> EnumerateObjects(string location);
		IEnumerable<string> EnumerateObjects(string location, Func<DateTime, bool> lastModifiedFilter);

		Stream Get(string path);
		void Put(Stream input, string path);
	}
}
