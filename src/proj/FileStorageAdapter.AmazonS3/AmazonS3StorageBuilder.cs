namespace FileStorageAdapter.AmazonS3
{
	using System;
	using Amazon;

	public class AmazonS3StorageBuilder
	{
		public static AmazonS3StorageBuilder UsingCredentials(string accessKey, string secretAccessKey)
		{
			return new AmazonS3StorageBuilder
			{
				awsAccessKey = accessKey,
				awsSecretAccessKey = secretAccessKey
			};
		}

		public AmazonS3StorageBuilder InBucket(string bucket)
		{
			this.bucketname = bucket;
			return this;
		}

		public AmazonS3StorageBuilder UsingSpecifiedUrlValidityPeriod(TimeSpan duration)
		{
			this.urlValidity = duration;
			return this;
		}

		public AmazonS3Storage Build()
		{
			if (string.IsNullOrEmpty(this.awsAccessKey) || string.IsNullOrEmpty(this.awsSecretAccessKey) || string.IsNullOrEmpty(bucketname))
				throw new FileStorageException("You must provide the following: AWS Access Key, AWS Secret Access Key, and the name of an S3 Bucket.");

			return new AmazonS3Storage(
				new AmazonS3Client(AWSClientFactory.CreateAmazonS3Client(this.awsAccessKey, this.awsSecretAccessKey)),
				new AmazonS3RequestFactory(this.bucketname),
				new FileSystem(), 
				this.bucketname) { UrlValidity = this.urlValidity };
		}

		private AmazonS3StorageBuilder()
		{
			this.urlValidity = TimeSpan.FromSeconds(30);
		}

		private string awsAccessKey;
		private string awsSecretAccessKey;
		private string bucketname;
		private TimeSpan urlValidity;
	}
}