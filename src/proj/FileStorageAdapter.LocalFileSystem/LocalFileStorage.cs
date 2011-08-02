namespace FileStorageAdapter.LocalFileSystem
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class LocalFileStorage : IStoreFiles
	{
		public Stream Get(string path)
		{
			return File.OpenRead(path);
		}

		public void Put(Stream input, string path)
		{
			using (var output = File.OpenWrite(path))
				input.CopyTo(output);
		}

		public void Delete(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);

			File.Delete(path);
		}

		public IEnumerable<string> EnumerateObjects(string location)
		{
			return Directory.EnumerateFileSystemEntries(location);
		}

		public bool Exists(string pathOrLocation)
		{
			return File.Exists(pathOrLocation) || Directory.Exists(pathOrLocation);
		}
	}
}