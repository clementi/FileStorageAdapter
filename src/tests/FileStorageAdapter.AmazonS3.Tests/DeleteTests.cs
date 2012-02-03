#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using Amazon.S3.Model;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject("Amazon Delete Request")]
	public class when_deleting_an_object_on_amazon_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => storage.Delete(Key);

		It should_remove_the_item = () => client.Verify();

		const string Key = "Key";

		private static void Setup()
		{
			InitializeComponents();
			ClientDeletesObject();
		}
		private static void ClientDeletesObject()
		{
			client
				.Setup(x => x.DeleteObject(Moq.It.Is<DeleteObjectRequest>(y => y.Key == Key)))
				.Returns(true)
				.Verifiable();
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169