#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System.IO;
	using System.Text;
	using Amazon.S3.Model;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject("Amazon Get Request")]
	public class when_getting_an_object_from_S3 : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => result = storage.Get(Key);

		It should_wrap_the_response_in_order_to_avoid_gargabe_collection_of_the_response_stream = () => 
			result.ShouldBeOfType(typeof(DisposableS3ResponseStream));
		It should_receive_the_correct_contents = EnsureCorrectContents;

		const string Key = "Key";
		const string RemoteContents = "asdf";
		static Stream result;

		private static void Setup()
		{
			InitializeComponents();
			SetupComponents();
		}

		private static void SetupComponents()
		{
			client
				.Setup(x => x.GetObject(Moq.It.Is<GetObjectRequest>(y => y.Key == Key)))
				.Returns(new GetObjectResponse { ResponseStream = new MemoryStream(new UTF8Encoding().GetBytes(RemoteContents)) });
		}

		private static void EnsureCorrectContents()
		{
			using (var reader = new StreamReader(result))
				reader.ReadToEnd().ShouldEqual(RemoteContents);
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169