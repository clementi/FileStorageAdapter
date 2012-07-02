namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Client : IDisposable
	{
		public virtual string GetPreSignedUrl(GetPreSignedUrlRequest request)
		{
			return this.client.GetPreSignedURL(request);
		}
		public virtual IEnumerable<string> ListObjects(ListObjectsRequest request)
		{
			return this.ListAllObjects(request).Select(x => x.Key);
		}

		public virtual IEnumerable<string> ListObjects(ListObjectsRequest request, Func<DateTime, bool> lastModifiedFilter)
		{
			return this.ListAllObjects(request)
				.Where(x => lastModifiedFilter(DateTime.Parse(x.Value)))
				.Select(x => x.Key);
		}
		private IEnumerable<KeyValuePair<string, string>> ListAllObjects(ListObjectsRequest request)
		{
			while (true)
			{
				var response = this.client.ListObjects(request);

				foreach (var item in response.S3Objects)
					yield return new KeyValuePair<string, string>(Normalize(item.Key), item.LastModified);

				if (!response.IsTruncated)
					break;

				request.Marker = response.NextMarker;
			}
		}
		private static string Normalize(string value)
		{
			return "/" + value;
		}

		public virtual GetObjectMetadataResponse GetObjectMetadata(GetObjectMetadataRequest request)
		{
			return this.client.GetObjectMetadata(request);
		}
		public virtual GetObjectResponse GetObject(GetObjectRequest request)
		{
			return this.client.GetObject(request);
		}
		public virtual bool PutObject(PutObjectRequest request)
		{
			using (this.client.PutObject(request))
				return true;
		}
		public virtual bool DeleteObject(DeleteObjectRequest request)
		{
			using (this.client.DeleteObject(request))
				return true;
		}
		public virtual bool CopyObject(CopyObjectRequest request)
		{
			using (this.client.CopyObject(request))
				return true;
		}

		public AmazonS3Client() { }
		public AmazonS3Client(AmazonS3 client)
		{
			this.client = client;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.client != null)
				this.client.Dispose();
		}
	
		private readonly AmazonS3 client;
	}
}