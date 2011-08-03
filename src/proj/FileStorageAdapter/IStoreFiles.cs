namespace FileStorageAdapter
{
	using System.Collections.Generic;
	using System.IO;

	public interface IStoreFiles
	{
		Stream Get(string path);
		void Put(Stream input, string path);
		void Delete(string path);
		void Rename(string source, string destination);
		IEnumerable<string> EnumerateObjects(string location);
		bool Exists(string pathOrLocation);
	}
}
