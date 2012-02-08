namespace FileStorageAdapter.AmazonS3.IntegrationTests
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using Machine.Specifications;

	public class Program
	{
		public static void Main(string[] args)
		{
			Try(Cleanup);
			Try(Put);
			Try(Get);
			Try(Download);
			Try(Rename);
			Try(Upload);
			Try(GetDownloadUrl);
			Try(Delete);
			Try(Cleanup);

			Report();
		}

		private static void Cleanup()
		{
			foreach (var remotePath in new[] { RemotePath1, RemotePath2 })
				try { Storage.Delete(remotePath); }
				catch (FileNotFoundException) { }

			File.Delete(LocalPath);

			Storage.Exists(RemotePath1).ShouldBeFalse();
			Storage.Exists(RemotePath2).ShouldBeFalse();
			Storage.EnumerateObjects(Root).ShouldBeEmpty();
		}
		private static void Try(Action action)
		{
			try
			{
				Console.Write("Testing {0}...", action.Method.Name);
				action();
				Console.WriteLine("Done");
				passed++;
			}
			catch (Exception e)
			{
				Console.WriteLine();
				Console.WriteLine("Caught {0} during {1}.", e, action.Method.Name);
				Console.WriteLine(e.Message);
				if (e.InnerException != null)
					Console.WriteLine(e.InnerException.Message);
				Console.WriteLine();
				failed++;
			}
		}
		private static void Put()
		{
			using (var s = new MemoryStream(new UTF8Encoding().GetBytes(Content)))
				Storage.Put(s, RemotePath1);

			Storage.Exists(RemotePath1).ShouldBeTrue();
			Storage.EnumerateObjects(Root).SequenceEqual(new[] { RemotePath1 }).ShouldBeTrue();
		}
		private static void Get()
		{
			using (var content = Storage.Get(RemotePath1))
			using (var reader = new StreamReader(content))
				reader.ReadToEnd().ShouldEqual(Content);
		}
		private static void Download()
		{
			Storage.Download(RemotePath1, LocalPath);
			using (var reader = new StreamReader(LocalPath))
				reader.ReadToEnd().ShouldEqual(Content);
		}
		private static void Rename()
		{
			Storage.Rename(RemotePath1, RemotePath2);
			Storage.Exists(RemotePath1).ShouldBeFalse();
			Storage.Exists(RemotePath2).ShouldBeTrue();
			Storage.EnumerateObjects(Root).SequenceEqual(new[] { RemotePath2 }).ShouldBeTrue();
		}
		private static void Upload()
		{
			Storage.Upload(LocalPath, RemotePath1);
			Storage.Exists(RemotePath1).ShouldBeTrue();
			Storage.EnumerateObjects(Root).ShouldContain(RemotePath1);
		}
		private static void GetDownloadUrl()
		{
			using (var response = WebRequest.Create(Storage.GetDownloadUrl(RemotePath1)).GetResponse())
			using (var reader = new StreamReader(response.GetResponseStream() ?? new MemoryStream()))
				reader.ReadToEnd().ShouldEqual(Content);
		}
		private static void Delete()
		{
			foreach (var remotePath in new[] { RemotePath1, RemotePath2 })
				Delete(remotePath);

			Storage.EnumerateObjects(Root).ShouldBeEmpty();
		}
		private static void Delete(string path)
		{
			Storage.Delete(path);
			Storage.Exists(path).ShouldBeFalse();
			Storage.EnumerateObjects(Root).ShouldNotContain(path);
		}
		private static void Report()
		{
			var total = passed + failed;
			var score = passed / total;
			Console.WriteLine();
			Console.WriteLine("Executed {0} tests, {1} were correct ({2:P2})", total, passed, score);
			Console.WriteLine();
			Console.WriteLine(passed == total ? "OK" : "**FAILED** (check failed tests and try again)");
			Console.ReadLine();
		}

		private const string BucketName = "FileStorageAdapter-62fe2303";
		private const string Root = "/";
		private const string File1 = "File1.txt";
		private const string File2 = "File2.txt";
		private const string RemotePath1 = Root + File1;
		private const string RemotePath2 = Root + File2;
		private const string Content = "This is a test to see if the content is preserved.";
		private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
		private static readonly string LocalPath = Path.Combine(CurrentDirectory, File1);
		private static readonly string AwsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
		private static readonly string AwsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
		private static readonly AmazonS3Storage Storage = AmazonS3StorageBuilder
			.UsingCredentials(AwsAccessKey, AwsSecretAccessKey)
			.InBucket(BucketName)
			.Build();
		private static decimal passed;
		private static decimal failed;
	}
}
