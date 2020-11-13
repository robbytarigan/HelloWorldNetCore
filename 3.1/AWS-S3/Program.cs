using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace AWS_S3
{
    class Program
    {

        static async Task Main(string[] args)
        {
            const string BucketName = "example-bucket";
            const string TableName = "aircraft";
            var recorder = new Recorder(BucketName);

            var random = new Random();
            var countBytes = new byte[8];
            random.NextBytes(countBytes);
            var count = Math.Abs(BitConverter.ToInt64(countBytes, 0));

            var loadDate = DateTime.UtcNow.AddMonths(7);

            Console.WriteLine($"Get last count {await recorder.GetPreviousCount(loadDate, TableName)}");

            Console.WriteLine($"Saving count {loadDate:s} {count}");

            await recorder.SaveCount(loadDate, "aircraft", count);
        }


    }

    public sealed class Recorder
    {
        private const string CsvNewLine = "\r\n";
        private readonly string bucketName;
        private readonly TransferUtility transferUtility;

        public Recorder(string bucketName)
        {
            this.bucketName = bucketName;
            var s3Client = new AmazonS3Client(new AmazonS3Config { Timeout = TimeSpan.FromMinutes(2) });
            this.transferUtility = new TransferUtility(s3Client);
        }

        public async Task<long> GetPreviousCount(DateTime date, string tableName)
        {
            var previousDate =await GetPreviousDate(date, tableName);
            if (previousDate == DateTime.MinValue) {
                return -1;
            }

            using var previousStream = await GetDownloadStream(previousDate, tableName);
            return GetCount(await GetLastLine(previousStream));
        }    

        private async Task<string> GetLastLine(Stream stream)
        {
            string line = null;
            using (var streamReader = new StreamReader(stream))
            {
                while (!streamReader.EndOfStream)
                {
                    line = await streamReader.ReadLineAsync();
                }
            }

            return line;
        }

        private static long GetCount(string line)
        {
            if (line != null)
            {
                var lineParts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (lineParts.Length == 2)
                {
                    return long.Parse(lineParts[1]);
                }
            }

            return -1;
        }

        private async Task<DateTime> GetPreviousDate(DateTime date, string tableName)
        {
            var searchDate = date;

            for (var month = 0; month < 6; month++)
            {
                if (await this.FileExists(searchDate, tableName))
                {
                    return searchDate;
                }

                searchDate = searchDate.AddMonths(-1);
            }

            return DateTime.MinValue;
        }

        public async Task SaveCount(DateTime date, string tableName, long count)
        {
            var bytes = Encoding.UTF8.GetBytes($"{date:s},{count}{CsvNewLine}");
            using var stream = await GetStream(date, tableName);
            await stream.WriteAsync(bytes);

            await transferUtility.UploadAsync(stream, this.bucketName, GetUploadFileKey(date, tableName));
        }

        private async Task<bool> FileExists(DateTime loadDate, string tableName)
        {
            try
            {
                var metadata = await this.transferUtility.S3Client.GetObjectMetadataAsync(this.bucketName, GetUploadFileKey(loadDate, tableName));
                return metadata != null;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"WARN: {ex.ErrorCode} {ex.Message}");
                return false;
            }
        }

        private async Task<Stream> GetStream(DateTime date, string tableName)
        {
            if (!await this.FileExists(date, tableName))
            {
                return new MemoryStream();
            }

            var fileStream = await GetDownloadStream(date, tableName);
            fileStream.Seek(0, SeekOrigin.End);
            return fileStream;
        }

        private async Task<Stream> GetDownloadStream(DateTime date, string tableName)
        {
            var filePath = GetDownloadFilePath(tableName);
            await this.transferUtility.DownloadAsync(filePath, this.bucketName, GetUploadFileKey(date, tableName));
            return File.Open(filePath, FileMode.Open);
        }

        private static string GetDownloadFilePath(string tableName) => $"data/records-count/{tableName}.csv";

        private static string GetUploadFileKey(DateTime date, string tableName) => $"local/records-count/{date:yyyy-MM}/{tableName}.csv";
    }
}
