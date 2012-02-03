#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System.IO;
	using System.Net;
	using Amazon.S3;
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Amazon Exists Requests")]
	public class when_checking_for_an_existing_file_on_Amazon_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => found = storage.Exists(Path);

		It should_indicate_that_the_file_exists = () => found.ShouldBeTrue();

		private const string KeyNotFound = AmazonS3ErrorCodes.NoSuchKey;
		const string Path = "Path";
		static bool found;

		private static void Setup()
		{
			InitializeComponents();
			ClientThrowsForNonExistentObjects();
			ClientStreamsExistingObject();
		}
		private static void ClientThrowsForNonExistentObjects()
		{
			client
				.Setup(x => x.GetObject(Moq.It.Is<GetObjectRequest>(y => y.Key != Path)))
				.Throws(new AmazonS3Exception(
					KeyNotFound, HttpStatusCode.NotFound, KeyNotFound, "", "", "", "", null));
		}
		private static void ClientStreamsExistingObject()
		{
			client
				.Setup(x => x.GetObject(Moq.It.Is<GetObjectRequest>(y => y.Key == Path)))
				.Returns(new MemoryStream());
		}
	}

	[Subject("Amazon Exists Requests")]
	public class when_checking_for_an_existing_file_on_Amazon_S3_using_its_download_link : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => found = storage.Exists(DownloadLink);

		It should_indicate_that_the_file_exists = () => found.ShouldBeTrue();

		private const string KeyNotFound = AmazonS3ErrorCodes.NoSuchKey;
		const string Path = "/Path";
		const string DownloadLink = "http://" + Bucket + ".s3.amazonaws.com/" + Path;
		static bool found;

		private static void Setup()
		{
			InitializeComponents();
			ClientThrowsForNonExistentObjects();
			ClientStreamsExistingObject();
		}
		private static void ClientThrowsForNonExistentObjects()
		{
			client
				.Setup(x => x.GetObject(Moq.It.Is<GetObjectRequest>(y => y.Key != Path)))
				.Throws(new AmazonS3Exception(
					KeyNotFound, HttpStatusCode.NotFound, KeyNotFound, "", "", "", "", null));
		}
		private static void ClientStreamsExistingObject()
		{
			client
				.Setup(x => x.GetObject(Moq.It.Is<GetObjectRequest>(y => y.Key == Path)))
				.Returns(new MemoryStream());
		}
	}

	[Subject("Amazon Exists Requests")]
	public class when_checking_for_a_non_existent_file_on_Amazon_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => found = storage.Exists("Not-there");

		It should_indicate_that_the_file_does_not_exist = () => found.ShouldBeFalse();

		private const string KeyNotFound = AmazonS3ErrorCodes.NoSuchKey;
		static bool found;

		private static void Setup()
		{
			InitializeComponents();
			client.Setup(x => x.GetObject(Moq.It.IsAny<GetObjectRequest>()))
				.Throws(new AmazonS3Exception(
					KeyNotFound, HttpStatusCode.NotFound, KeyNotFound, "", "", "", "", null));
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169