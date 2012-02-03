namespace FileStorageAdapter.AmazonS3
{
	using System.IO;

	public interface IFileSystem
	{
		Stream OpenRead(string path);
		Stream OpenWrite(string path);
	}

	public class FileSystem : IFileSystem
	{
		public Stream OpenRead(string path)
		{
			return File.OpenRead(path);
		}
		public Stream OpenWrite(string path)
		{
			return File.OpenWrite(path);
		}
	}
}