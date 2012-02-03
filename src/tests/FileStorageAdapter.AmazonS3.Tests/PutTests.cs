#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System.IO;
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Amazon Put Request")]
	public class when_putting_an_object_on_S3 : using_amazon_storage
	{
		Establish context = Setup;
		
		Because of = () => storage.Put(new MemoryStream(new byte[] { 1 }), Key);

		It should_complete_successfully = () => client.Verify();

		const string Key = "path";

		static void Setup()
		{
			InitializeComponents();
			ClientPutsObject();
		}

		private static void ClientPutsObject()
		{
			client
				.Setup(x => x.PutObject(Moq.It.Is<PutObjectRequest>(y => y.Key == Key)))
				.Verifiable();
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169