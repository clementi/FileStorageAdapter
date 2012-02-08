#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System;
	using System.Web;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject("Get Download URL")]
	public class when_requesting_a_download_url_of_a_file_stored_on_amazon_S3
	{
		Because of = () => url = new Uri(Storage.GetDownloadUrl("Object"));

		It should_use_https = () => url.Scheme.ShouldEqual("https");
		
		It should_reference_the_correct_bucket = () => 
			url.Host.ShouldStartWith(Bucket);
		
		It should_produce_a_url_that_shows_provided_access_key = () => 
			HttpUtility.ParseQueryString(url.Query)["AWSAccessKeyId"].ShouldEqual(AccessKey);

		It should_produce_a_url_that_is_signed = () =>
			HttpUtility.ParseQueryString(url.Query)["Signature"].ShouldNotBeEmpty();

		It should_produce_a_url_that_expires_after_the_provided_url_validity_period = () =>
		{
			var expirationStamp = HttpUtility.ParseQueryString(url.Query)["Expires"];
			var epoch = new DateTime(1970, 1, 1);
			var expiration = epoch.AddSeconds(int.Parse(expirationStamp));
			expiration.ShouldBeCloseTo(DateTime.UtcNow.AddSeconds(Storage.UrlValidity.Seconds), TimeSpan.FromSeconds(1));
		};

		private const string Bucket = "bucket";
		private const string AccessKey = "asdf";
		private static readonly AmazonS3Storage Storage = AmazonS3StorageBuilder
			.UsingCredentials(AccessKey, "qwer")
			.InBucket(Bucket)
			.Build();
		private static Uri url;
	}

	[Subject("Get Download URL")]
	public class when_requesting_a_download_url_using_an_alternate_name
	{
		Because of = () => url = new Uri(Storage.GetDownloadUrl("Object", GivenName));

		It should_reference_the_correct_bucket = () =>
			url.Host.ShouldStartWith(Bucket);

		It should_produce_a_url_that_shows_provided_access_key = () =>
			HttpUtility.ParseQueryString(url.Query)["AWSAccessKeyId"].ShouldEqual(AccessKey);

		It should_produce_a_url_that_is_signed = () =>
			HttpUtility.ParseQueryString(url.Query)["Signature"].ShouldNotBeEmpty();

		It should_produce_a_url_that_expires_after_the_provided_url_validity_period = () =>
		{
			var expirationStamp = HttpUtility.ParseQueryString(url.Query)["Expires"];
			var epoch = new DateTime(1970, 1, 1);
			var expiration = epoch.AddSeconds(int.Parse(expirationStamp));
			expiration.ShouldBeCloseTo(DateTime.UtcNow.AddSeconds(Storage.UrlValidity.Seconds), TimeSpan.FromSeconds(1));
		};

		It should_append_content_disposition_header_overrides = () =>
		{
			var query = HttpUtility.ParseQueryString(url.Query);
			var expected = string.Format("attachment;filename=\"{0}\"", GivenName);
			query["response-content-disposition"].ShouldEqual(expected);
		};

		private const string Bucket = "bucket";
		private const string AccessKey = "asdf";
		private const string GivenName = "Filename.zip";
		private static readonly AmazonS3Storage Storage = AmazonS3StorageBuilder
			.UsingCredentials(AccessKey, "qwer")
			.InBucket(Bucket)
			.Build();
		private static Uri url;
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169