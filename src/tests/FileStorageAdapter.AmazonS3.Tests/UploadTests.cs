#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System.IO;
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Amazon Upload")]
	public class when_uploading_a_file_to_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => storage.Upload(Local, Remote);

		It should_stream_the_file_the_S3_bucket = () => client.Verify();

		private const string Local = "Local";
		private const string Remote = "Remote";
		private static readonly MemoryStream StreamedContents = new MemoryStream(new byte[] { 1, 2, 3 });

		private static void Setup()
		{
			InitializeComponents();
			FileSystemStreamsContents();
			ClientExpectsStreamedContents();
		}
		private static void FileSystemStreamsContents()
		{
			fileSystem
				.Setup(x => x.OpenRead(Local))
				.Returns(StreamedContents);
		}
		private static void ClientExpectsStreamedContents()
		{
			client
				.Setup(x => x.PutObject(Moq.It.Is<PutObjectRequest>(y => y.InputStream == StreamedContents)))
				.Verifiable();
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169