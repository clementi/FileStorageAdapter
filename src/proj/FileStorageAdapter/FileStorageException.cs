namespace FileStorageAdapter
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FileStorageException : Exception
	{
		public FileStorageException(string message) : base(message)
		{
		}

		public FileStorageException(string message, Exception inner) : base(message, inner)
		{
		}

		public FileStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}