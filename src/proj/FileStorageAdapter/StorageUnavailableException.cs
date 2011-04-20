namespace FileStorageAdapter
{
	using System;
	using System.Runtime.Serialization;
	
	[Serializable]
	public class StorageUnavailableException : Exception
	{
		private const string ErrorMessageFormat = "Storage unavailable: {0}";
		
		public StorageUnavailableException(string message) : base(string.Format(ErrorMessageFormat, message))
		{
		}
		
		public StorageUnavailableException(string message, Exception inner) : base(string.Format(ErrorMessageFormat, message), inner)
		{
		}
		
		public StorageUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
