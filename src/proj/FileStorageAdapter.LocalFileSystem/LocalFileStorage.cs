namespace FileStorageAdapter.LocalFileSystem
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class LocalFileStorage : IStoreFiles
	{
		private const string LocalUrlTemplate = "file:///{0}";
		private readonly string remoteLocationPrefix;

		public LocalFileStorage(string remoteLocationPrefix)
		{
			EnsureDirectory(remoteLocationPrefix);

			this.remoteLocationPrefix = remoteLocationPrefix;
		}
		private static void EnsureDirectory(string path)
		{
			if (string.IsNullOrEmpty(path))
				return;

			var directory = Path.GetDirectoryName(path);
			if (string.IsNullOrEmpty(directory))
				return;

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}

		public string GetDownloadUrl(string path)
		{
			return string.Format(LocalUrlTemplate, this.Prefix(path));
		}
		public void Download(string remotePath, string localPath)
		{
			EnsureDirectory(localPath);

			try
			{
				File.Copy(this.Prefix(remotePath), localPath, true);
			}
			catch (DirectoryNotFoundException e)
			{
				throw new FileNotFoundException(e.Message, Path.GetFileName(remotePath), e);
			}
		}
		public void Upload(string localPath, string remotePath)
		{
			var destination = this.Prefix(remotePath);
			EnsureDirectory(destination);

			File.Copy(localPath, destination, true);
		}
		
		public virtual bool Exists(string pathOrLocation)
		{
			pathOrLocation = this.Prefix(pathOrLocation);
			return File.Exists(pathOrLocation) || Directory.Exists(pathOrLocation);
		}
		public virtual void Rename(string source, string destination)
		{
			destination = this.Prefix(destination);
			EnsureDirectory(destination);
			File.Move(this.Prefix(source), destination);
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
			try
			{
				return File.OpenRead(this.Prefix(path));
			}
			catch (DirectoryNotFoundException e)
			{
				throw new FileNotFoundException(e.Message, Path.GetFileName(path), e);
			}
		}
		public virtual void Put(Stream input, string path)
		{
			var destination = this.Prefix(path);
			EnsureDirectory(destination);

			using (var output = File.OpenWrite(destination))
				input.CopyTo(output);
		}

		private string Prefix(string path)
		{
			if (path.StartsWith("/") || path.StartsWith("\\"))
				path = path.Substring(1);

			return Path.Combine(this.remoteLocationPrefix, path);
		}
	}
}