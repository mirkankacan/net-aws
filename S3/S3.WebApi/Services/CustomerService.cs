using Amazon.S3;
using Amazon.S3.Model;
using System.Net;

namespace S3.WebApi.Services
{
    public class CustomerService(IAmazonS3 s3, IConfiguration configuration)
    {
        private readonly string bucketName = configuration["AwsOptions:BucketName"]!;
        public async Task<Guid> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid();
            var putObjRequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = $"images/{id}.jpg",
                ContentType = file.ContentType,
                InputStream = file.OpenReadStream(),
                Metadata =
                {
                      ["x-amz-meta-extension"] = Path.GetExtension(file.FileName),
                      ["x-amz-meta-title"] = file.FileName
                }
            };
            await s3.PutObjectAsync(putObjRequest, cancellationToken);
            return id;
        }
        public async Task<GetObjectResponse?> GetImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var getObjRequest = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = $"images/{id}.jpg",
            };
            try
            {
                return await s3.GetObjectAsync(getObjRequest, cancellationToken);
            }
            catch (NoSuchKeyException)
            {
                return null;
            }

        }
        public async Task<bool> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var key = $"images/{id}.jpg";

            // S3's DeleteObject is idempotent and succeeds even if the key doesn't exist,
            // so existence has to be checked explicitly to report NotFound accurately.
            try
            {
                await s3.GetObjectMetadataAsync(bucketName, key, cancellationToken);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            var deleteObjRequest = new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
            };
            await s3.DeleteObjectAsync(deleteObjRequest, cancellationToken);
            return true;
        }
    }
}
