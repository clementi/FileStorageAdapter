namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using Amazon.S3;

	public class AmazonS3Storage : IStoreFiles, IDisposable
	{
		public string GetDownloadUrl(string path)
		{
			var request = this.factory.PreSignedUrl(path, DateTime.UtcNow + this.UrlValidity);
			return this.client.GetPreSignedUrl(request);
		}
		public string GetDownloadUrl(string path, string fileName)
		{
			var request = this.factory.PreSignedUrl(path, fileName, DateTime.UtcNow + this.UrlValidity);
			return this.client.GetPreSignedUrl(request);
		}
		public void Download(string remotePath, string localPath)
		{
			using (var remote = this.Get(remotePath))
			using (var local = this.fileSystem.OpenWrite(localPath))
				remote.CopyTo(local);
		}
		public void Upload(string localPath, string remotePath)
		{
			using (var local = this.fileSystem.OpenRead(localPath))
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
		
		public void Rename(string source, string destination)
		{
			ExecuteAndThrowOnFailure(() =>
			{
				this.client.CopyObject(this.factory.Copy(source, destination));
				this.client.DeleteObject(this.factory.Delete(source));
				return true;
			});
		}
		public void Delete(string path)
		{
			ExecuteAndThrowOnFailure(() => this.client.DeleteObject(this.factory.Delete(path)));
		}
		public IEnumerable<string> EnumerateObjects(string location)
		{
			return ExecuteAndThrowOnFailure(() =>
				this.client.ListObjects(this.factory.ListObjects(location)));
		}
		public Stream Get(string path)
		{
			return ExecuteAndThrowOnFailure(() => this.client.GetObject(this.factory.Get(path)));
		}
		public void Put(Stream input, string path)
		{
			ExecuteAndThrowOnFailure(() => this.client.PutObject(this.factory.Put(input, path)));
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

				throw new FileStorageException(e.Message, e);
			}
			catch (WebException e)
			{
				throw new StorageUnavailableException(e.Message, e);
			}
			catch (Exception e)
			{
				throw new FileStorageException(e.Message, e);
			}
		}

		public TimeSpan UrlValidity { get; set; }

		public AmazonS3Storage(
			AmazonS3Client client, AmazonS3RequestFactory factory, IFileSystem fileSystem, string bucketName)
		{
			this.client = client;
			this.factory = factory;
			this.fileSystem = fileSystem;
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

		private const string DownloadUrlTemplate = "{0}.s3.amazonaws.com";
		private static readonly TimeSpan DefaultUrlValidity = TimeSpan.FromSeconds(30);
		private readonly AmazonS3Client client;
		private readonly AmazonS3RequestFactory factory;
		private readonly IFileSystem fileSystem;
		private readonly string bucketName;
	}
}