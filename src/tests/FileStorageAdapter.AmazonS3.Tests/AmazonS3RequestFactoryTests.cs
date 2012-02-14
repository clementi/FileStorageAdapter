#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System;
	using System.IO;
	using Amazon.S3.Model;
	using Machine.Specifications;

	[Subject("Generating Amazon S3 requests")]
	public class when_generating_a_get_request
	{
		const string Bucket = "Bucket";
		const string Key = "path";
		static AmazonS3RequestFactory factory;
		static GetObjectRequest request;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);
		
		Because of = () => request = factory.Get(Key);

		It should_use_the_maximum_timeout = () => request.Timeout.ShouldEqual(int.MaxValue);
		It should_use_the_given_key_name = () => request.Key.ShouldEqual(Key);
		It should_use_the_give_bucket_name = () => request.BucketName.ShouldEqual(Bucket);
	}

	[Subject("Generating Amazon S3 Put requests")]
	public class when_generating_a_put_request
	{
		const string Bucket = "Bucket";
		const string Key = "key";
		static MemoryStream input;
		static AmazonS3RequestFactory factory;
		static PutObjectRequest request;

		Establish context = () =>
		{
			factory = new AmazonS3RequestFactory(Bucket);
			input = new MemoryStream();
		};

		Because of = () => request = factory.Put(input, Key);

		It should_use_the_maximum_timout = () => request.Timeout.ShouldEqual(int.MaxValue);
		It should_use_the_given_key = () => request.Key.ShouldEqual(Key);
		It should_use_the_give_bucket_name = () => request.BucketName.ShouldEqual(Bucket);
		It should_stream_from_the_provided_input = () => request.InputStream.ShouldEqual(input);
		It should_generate_an_MD5_digest = () => request.GenerateMD5Digest.ShouldBeTrue();
	}

	[Subject("Generating Amazon S3 Delete requests")]
	public class when_generating_a_delete_request
	{
		const string Bucket = "Bucket";
		const string Key = "path";
		static AmazonS3RequestFactory factory;
		static DeleteObjectRequest request;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);

		Because of = () => request = factory.Delete(Key);

		It should_use_the_given_bucket_name = () => request.BucketName.ShouldEqual(Bucket);
		It should_use_the_given_path = () => request.Key.ShouldEqual(Key);
	}

	[Subject("Generating Amazon S3 ListObjects requests")]
	public class when_generating_a_list_objects_request
	{
		const string Bucket = "Bucket";
		const string Location = "path";
		const string ForwardSlash = "/";
		static AmazonS3RequestFactory factory;
		static ListObjectsRequest request;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);

		Because of = () => request = factory.ListObjects(Location);

		It should_use_the_given_bucket_name = () => request.BucketName.ShouldEqual(Bucket);
		It should_use_the_correct_delimiter = () => request.Delimiter.ShouldEqual(ForwardSlash);
		It should_user_the_given_location = () => request.Prefix.ShouldEqual(Location);
	}

	[Subject("Generating Amazon S3 Copy requests")]
	public class when_generating_a_copy_request
	{
		const string Bucket = "Bucket";
		const string Source = "source";
		const string Destination = "destination";
		static AmazonS3RequestFactory factory;
		static CopyObjectRequest request;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);

		Because of = () => request = factory.Copy(Source, Destination);

		It should_use_the_given_bucket_name_for_the_source = () => 
			request.DestinationBucket.ShouldEqual(Bucket);
		
		It should_use_the_given_bucket_name_for_the_destination = () =>
			request.SourceBucket.ShouldEqual(Bucket);

		It should_use_the_given_source = () => request.SourceKey.ShouldEqual(Source);
		It should_use_the_given_destination = () => request.DestinationKey.ShouldEqual(Destination);
		It should_use_the_maximum_timeout = () => request.Timeout.ShouldEqual(int.MaxValue);
	}

	[Subject("Generating Amazon S3 PreSignedUrl requests")]
	public class when_generating_a_PreSignedUrl_request
	{
		const string Bucket = "Bucket";
		const string Key = "path";
		const string ForwardSlash = "/";
		static AmazonS3RequestFactory factory;
		static GetPreSignedUrlRequest request;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);

		Because of = () => request = factory.PreSignedUrl(Key, DateTime.MaxValue);

		It should_use_the_given_bucket_name = () => request.BucketName.ShouldEqual(Bucket);
		It should_use_the_given_key = () => request.Key.ShouldEqual(Key);
		It should_use_HTTPS = () => request.Protocol.ShouldEqual(Protocol.HTTPS);
		It should_use_GET = () => request.Verb.ShouldEqual(HttpVerb.GET);
		It should_make_the_request_good_for_the_given_validity = () => 
			request.Expires.ShouldEqual(DateTime.MaxValue);
	}

	[Subject("Generating Amazon S3 PreSignedUrl requests")]
	public class when_generating_a_PreSignedUrl_request_with_a_modified_filename
	{
		const string Bucket = "Bucket";
		const string Key = "path";
		const string ForwardSlash = "/";
		static AmazonS3RequestFactory factory;
		static GetPreSignedUrlRequest request;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);

		Because of = () => request = factory.PreSignedUrl(Key, "this+is=a#different%name$asdf.zip", DateTime.MaxValue);

		It should_override_the_content_disposition = () =>
			request.ResponseHeaderOverrides.ContentDisposition.ShouldNotBeNull();

		It should_indicate_an_attachment = () => 
			request.ResponseHeaderOverrides.ContentDisposition.ShouldContain("attachment;");

		It should_sanitize_the_alternate_name_before_adding_it_the_content_disposition_header = () =>
			request.ResponseHeaderOverrides.ContentDisposition.ShouldContain("this-is-a-different-name-asdf.zip");
	}

	[Subject("Generating Amazon S3 Get requests")]
	public class when_the_path_delimiters_are_not_consistent
	{
		const string Bucket = "Bucket";
		const string Key = "path/to\\file";
		const string Normalized = "path/to/file";
		static AmazonS3RequestFactory factory;
		static GetObjectRequest get;
		static PutObjectRequest put;
		static ListObjectsRequest list;
		static DeleteObjectRequest delete;
		static CopyObjectRequest copy;
		static GetPreSignedUrlRequest url;

		Establish context = () => factory = new AmazonS3RequestFactory(Bucket);

		Because of = () =>
		{
			get = factory.Get(Key);
			put = factory.Put(null, Key);
			list = factory.ListObjects(Key);
			delete = factory.Delete(Key);
			copy = factory.Copy(Key, Key);
			url = factory.PreSignedUrl(Key, DateTime.MinValue);
		};

		It should_normalize_the_get_path = () => get.Key.ShouldEqual(Normalized);
		It should_normalize_the_put_path = () => put.Key.ShouldEqual(Normalized);
		It should_normalize_the_list_location = () => list.Prefix.ShouldEqual(Normalized);
		It should_normalize_the_delete_path = () => delete.Key.ShouldEqual(Normalized);
		It should_normalize_the_copy_source_path = () => copy.SourceKey.ShouldEqual(Normalized);
		It should_normalize_the_copy_destination_path = () => copy.DestinationKey.ShouldEqual(Normalized);
		It should_normalize_the_url_path = () => url.Key.ShouldEqual(Normalized);
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169