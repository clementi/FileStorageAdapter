namespace FileStorageAdapter.LocalFileSystem
{
	using System.Collections.Generic;
	using System.IO;

	public class LocalFileStorage : IStoreFiles
	{
		private readonly string remoteLocationPrefix;

		public LocalFileStorage(string remoteLocationPrefix)
		{
			if (!Directory.Exists(remoteLocationPrefix))
				Directory.CreateDirectory(remoteLocationPrefix);

			this.remoteLocationPrefix = remoteLocationPrefix;
		}

		private string Prefix(string path)
		{
			return Path.Combine(this.remoteLocationPrefix, path);
		}

		public void Download(string remotePath, string localPath)
		{
			File.Copy(this.Prefix(remotePath), localPath);
		}
		public void Upload(string localPath, string remotePath)
		{
			File.Copy(localPath, this.Prefix(remotePath));
		}
		
		public virtual bool Exists(string pathOrLocation)
		{
			pathOrLocation = this.Prefix(pathOrLocation);
			return File.Exists(pathOrLocation) || Directory.Exists(pathOrLocation);
		}
		public virtual void Rename(string source, string destination)
		{
			File.Move(this.Prefix(source), this.Prefix(destination));
		}
		public virtual void Delete(string path)
		{
			path = this.Prefix(path);

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
			return Directory.EnumerateFileSystemEntries(this.Prefix(location));
		}
		
		public virtual Stream Get(string path)
		{
			return File.OpenRead(this.Prefix(path));
		}
		public virtual void Put(Stream input, string path)
		{
			using (var output = File.OpenWrite(this.Prefix(path)))
				input.CopyTo(output);
		}
	}
}