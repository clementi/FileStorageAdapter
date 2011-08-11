#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.LocalFileSystem.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Machine.Specifications;

	[Subject(typeof(LocalFileStorage))]
	public class when_getting_a_file_in_a_nonexistent_directory : using_the_local_file_storage_adapter
	{
		Because of = () =>
			exception = Catch.Exception(() => Storage.Get("C:/Does/Not/Exist.txt"));

		It should_throw_a_file_not_found_exception = () => 
			exception.ShouldBeOfType<FileNotFoundException>();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_getting_a_nonexistent_file : using_the_local_file_storage_adapter
	{
		Because of = () =>
			exception = Catch.Exception(() => Storage.Get("C:/Does_Not_Exist.txt"));

		It should_throw_an_exception = () =>
			exception.ShouldBeOfType<FileNotFoundException>();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_getting_an_existing_file : using_the_local_file_storage_adapter
	{
		static Stream stream;

		Establish context = () =>
			File.AppendAllText(Path.Combine(TempPath, First), Contents);

		Because of = () =>
			stream = Storage.Get(First);

		It should_open_the_file_for_reading = () =>
		{
			stream.ShouldNotBeNull();
			stream.CanRead.ShouldBeTrue();
			stream.Length.ShouldEqual(Contents.Length);
			stream.Dispose();
		};
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_putting_a_file : using_the_local_file_storage_adapter
	{
		private static readonly string Local = First;
		private static readonly string Remote = Path.Combine(TempPath, Second);

		Establish context = () =>
		{
			File.Delete(Local);
			File.AppendAllText(Local, Contents);
		};

		Because of = () =>
		{
			using (var first = File.OpenRead(Local))
				Storage.Put(first, Remote);
		};

		It should_deposit_the_entire_file_at_the_specified_location = () =>
		{
			File.Exists(Remote).ShouldBeTrue();
			File.ReadAllText(Remote).ShouldEqual(Contents);
		};
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_putting_a_file_that_was_stored_previously : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			File.AppendAllText(Path.Combine(TempPath, First), Contents + "New");
			File.AppendAllText(Path.Combine(TempPath, Second), Contents);
		};

		Because of = () =>
		{
			using (var first = File.OpenRead(Path.Combine(TempPath, First)))
				Storage.Put(first, Second);
		};

		It should_overwrite_the_old_file_with_the_new_one = () =>
			File.ReadAllText(Path.Combine(TempPath, Second)).ShouldEqual(Contents + "New");
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_deleting_a_nonexistent_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.Exists(Path.Combine(TempPath, First)).ShouldBeFalse();
			
		Because of = () =>
			Storage.Delete(First);

		It should_do_nothing_and_not_report_any_error = () =>
			File.Exists(Path.Combine(TempPath, First)).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_deleting_an_existing_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			File.AppendAllText(Path.Combine(TempPath, First), Contents);
			File.Exists(Path.Combine(TempPath, First)).ShouldBeTrue();
		};

		Because of = () =>
			Storage.Delete(First);
		
		It should_delete_the_file = () =>
			File.Exists(Path.Combine(TempPath, First)).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_deleting_a_nonexistent_directory : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			Directory.Exists(InnerTempPath).ShouldBeFalse();
		
		Because of = () =>
			Storage.Delete(InnerTempPath);

		It should_do_nothing_and_not_report_any_error = () =>
			Directory.Exists(InnerTempPath).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_deleting_an_existing_directory : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			Directory.CreateDirectory(InnerTempPath);
			Directory.Exists(InnerTempPath).ShouldBeTrue();
			File.AppendAllText(Third, Contents);
		};

		Because of = () =>
			Storage.Delete(InnerTempPath);

		It should_delete_the_directory_and_all_contents = () =>
			Directory.Exists(InnerTempPath).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_checking_for_a_nonexistent_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.Exists(Path.Combine(TempPath, First)).ShouldBeFalse();

		It should_report_that_it_does_not_exist = () =>
			Storage.Exists(Path.Combine(TempPath, First)).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_checking_for_an_existing_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.AppendAllText(Path.Combine(TempPath, First), Contents);

		It should_report_that_it_does_exist = () =>
			Storage.Exists(Path.Combine(TempPath, First)).ShouldBeTrue();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_checking_for_a_nonexistent_directory : using_the_local_file_storage_adapter
	{
		It should_report_that_it_does_not_exist = () =>
			Storage.Exists(InnerTempPath).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_renaming_a_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.AppendAllText(Path.Combine(TempPath, First), Contents);

		Because of = () =>
			Storage.Rename(First, Second);

		It should_delete_the_original_location = () =>
			File.Exists(Path.Combine(TempPath, First)).ShouldBeFalse();

		It should_exist_in_the_new_location = () =>
			File.Exists(Path.Combine(TempPath, Second)).ShouldBeTrue();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_enumerating_directory_contents_of_a_nonexistent_directory : using_the_local_file_storage_adapter
	{
		Because of = () =>
			exception = Catch.Exception(() => Storage.EnumerateObjects(InnerTempPath));

		It should_raise_an_error = () =>
			exception.ShouldBeOfType<DirectoryNotFoundException>();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_enumerating_directory_contents_of_an_existing_directory : using_the_local_file_storage_adapter
	{
		static readonly IEnumerable<string> Expected = new List<string> { First, Second, InnerTempPath };
		static IEnumerable<string> actual;

		Establish context = () =>
		{
			File.AppendAllText(Path.Combine(TempPath, First), Contents);
			File.AppendAllText(Path.Combine(TempPath, Second), Contents);
			Directory.CreateDirectory(InnerTempPath);
		};

		Because of = () =>
			actual = Storage.EnumerateObjects(string.Empty);

		It should_return_all_entries = () =>
		{
			foreach (var thing in Expected)
				actual.ShouldContain(Path.Combine(TempPath, thing));

			actual.Count().ShouldEqual(Expected.Count());
		};
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_downloading_a_file : using_the_local_file_storage_adapter
	{
		static readonly string Local = Path.Combine("./", First);

		Establish context = () =>
		{
			File.Delete(Local);
			File.Delete(Path.Combine(TempPath, First));
			File.AppendAllText(Path.Combine(TempPath, First), Contents);
		};

		Because of = () =>
			Storage.Download(Path.Combine(TempPath, First), Local);

		It should_download_the_file_to_the_local_location = () =>
		{
			File.Exists(Local).ShouldBeTrue();
			File.ReadAllText(Local).ShouldEqual(Contents);
		};

		It should_keep_the_file_at_the_remote_location = () =>
			File.Exists(Path.Combine(TempPath, First)).ShouldBeTrue();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_uploading_a_file : using_the_local_file_storage_adapter
	{
		static readonly string Local = Path.Combine("./", Path.GetFileName(First));

		Establish context = () =>
		{
			File.Delete(Local);
			File.Delete(First);
			File.AppendAllText(Local, Contents);
		};

		Because of = () =>
			Storage.Upload(Local, First);

		It should_place_the_file_in_the_remote_location = () =>
		{
			File.Exists(First).ShouldBeTrue();
			File.ReadAllText(First).ShouldEqual(Contents);
		};

		It should_keep_a_copy_of_the_file_in_the_local_location = () =>
			File.Exists(Local).ShouldBeTrue();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_retreiving_the_download_url : using_the_local_file_storage_adapter
	{
		static string result;

		Because of = () =>
			result = Storage.GetDownloadUrl("path");

		It should_provide_a_well_formed_local_url = () =>
		{
			result.ShouldStartWith("file:///");
			result.ShouldEndWith("path");
			result.ShouldContain(TempPath);
		};
	}

	public abstract class using_the_local_file_storage_adapter
	{
		protected const string Contents = "This is a test";
		protected static readonly string TempPath = Path.Combine(Path.GetTempPath(), "LocalFileStorage");
		protected static readonly string First = "first.txt";
		protected static readonly string Second = "second.txt";
		protected static readonly string InnerTempPath = Path.Combine(TempPath, "inner");
		protected static readonly string Third = Path.Combine(InnerTempPath, "third");
		protected static readonly LocalFileStorage Storage = new LocalFileStorage(TempPath);
		protected static Exception exception;

		Establish context = () =>
		{
			if (Directory.Exists(TempPath))
				Directory.Delete(TempPath, true);
			Directory.CreateDirectory(TempPath);
		};
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169