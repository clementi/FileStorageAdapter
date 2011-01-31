namespace FileStorageAdapter
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FileStorageException : Exception
	{
		private const string ErrorMessageFormat = "Unable to store files: {0}";

		public FileStorageException(string message) : base(string.Format(ErrorMessageFormat, message))
		{
		}

		public FileStorageException(string message, Exception inner) : base(string.Format(ErrorMessageFormat, message), inner)
		{
		}

		public FileStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}