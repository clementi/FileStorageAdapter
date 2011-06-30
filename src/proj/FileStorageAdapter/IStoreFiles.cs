namespace FileStorageAdapter
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public interface IStoreFiles
	{
		Stream Get(string path);
		string GetPreSignedUrl(string path, DateTime expiration);
		void Put(Stream input, string path);
		void Delete(string path);
		IEnumerable<string> EnumerateObjects(string location);
		bool Exists(string pathOrLocation);
	}
}