using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using FileModification.Models;
using FileModification.Resources;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FileModification
{

    public class Function
    {
        public async Task<Result> FunctionHandler(S3Event s3Event, ILambdaContext context)
        {
            var objectKey = s3Event.Records[0].S3.Object.Key;
            var bucketName = s3Event.Records[0].S3.Bucket.Name;

            if (string.IsNullOrWhiteSpace(objectKey))
            {
                return new Result(false, ErrorMessages.S3Validation_KeyIsNullOrEmpty);
            }
            
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                return new Result(false, ErrorMessages.S3Validation_BucketNameIsNullOrEmpty);
            }

            var s3Client = new AmazonS3Client();

            var objectAttributesRequest = new GetObjectAttributesRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            //Get object metadata
            var objectAttributes = await s3Client.GetObjectAttributesAsync(objectAttributesRequest);
            
        }
    }
}
