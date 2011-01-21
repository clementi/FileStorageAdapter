namespace FileStorageAdapter
{
	public class RemotePath
	{
		private const char Backslash = '\\';
		private const char ForwardSlash = '/';

		public static string Normalize(string path)
		{
			return path.Replace(Backslash, ForwardSlash);
		}
	}
}