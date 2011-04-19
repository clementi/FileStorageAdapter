namespace FileStorageAdapter
{
    using System.Collections.Generic;
    using System.IO;

	public interface IStoreFiles
	{
		Stream Get(string path);
		void Put(Stream input, string path);
		void Delete(string path);
	    IEnumerable<string> EnumerateObjects(string location);
	}
}