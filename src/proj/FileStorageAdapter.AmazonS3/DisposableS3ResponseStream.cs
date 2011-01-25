namespace FileStorageAdapter.AmazonS3
{
	using System.IO;
	using Amazon.S3.Model;

	public class DisposableS3ResponseStream : Stream
	{
		private readonly GetObjectResponse response;

		public DisposableS3ResponseStream(GetObjectResponse response)
		{
			this.response = response;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				this.response.Dispose();
		}

		public override void Flush()
		{
			this.response.ResponseStream.Flush();
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.response.ResponseStream.Seek(offset, origin);
		}
		public override void SetLength(long value)
		{
			this.response.ResponseStream.SetLength(value);
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.response.ResponseStream.Read(buffer, offset, count);
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.response.ResponseStream.Write(buffer, offset, count);
		}

		public override bool CanRead
		{
			get { return this.response.ResponseStream.CanRead; }
		}
		public override bool CanSeek
		{
			get { return this.response.ResponseStream.CanSeek; }
		}
		public override bool CanWrite
		{
			get { return this.response.ResponseStream.CanWrite; }
		}
		public override long Length
		{
			get { return this.response.ResponseStream.Length; }
		}
		public override long Position
		{
			get { return this.response.ResponseStream.Position; }
			set { this.response.ResponseStream.Position = value; }
		}
	}
}