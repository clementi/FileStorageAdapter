﻿namespace FileStorageAdapter.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.IO;
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
			return this.client.ListObjects(request).S3Objects.Select(x => x.Key);
		}
		public virtual Stream GetObject(GetObjectRequest request)
		{
			return this.client.GetObject(request).ResponseStream;
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
			if (disposing)
				this.client.Dispose();
		}
	
		private readonly AmazonS3 client;
	}
}