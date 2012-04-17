#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.AmazonS3.Tests
{
	using System;
	using System.Collections.Generic;
	using Amazon.S3.Model;
	using Machine.Specifications;
	using It = Machine.Specifications.It;

	[Subject("Amazon Enumerate Request")]
	public class when_enumerating_objects_in_an_amazon_S3_location : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => actual = storage.EnumerateObjects("path");

		It should_list_any_objects_in_that_location = () => actual.ShouldEqual(expected);

		private static readonly IEnumerable<string> expected = new List<string> { "1", "2" };
		private static IEnumerable<string> actual;

		static void Setup()
		{
			InitializeComponents();
			client
				.Setup(x => x.ListObjects(Moq.It.IsAny<ListObjectsRequest>()))
				.Returns(expected);
		}
	}

	[Subject("Amazon Enumerate Request")]
	public class when_enumerating_objects_and_filtering_on_date_last_modified : using_amazon_storage
	{
		Establish context = Setup;

		Because of = () => actual = storage.EnumerateObjects("path", date => true);

		It should_list_any_objects_in_that_location = () => actual.ShouldEqual(expected);

		static void Setup()
		{
			InitializeComponents();
			client
				.Setup(x => x.ListObjects(Moq.It.IsAny<ListObjectsRequest>(), Moq.It.IsAny<Func<DateTime, bool>>()))
				.Returns(expected);
		}

		private static IEnumerable<string> actual;
		private static readonly IEnumerable<string> expected = new List<string> { "1", "2" };
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169