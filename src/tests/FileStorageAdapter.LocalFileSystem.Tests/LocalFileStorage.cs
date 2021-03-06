﻿#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace FileStorageAdapter.LocalFileSystem.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading;
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
			files = Storage.EnumerateObjects(InnerTempPath);

		It should_return_an_empty_set = () =>
			files.ShouldBeEmpty();

		static IEnumerable<string> files;
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_enumerating_directory_contents_of_an_existing_directory : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			File.AppendAllText(Expected.First(), Contents);
			File.AppendAllText(Expected.Last(), Contents);
			Directory.CreateDirectory(InnerTempPath);
		};

		Because of = () =>
			actual = Storage.EnumerateObjects(TempPath);

		It should_return_all_entries = () =>
			actual.SequenceEqual(Expected).ShouldBeTrue();

		static IEnumerable<string> actual;
		static readonly IList<string> Expected = new List<string>
		{
			Path.Combine(TempPath, First), 
			InnerTempPath,
			Path.Combine(TempPath, Second)
		};
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_enumerating_directory_contents_of_a_NON_directory : using_the_local_file_storage_adapter
	{
		Establish context = () =>
			File.AppendAllText(path, Contents);

		Because of = () =>
			exception = Catch.Exception(() => Storage.EnumerateObjects(path));

		It should_throw_an_exception = () =>
			exception.ShouldBeOfType<IOException>();

		static readonly string path = Path.Combine(TempPath, First);
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_enumerating_a_nonexistent_directory_using_a_filter : using_the_local_file_storage_adapter
	{
		Because of = () =>
			files = Storage.EnumerateObjects(InnerTempPath, x => true);

		It should_return_an_empty_set = () =>
			files.ShouldBeEmpty();

		static IEnumerable<string> files;
	}

	[Subject(typeof(LocalFileStorage))]
	public class when_enumerating_using_a_filter : using_the_local_file_storage_adapter
	{
		Establish context = () =>
		{
			File.AppendAllText(Expected.First(), Contents);
			Thread.Sleep(100);
			threshold = DateTime.UtcNow;
			File.AppendAllText(Expected.Last(), Contents);
			Directory.CreateDirectory(InnerTempPath);
		};

		Because of = () =>
			actual = Storage.EnumerateObjects(TempPath, date => date < threshold);

		It should_only_return_files_that_match_the_given_predicate = () =>
			actual.SequenceEqual(Expected).ShouldBeTrue();

		static DateTime threshold;
		static IEnumerable<string> actual;
		static readonly IList<string> Expected = new List<string>
		{
			Path.Combine(TempPath, First), 
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

	[Subject(typeof(LocalFileStorage))]
	public class when_using_the_download_url_to_check_for_an_existing_file : using_the_local_file_storage_adapter
	{
		private static string url;
		private static bool result;

		Establish context = () =>
		{
			File.AppendAllText(Path.Combine(TempPath, First), Contents);
			url = Storage.GetDownloadUrl(Path.Combine(TempPath, First));
		};

		Because of = () =>
			result = Storage.Exists(url);

		It should_show_that_it_exists = () =>
			result.ShouldBeTrue();
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
			Cleanup();
			Directory.CreateDirectory(TempPath);
		};

		Cleanup after = Cleanup;

		private static void Cleanup()
		{
			if (Directory.Exists(TempPath))
				Directory.Delete(TempPath, true);
		}
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169