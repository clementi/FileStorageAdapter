namespace FileStorageAdapter
{
	using System.Collections.Generic;
	using System.IO;

	public interface IStoreFiles
	{
		Stream Get(string path);
		void Put(Stream input, string location);
		void Delete(string path);

		bool Exists(string path);
		IEnumerable<string> EnumerateObjects(string location);
	}
}