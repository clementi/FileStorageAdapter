namespace FileStorageAdapter.AmazonS3
{
	using System;
	using Amazon;
	using Amazon.S3;
	using Amazon.S3.Model;

	public class AmazonS3StorageBuilder
	{
		public AmazonS3StorageBuilder UsingCredentials(string accessKey, string secretAccessKey)
		{
			return new AmazonS3StorageBuilder
			{
				awsAccessKey = accessKey,
				awsSecretAccessKey = secretAccessKey
			};
		}
		public AmazonS3StorageBuilder WithMaxAttempts(int attempts)
		{
			this.retryAttempts = attempts;
			return this;
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
			if (string.IsNullOrEmpty(this.awsAccessKey) || string.IsNullOrEmpty(this.awsSecretAccessKey) || string.IsNullOrEmpty(this.bucketname))
				throw new FileStorageException("You must provide the following: AWS Access Key, AWS Secret Access Key, and the name of an S3 Bucket.");

			var config = new AmazonS3Config()
				.WithMaxErrorRetry(this.retryAttempts)
				.WithCommunicationProtocol(Protocol.HTTPS)
				.WithUseSecureStringForAwsSecretKey(true);
				
			return new AmazonS3Storage(
				new AmazonS3Client(AWSClientFactory.CreateAmazonS3Client(this.awsAccessKey, this.awsSecretAccessKey, config)),
				new AmazonS3RequestFactory(this.bucketname),
				new FileSystem(), 
				this.bucketname)
			{
				UrlValidity = this.urlValidity
			};
		}

		private TimeSpan urlValidity = TimeSpan.FromSeconds(30);
		private int retryAttempts = 3;
		private string awsAccessKey;
		private string awsSecretAccessKey;
		private string bucketname;
	}
}