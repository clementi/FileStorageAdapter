namespace FileStorageAdapter.AmazonS3
{
	using Amazon;

	public class AmazonS3StorageBuilder
	{
		public static AmazonS3Storage Build(string awsAccessKey, string awsSecretAccessKey, string bucketName)
		{
			return new AmazonS3Storage(
		 		new AmazonS3Client(AWSClientFactory.CreateAmazonS3Client(awsAccessKey, awsSecretAccessKey)),
		 		new AmazonS3RequestFactory(bucketName),
		 		new FileSystem(),
		 		bucketName);
		}
	}
}