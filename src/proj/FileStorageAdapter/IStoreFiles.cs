namespace FileStorageAdapter
{
	using System.Collections.Generic;
	using System.IO;

	public interface IStoreFiles
	{
		void Download(string remotePath, string localPath);
		void Upload(string localPath, string remotePath);

		bool Exists(string pathOrLocation);
		void Rename(string source, string destination);
		void Delete(string path);

		IEnumerable<string> EnumerateObjects(string location);
	
		Stream Get(string path);
		void Put(Stream input, string path);
	}
}
