namespace FileStorageAdapter.LocalFileSystem
{
	using System.Collections.Generic;
	using System.IO;

	public class LocalFileStorage : IStoreFiles
	{
		public void Download(string remotePath, string localPath)
		{
			File.Copy(remotePath, localPath);
		}
		public void Upload(string localPath, string remotePath)
		{
			File.Copy(localPath, remotePath);
		}
		
		public virtual bool Exists(string pathOrLocation)
		{
			return File.Exists(pathOrLocation) || Directory.Exists(pathOrLocation);
		}
		public virtual void Rename(string source, string destination)
		{
			File.Move(source, destination);
		}
		public virtual void Delete(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);

			try
			{
				File.Delete(path);
			}
			catch (DirectoryNotFoundException)
			{
				// in case another thread deletes the directory
			}
		}
		
		public virtual IEnumerable<string> EnumerateObjects(string location)
		{
			return Directory.EnumerateFileSystemEntries(location);
		}
		
		public virtual Stream Get(string path)
		{
			return File.OpenRead(path);
		}
		public virtual void Put(Stream input, string path)
		{
			using (var output = File.OpenWrite(path))
				input.CopyTo(output);
		}
	}
}