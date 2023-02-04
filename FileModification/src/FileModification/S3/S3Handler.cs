using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FileModification.Models;
using FileModification.Resources;

namespace FileModification.S3;

public class S3Handler
{
    private const string METADATA_FILE_TYPE_CODE_KEY = "x-amz-meta-file-code";
    
    public static async Task<Result<string>> GetFileTypeCodeFromObject(string bucketName, string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return new Result<string>(false, ErrorMessages.S3Validation_KeyIsNullOrEmpty);
        }
            
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            return new Result<string>(false, ErrorMessages.S3Validation_BucketNameIsNullOrEmpty);
        }

        // https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/TS3Client.html
        var s3Client = new AmazonS3Client();

        var objectAttributesRequest = new GetObjectMetadataRequest()
        {
            BucketName = bucketName,
            Key = objectKey
        };

        // Get object metadata
        var objectMetadata = await s3Client.GetObjectMetadataAsync(objectAttributesRequest);

        var objectMetadataCollection = objectMetadata.Metadata;

        if (!objectMetadataCollection.Keys.Contains(METADATA_FILE_TYPE_CODE_KEY))
        {
            return new Result<string>(false,
                string.Format(ErrorMessages.S3Validation_MetadataWithTypeCodeKeyNotFound,
                    METADATA_FILE_TYPE_CODE_KEY));
        }

        //Used to determine file modification processor
        var fileTypeCode = objectMetadataCollection[METADATA_FILE_TYPE_CODE_KEY];

        if (string.IsNullOrWhiteSpace(fileTypeCode))
        {
            return new Result<string>(false, ErrorMessages.S3Validation_MetadataValueIsNullOrEmpty);
        }

        return new Result<string>(true, string.Empty, fileTypeCode);
    }

    public static async Task<Result<string>> CopyS3Object(string sourceBucketName, string sourceObjectKey,
        string destinationBucketName, string destinationObjectKey)
    {
        if (string.IsNullOrWhiteSpace(sourceBucketName) || string.IsNullOrEmpty(sourceObjectKey))
        {
            return new Result<string>(false, ErrorMessages.S3Validation_CopyObject_SourceIsNullOrEmpty);
        }
        
        if (string.IsNullOrWhiteSpace(destinationBucketName) || string.IsNullOrEmpty(destinationObjectKey))
        {
            return new Result<string>(false, ErrorMessages.S3Validation_CopyObject_DestinationIsNullOrEmpty);
        }

        var s3Client = new AmazonS3Client();
        var copyObjectRequest = new CopyObjectRequest
        {
            SourceBucket = sourceBucketName,
            SourceKey = sourceObjectKey,
            DestinationBucket = destinationBucketName,
            DestinationKey = destinationObjectKey
        };

        var copyObjectResult = await s3Client.CopyObjectAsync(copyObjectRequest);

        // That's more readable
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (copyObjectResult.HttpStatusCode != HttpStatusCode.OK)
        {
            return new Result<string>(false, ErrorMessages.S3Handler_CopyObjectFailed);
        }

        return new Result<string>(true, string.Empty, destinationObjectKey);
    }

    public static async Task<Stream> GetFileStream(string bucketName, string objectKey)
    {
        var s3Client = new AmazonS3Client();
        var s3Object = await s3Client.GetObjectAsync(bucketName, objectKey);

        return s3Object.ResponseStream;
    }
    
}