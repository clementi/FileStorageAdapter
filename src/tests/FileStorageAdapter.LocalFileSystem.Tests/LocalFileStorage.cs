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
			Exception = Catch.Exception(() => Storage.Get("C:/Does/Not/Exist.txt"));

		It should_throw_an_exception = () => 
			Exception.ShouldBeOfType<DirectoryNotFoundException>();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_getting_a_nonexistent_file : using_the_local_file_storage_adapter
	{
		Because of = () =>
			Exception = Catch.Exception(() => Storage.Get("C:/Does_Not_Exist.txt"));

		It should_throw_an_exception = () =>
			Exception.ShouldBeOfType<FileNotFoundException>();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_getting_an_existing_file : using_the_local_file_storage_adapter
	{
		static Stream FileHandle;

		Establish context = () =>
			File.AppendAllText(First, Contents);

		Because of = () =>
			FileHandle = Storage.Get(First);

		It should_open_the_file_for_reading = () =>
		{
			FileHandle.ShouldNotBeNull();
			FileHandle.CanRead.ShouldBeTrue();
			FileHandle.Length.ShouldEqual(Contents.Length);
			FileHandle.Dispose();
		};
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_putting_a_file : using_the_local_file_storage_adapter
	{
		Establish context = () => 
			File.AppendAllText(First, Contents);

		Because of = () =>
		{
			using (var first = File.OpenRead(First))
				Storage.Put(first, Second);
		};

		It should_deposit_the_entire_file_at_the_specified_location = () =>
		{
			File.Exists(Second).ShouldBeTrue();
			File.ReadAllText(Second).ShouldEqual(Contents);
		};
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_putting_a_file_that_was_stored_previously : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			File.AppendAllText(First, Contents + "New");
			File.AppendAllText(Second, Contents);
		};

		Because of = () =>
		{
			using (var first = File.OpenRead(First))
				Storage.Put(first, Second);
		};

		It should_overwrite_the_old_file_with_the_new_one = () =>
			File.ReadAllText(Second).ShouldEqual(Contents + "New");
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_deleting_a_nonexistent_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.Exists(First).ShouldBeFalse();
			
		Because of = () =>
			Storage.Delete(First);

		It should_do_nothing_and_not_report_any_error = () =>
			File.Exists(First).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_deleting_an_existing_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			File.AppendAllText(First, Contents);
			File.Exists(First).ShouldBeTrue();
		};

		Because of = () =>
			Storage.Delete(First);
		
		It should_delete_the_file = () =>
			File.Exists(First).ShouldBeFalse();
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
			File.Exists(First).ShouldBeFalse();

		It should_report_that_it_does_not_exist = () =>
			Storage.Exists(First).ShouldBeFalse();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_checking_for_an_existing_file : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.AppendAllText(First, Contents);

		It should_report_that_it_does_exist = () =>
			Storage.Exists(First).ShouldBeTrue();
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_checking_for_an_existing_directory : using_the_local_file_storage_adapter
	{
		It should_report_that_it_does_not_exist = () =>
			Storage.Exists(TempPath).ShouldBeTrue();
	}

	public class when_checking_for_a_nonexistent_directory : using_the_local_file_storage_adapter
	{
		It should_report_that_it_does_not_exist = () =>
			Storage.Exists(InnerTempPath).ShouldBeFalse();
	}

	public class when_enumerating_directory_contents_of_a_nonexistent_directory : using_the_local_file_storage_adapter
	{
		Because of = () =>
			Exception = Catch.Exception(() => Storage.EnumerateObjects(InnerTempPath));

		It should_raise_an_error = () =>
			Exception.ShouldBeOfType<DirectoryNotFoundException>();
	}

	public class when_enumerating_directory_contents_of_an_existing_directory :	using_the_local_file_storage_adapter
	{
		static IEnumerable<string> Expected = new List<string> { First, Second, InnerTempPath };
		static IEnumerable<string> Actual;

		Establish context = () =>
		{
			File.AppendAllText(First, Contents);
			File.AppendAllText(Second, Contents);
			Directory.CreateDirectory(InnerTempPath);
		};

		Because of = () =>
			Actual = Storage.EnumerateObjects(TempPath);

		It should_return_all_entries = () =>
		{
			foreach (var thing in Expected)
				Actual.ShouldContain(thing);

			Actual.Count().ShouldEqual(Expected.Count());
		};
	}

	public abstract class using_the_local_file_storage_adapter
	{
		protected static Exception Exception;
		protected const string Contents = "This is a test";
		protected static string TempPath = Path.Combine(Path.GetTempPath(), "LocalFileStorage");
		protected static string First = Path.Combine(TempPath, "first.txt");
		protected static string Second = Path.Combine(TempPath, "second.txt");
		protected static string InnerTempPath = Path.Combine(TempPath, "inner");
		protected static string Third = Path.Combine(InnerTempPath, "third");
		protected static LocalFileStorage Storage = new LocalFileStorage();
		
		Establish context = () =>
			Directory.CreateDirectory(TempPath);

		Cleanup after = () =>
			Directory.Delete(TempPath, true);
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169