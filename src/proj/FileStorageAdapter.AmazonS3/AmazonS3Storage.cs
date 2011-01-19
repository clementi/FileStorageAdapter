namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Storage : IStoreFiles
	{
		private const string ErrorMessageFormat = "Unable to store files: {0}";
		private const string ForwardSlash = "/";
		private readonly AmazonS3 client;
		private readonly string bucketName;

		public AmazonS3Storage(AmazonS3 client, string bucketName)
		{
			this.client = client;
			this.bucketName = bucketName;
		}

		public Stream Get(string path)
		{
			var request = new GetObjectRequest
			{
				BucketName = this.bucketName,
				Key = path
			};

			return ExecuteAndThrowOnFailure(() =>
			{
				using (var response = this.client.GetObject(request))
					return response.ResponseStream;
			});
		}

		public void Put(Stream input, string location)
		{
			var request = new PutObjectRequest
			{
				BucketName = this.bucketName,
				InputStream = input,
				Key = location
			};

			ExecuteAndThrowOnFailure(() =>
			{
				using (this.client.PutObject(request))
				{
				}
			});
		}

		public void Delete(string path)
		{
			var request = new DeleteObjectRequest
			{
				BucketName = this.bucketName,
				Key = path
			};

			ExecuteAndThrowOnFailure(() =>
			{
				using (this.client.DeleteObject(request))
				{
				}
			});
		}

		public bool Exists(string path)
		{
			path = RemoveInitialForwardSlash(path);

			return this.EnumerateObjects(path).Contains(path);
		}

		public IEnumerable<string> EnumerateObjects(string location)
		{
			location = RemoveInitialForwardSlash(location);

			var request = new ListObjectsRequest
			{
				BucketName = this.bucketName,
				Prefix = location
			};

			var objects = ExecuteAndThrowOnFailure(() => this.client.ListObjects(request).S3Objects);
			return objects.Select(x => x.Key);
		}

		private static string RemoveInitialForwardSlash(string location)
		{
			if (location.StartsWith(ForwardSlash))
				location = location.Substring(1);
			return location;
		}

		private static void ExecuteAndThrowOnFailure(Action action)
		{
			try
			{
				action();
			}
			catch (AmazonS3Exception e)
			{
				throw BuildException(e);
			}
		}

		private static T ExecuteAndThrowOnFailure<T>(Func<T> func)
		{
			try
			{
				return func();
			}
			catch (AmazonS3Exception e)
			{
				throw BuildException(e);
			}
		}

		private static FileStorageException BuildException(Exception e)
		{
			return new FileStorageException(string.Format(ErrorMessageFormat, e.Message), e);
		}
	}
}