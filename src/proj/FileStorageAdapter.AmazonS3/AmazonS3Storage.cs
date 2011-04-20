namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;
	using Amazon;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Storage : IStoreFiles, IDisposable
	{
		private const string ForwardSlash = "/";
		private const string DllExtension = ".dll";
		private readonly AmazonS3 client;
		private readonly string bucketName;

		static AmazonS3Storage()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				var resourceName = new AssemblyName(args.Name) + DllExtension;
				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					var assemblyData = new byte[stream.Length];
					stream.Read(assemblyData, 0, assemblyData.Length);
					return Assembly.Load(assemblyData);
				}
			};
		}

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
				Key = RemotePath.Normalize(path),
				Timeout = int.MaxValue
			};

			return ExecuteAndThrowOnFailure(() => new DisposableS3ResponseStream(this.client.GetObject(request)));
		}

		public void Put(Stream input, string path)
		{
			var request = new PutObjectRequest
			{
				BucketName = this.bucketName,
				InputStream = input,
				Key = RemotePath.Normalize(path),
				GenerateMD5Digest = true,
				Timeout = int.MaxValue
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

		public IEnumerable<string> EnumerateObjects()
		{
			return this.EnumerateObjects(string.Empty);
		}

		public IEnumerable<string> EnumerateObjects(string location)
		{
			if (location.StartsWith(ForwardSlash))
				location = location.Substring(1);

			var request = new ListObjectsRequest
			{
				BucketName = this.bucketName,
				Prefix = location,
				Delimiter = ForwardSlash
			};

			return ExecuteAndThrowOnFailure(() =>
			{
				using (var response = this.client.ListObjects(request))
					return response.S3Objects.Select(s3Object => s3Object.Key);
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
				if (e.ErrorCode == AmazonS3ErrorCodes.NoSuchKey)
					throw new FileNotFoundException(e.Message, e);

				throw BuildException(e);
			}
			catch (WebException e)
			{
				throw new StorageUnavailableException(e.Message, e);	
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
				if (e.ErrorCode == AmazonS3ErrorCodes.NoSuchKey)
					throw new FileNotFoundException(e.Message, e);

				throw BuildException(e);
			}
			catch (WebException e)
			{
				throw new StorageUnavailableException(e.Message, e);	
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