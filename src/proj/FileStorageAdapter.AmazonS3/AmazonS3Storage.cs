namespace FileStorageAdapter.AmazonS3
{
	using System.IO;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3Storage : IStoreFiles
	{
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

			using (var response = this.client.GetObject(request))
				return response.ResponseStream;
		}

		public void Put(Stream input, string location)
		{
			var request = new PutObjectRequest
			{
				BucketName = this.bucketName,
				InputStream = input,
				Key = location
			};

			using (this.client.PutObject(request))
			{
			}
		}

		public void Delete(string path)
		{
			var request = new DeleteObjectRequest
			{
				BucketName = this.bucketName,
				Key = path
			};

			using (this.client.DeleteObject(request))
			{
			}
		}
	}
}