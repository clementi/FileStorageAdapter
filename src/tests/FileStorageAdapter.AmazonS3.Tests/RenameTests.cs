#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Amazon Rename Request")]
	public class when_copying_an_object_on_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => storage.Rename(SourceKey, DestinationKey);

		It should_execute_a_copy_request_and_a_delete_request = () => client.Verify();

		private const string SourceKey = "Source";
		private const string DestinationKey = "Destination";

		private static void Setup()
		{
			InitializeComponents();
			ClientCopiesObject();
			ClientDeletesSourceObject();
		}
		private static void ClientCopiesObject()
		{
			client
				.Setup(x => x.CopyObject(
					Moq.It.Is<CopyObjectRequest>(
						y => y.SourceKey == SourceKey && y.DestinationKey == DestinationKey)))
				.Verifiable();
		}
		private static void ClientDeletesSourceObject()
		{
			client
				.Setup(x => x.DeleteObject(Moq.It.Is<DeleteObjectRequest>(y => y.Key == SourceKey)))
				.Verifiable();
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169