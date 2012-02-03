#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using Machine.Specifications;
	using Moq;

	public abstract class using_amazon_storage
	{
		protected const string Bucket = "bucket";
		protected static Mock<AmazonS3Client> client;
		protected static Mock<IFileSystem> fileSystem;
		protected static AmazonS3RequestFactory factory;
		protected static AmazonS3Storage storage;

		Cleanup after = ResetComponents;

		protected static void InitializeComponents()
		{
			client = new Mock<AmazonS3Client>();
			fileSystem = new Mock<IFileSystem>();
			factory = new AmazonS3RequestFactory(Bucket);
			storage = new AmazonS3Storage(client.Object, factory, fileSystem.Object, Bucket);
		}

		protected static void ResetComponents()
		{
			client = null;
			fileSystem = null;
			factory = null;
			storage = null;
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169