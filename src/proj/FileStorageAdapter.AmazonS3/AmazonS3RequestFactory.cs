﻿namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text.RegularExpressions;
	using Amazon.S3.Model;

	public class AmazonS3RequestFactory
	{
		public virtual GetObjectRequest Get(string path)
		{
			return new GetObjectRequest()
				.WithBucketName(this.bucketName)
				.WithKey(Normalize(path))
				.WithTimeout(int.MaxValue);
		}
		public virtual PutObjectRequest Put(Stream input, string path)
		{
			return new PutObjectRequest
			{
				BucketName = this.bucketName,
				InputStream = input,
				Key = Normalize(path),	
				GenerateMD5Digest = true,
				Timeout = int.MaxValue
			};
		}
		public virtual ListObjectsRequest ListObjects(string location)
		{
			return new ListObjectsRequest
			{
				BucketName = this.bucketName,
				Prefix = Normalize(location),
				Delimiter = ForwardSlash.ToString(CultureInfo.InvariantCulture)
			};
		}
		public virtual DeleteObjectRequest Delete(string path)
		{
			return new DeleteObjectRequest()
				.WithBucketName(this.bucketName)
				.WithKey(Normalize(path));
		}
		public virtual CopyObjectRequest Copy(string source, string destination)
		{
			return new CopyObjectRequest()
				.WithSourceBucket(this.bucketName)
				.WithSourceKey(Normalize(source))
				.WithDestinationBucket(this.bucketName)
				.WithDestinationKey(Normalize(destination))
				.WithTimeout(int.MaxValue);
		}
		public virtual GetPreSignedUrlRequest PreSignedUrl(string path, DateTime expiration)
		{
			return new GetPreSignedUrlRequest()
				.WithKey(Normalize(path))
				.WithProtocol(Protocol.HTTPS)
				.WithBucketName(this.bucketName)
				.WithExpires(expiration)
				.WithVerb(HttpVerb.GET);
		}
		public virtual GetPreSignedUrlRequest PreSignedUrl(string path, string filename, DateTime expiration)
		{
			return this.PreSignedUrl(path, expiration)
				.WithResponseHeaderOverrides(ComposeResponseHeaderOverrides(filename));
		}
		private static ResponseHeaderOverrides ComposeResponseHeaderOverrides(string filename)
		{
			var cleanFilename = NonWordCharacters.Replace(filename, Hyphen);
			var contentDisposition = string.Format(RenameDownloadContentDisposition, cleanFilename);
			return new ResponseHeaderOverrides { ContentDisposition = contentDisposition };
		}

		private static string Normalize(string path)
		{
			return path.Replace(Backslash, ForwardSlash);
		}

		public AmazonS3RequestFactory(string bucketName)
		{
			this.bucketName = bucketName;
		}

		private const string Hyphen = "-";
		private const string RenameDownloadContentDisposition = "attachment;filename=\"{0}\"";
		private static readonly Regex NonWordCharacters = new Regex("[^.^\\w-]");
		private readonly string bucketName;
		private const char Backslash = '\\';
		private const char ForwardSlash = '/';
	}
}