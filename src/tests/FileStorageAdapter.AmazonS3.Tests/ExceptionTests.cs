#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System;
	using System.IO;
	using System.Net;
	using Amazon.S3;
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Amazon requests could throw exceptions")]
	public class when_a_given_key_is_not_found : when_throwing_exceptions
	{
		Establish context = Setup;

		Because of = TryGet;

		It should_throw_an_appropriate_exception = () => fileNotFound.ShouldNotBeNull();
		It should_show_the_correct_error_message = () => fileNotFound.Message.ShouldEqual(KeyNotFound);
		It should_retain_the_original_exception = () => fileNotFound.InnerException.ShouldBe(typeof(AmazonS3Exception));

		private const string KeyNotFound = AmazonS3ErrorCodes.NoSuchKey;

		private static void Setup()
		{
			InitializeComponents();
			ClientThrowsException();
		}

		private static void ClientThrowsException()
		{
			client
				.Setup(x => x.GetObject(Moq.It.IsAny<GetObjectRequest>()))
				.Throws(new AmazonS3Exception(
					KeyNotFound, HttpStatusCode.NotFound, KeyNotFound, "", "", "", "", null));
		}
	}

	[Subject("Amazon requests could throw exceptions")]
	public class when_an_unknown_Amazon_S3_error_is_thrown : when_throwing_exceptions
	{
		Establish context = Setup;

		Because of = TryGet;

		It should_throw_a_general_exception = () => generalException.ShouldNotBeNull();
		It should_show_the_correct_error_message = () => generalException.Message.ShouldContain(UnknownError);
		It should_retain_the_original_exception = () => generalException.InnerException.ShouldBe(typeof(AmazonS3Exception));

		private const string UnknownError = "Unknown Error";

		private static void Setup()
		{
			InitializeComponents();
			ClientThrowsException();
		}

		private static void ClientThrowsException()
		{
			client
				.Setup(x => x.GetObject(Moq.It.IsAny<GetObjectRequest>()))
				.Throws(new AmazonS3Exception(
					UnknownError, HttpStatusCode.NotFound, UnknownError, "", "", "", "", null));
		}
	}

	[Subject("Amazon requests could throw exceptions")]
	public class when_amazon_storage_is_offline : when_throwing_exceptions
	{
		Establish context = Setup;

		Because of = TryGet;

		It should_throw_an_appropriate_exception = () => storageUnavailable.ShouldNotBeNull();
		It should_show_the_correct_error_message = () => storageUnavailable.Message.ShouldContain(ErrorMessage);
		It should_retain_the_original_exception = () => storageUnavailable.InnerException.ShouldBe(typeof(WebException));

		private const string ErrorMessage = "Message";

		private static void Setup()
		{
			InitializeComponents();
			ClientThrowsException();
		}

		private static void ClientThrowsException()
		{
			client
				.Setup(x => x.GetObject(Moq.It.IsAny<GetObjectRequest>()))
				.Throws(new WebException(ErrorMessage));
		}
	}

	[Subject("Amazon requests could throw exceptions")]
	public class when_amazon_storage_throws_an_unexpected_exception : when_throwing_exceptions
	{
		Establish context = Setup;

		Because of = TryGet;

		It should_throw_an_appropriate_exception = () => generalException.ShouldNotBeNull();
		It should_show_the_correct_error_message = () => generalException.Message.ShouldContain(ErrorMessage);
		It should_retain_the_original_exception = () => generalException.InnerException.ShouldBe(typeof(InvalidOperationException));

		private const string ErrorMessage = "Message";

		private static void Setup()
		{
			InitializeComponents();
			ClientThrowsException();
		}

		private static void ClientThrowsException()
		{
			client
				.Setup(x => x.GetObject(Moq.It.IsAny<GetObjectRequest>()))
				.Throws(new InvalidOperationException(ErrorMessage));
		}
	}

	public abstract class when_throwing_exceptions : using_amazon_storage
	{
		protected static FileNotFoundException fileNotFound;
		protected static StorageUnavailableException storageUnavailable;
		protected static FileStorageException generalException;

		protected static void TryGet()
		{
			try
			{
				storage.Get("not-found");
			}
			catch (FileNotFoundException e)
			{
				fileNotFound = e;
			}
			catch (StorageUnavailableException e)
			{
				storageUnavailable = e;
			}
			catch (FileStorageException e)
			{
				generalException = e;
			}
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169