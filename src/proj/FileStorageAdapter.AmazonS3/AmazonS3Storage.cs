namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.IO;
	using Amazon;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Storage : IStoreFiles, IDisposable
	{
		private readonly AmazonS3 client;
		private readonly string bucketName;

		public AmazonS3Storage(string awsAccessKey, string awsSecretAccessKey, string bucketName)
		{
			this.client = AWSClientFactory.CreateAmazonS3Client(awsAccessKey, awsSecretAccessKey);
			this.bucketName = bucketName;
		}

		public Stream Get(string path)
		{
			var request = new GetObjectRequest
			{
				BucketName = this.bucketName,
				Key = RemotePath.Normalize(path)
			};

			return ExecuteAndThrowOnFailure(() => new DisposableS3ResponseStream(this.client.GetObject(request)));
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
			return new FileStorageException(e.Message, e);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				this.client.Dispose();
		}
	}
}