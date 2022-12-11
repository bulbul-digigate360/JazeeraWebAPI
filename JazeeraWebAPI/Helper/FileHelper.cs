using System.Text;
using TripLover.AmazonS3;

namespace JazeeraWebAPI.Helper;

public class FileHelper
{
       private static readonly string bucketName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BucketName").Value;
        public static void ToWriteJson(string fileName, string folderName, string jsonString)
        {
            string path = Environment.CurrentDirectory + "/" + folderName + "/" + fileName + ".json";

            if (!Directory.Exists(path))
            {
                string dirPath = Path.GetDirectoryName(path);
                if (dirPath == null) throw new InvalidOperationException("Failure to save local security settings");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(path, jsonString, Encoding.ASCII);
            if (Convert.ToBoolean(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("IsS3Live").Value))
            {
                S3ServiceApis s3Services = new S3ServiceApis();
                if (s3Services.UploadFileAsync(bucketName, $"{folderName}/{fileName}", path).Result == true)
                {
                    File.Delete(path);
                }
            }
        }
        public static string ToReadJson(string fileName, string folderName)
        {
            try
            {
                S3ServiceApis s3Services = new S3ServiceApis();
                var s3Result = Convert.ToBoolean(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("IsS3Live").Value) ?
                    s3Services.ReadObjectFromBucketAsync(bucketName, $"{folderName}/{fileName}", "").Result :
                    (false, string.Empty, string.Empty);
                if (s3Result.Item1)
                {
                    return s3Result.Item2;
                }
                else
                {
                    string path = Environment.CurrentDirectory + "/" + folderName + "/" + fileName + ".json";
                    return File.ReadAllText(path);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            
        }
}