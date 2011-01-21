namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.IO;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Storage : IStoreFiles
	{
		private const string ErrorMessageFormat = "Unable to store files: {0}";
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
				Key = RemotePath.Normalize(path)
			};

			return ExecuteAndThrowOnFailure(() =>
			{
				using (var response = this.client.GetObject(request))
					return response.ResponseStream;
			});
		}

		public void Put(Stream input, string path)
		{
			var request = new PutObjectRequest
			{
				BucketName = this.bucketName,
				InputStream = input,
				Key = RemotePath.Normalize(path)
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
				Key = RemotePath.Normalize(path)
			};

			ExecuteAndThrowOnFailure(() =>
			{
				using (this.client.DeleteObject(request))
				{
				}
			});
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