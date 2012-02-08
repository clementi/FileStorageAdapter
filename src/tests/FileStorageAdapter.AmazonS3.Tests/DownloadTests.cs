#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System.IO;
	using System.Text;
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Amazon Download")]
	public class when_downloading_a_file_from_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => storage.Download(Remote, Local);

		It should_place_the_file_at_the_specified_location = () =>
		{
			DownloadedStream.Seek(0, SeekOrigin.Begin);
			var download = new StreamReader(DownloadedStream);
			download.ReadToEnd().ShouldEqual(Contents);
		};

		const string Remote = "Remote";
		const string Local = "Local";
		const string Contents = "Contents";
		private static readonly MemoryStream RemoteStream = new MemoryStream(new UTF8Encoding().GetBytes(Contents));
		private static readonly MemoryStream DownloadedStream = new MemoryStream();

		private static void Setup()
		{
			InitializeComponents();

			client
				.Setup(x => x.GetObject(Moq.It.Is<GetObjectRequest>(y => y.Key == Remote)))
				.Returns(RemoteStream);
			
			fileSystem
				.Setup(x => x.OpenWrite(Local))
				.Returns(new CapturingStream(DownloadedStream));
		}
	}

	internal class CapturingStream : Stream
	{
		private readonly MemoryStream capturer;

		public CapturingStream(MemoryStream capturer)
		{
			this.capturer = capturer;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.capturer.Write(buffer, offset, count);
		}

		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return true; } }

		#region--Not Implemented--

		public override void Flush()
		{
			throw new System.NotImplementedException();
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new System.NotImplementedException();
		}
		public override void SetLength(long value)
		{
			throw new System.NotImplementedException();
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException();
		}
		public override bool CanSeek
		{
			get { throw new System.NotImplementedException(); }
		}
		public override long Length
		{
			get { throw new System.NotImplementedException(); }
		}
		public override long Position
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		#endregion
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169