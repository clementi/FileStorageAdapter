namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using Amazon;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Storage : IStoreFiles, IDisposable
	{
		private const string ForwardSlash = "/";
		private readonly AmazonS3 client;
		private readonly string bucketName;
		private const string DownloadUrlTemplate = "{0}.s3.amazonaws.com";
		private static readonly TimeSpan DefaultUrlValidity = TimeSpan.FromSeconds(30);
		
		public TimeSpan UrlValidity { get; set; }

		public AmazonS3Storage(string awsAccessKey, string awsSecretAccessKey, string bucketName)
		{
			this.client = AWSClientFactory.CreateAmazonS3Client(awsAccessKey, awsSecretAccessKey);
			this.bucketName = bucketName;
			this.UrlValidity = DefaultUrlValidity;
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

		public string GetDownloadUrl(string path)
		{
			var request = new GetPreSignedUrlRequest()
				.WithKey(path)
				.WithProtocol(Protocol.HTTPS)
				.WithBucketName(this.bucketName)
				.WithExpires(DateTime.UtcNow.Add(this.UrlValidity))
				.WithVerb(HttpVerb.GET);

			return this.client.GetPreSignedURL(request);
		}
		public void Download(string remotePath, string localPath)
		{
			using (var remote = this.Get(remotePath))
			using (var local = File.OpenWrite(localPath))
				remote.CopyTo(local);
		}
		public void Upload(string localPath, string remotePath)
		{
			using (var local = File.OpenRead(localPath))
				this.Put(local, remotePath);
		}

		public virtual bool Exists(string pathOrLocation)
		{
			if (IsDownloadUrl(pathOrLocation))
				pathOrLocation = ExtractPathFromUrl(pathOrLocation);

			try
			{
				using (this.Get(pathOrLocation))
					return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}
		private bool IsDownloadUrl(string pathOrLocation)
		{
			return pathOrLocation.ToLower().Contains(string.Format(DownloadUrlTemplate, this.bucketName));
		}
		private static string ExtractPathFromUrl(string pathOrLocation)
		{
			pathOrLocation = new Uri(pathOrLocation).AbsolutePath;
			if (pathOrLocation.StartsWith("//"))
				pathOrLocation = pathOrLocation.Substring(1);

			return pathOrLocation;
		}
		public virtual void Rename(string source, string destination)
		{
			var copyRequest = new CopyObjectRequest()
				.WithSourceBucket(this.bucketName)
				.WithSourceKey(source)
				.WithDestinationBucket(this.bucketName)
				.WithDestinationKey(destination)
				.WithTimeout(int.MaxValue);

			var deleteRequest = new DeleteObjectRequest()
				.WithBucketName(this.bucketName)
				.WithKey(source);

			ExecuteAndThrowOnFailure(() =>
			{
				using (this.client.CopyObject(copyRequest))
				using (this.client.DeleteObject(deleteRequest))
					return 0;
			});
		}
		public virtual void Delete(string path)
		{
			var request = new DeleteObjectRequest
			{
				BucketName = this.bucketName,
				Key = RemotePath.Normalize(path)
			};

			ExecuteAndThrowOnFailure(() =>
			{
				using (this.client.DeleteObject(request))
					return 0;
			});
		}
		
		public virtual IEnumerable<string> EnumerateObjects(string location)
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

		public virtual Stream Get(string path)
		{
			var request = new GetObjectRequest()
				.WithBucketName(this.bucketName)
				.WithKey(RemotePath.Normalize(path))
				.WithTimeout(int.MaxValue);

			return ExecuteAndThrowOnFailure(() => new DisposableS3ResponseStream(this.client.GetObject(request)));
		}
		public virtual void Put(Stream input, string path)
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
					return 0;
			});
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
			catch (Exception e)
			{
				throw BuildException(e);
			}
		}
		private static FileStorageException BuildException(Exception e)
		{
			return new FileStorageException(e.Message, e);
		}
	}
}