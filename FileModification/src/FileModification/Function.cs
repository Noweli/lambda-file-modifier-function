using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using FileModification.FileProcessing;
using FileModification.Models;
using FileModification.Resources;
using FileModification.S3;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FileModification
{
    public class Function
    {
        public async Task<Result<bool>> FunctionHandler(S3Event s3Event, ILambdaContext context)
        {
            var bucketName = s3Event.Records[0].S3.Bucket.Name;
            var objectKey = s3Event.Records[0].S3.Object.Key;

            var retrieveMetadataResult = await S3Handler.GetFileTypeCodeFromObject(bucketName, objectKey);

            if (!retrieveMetadataResult.IsSuccessful)
            {
                return new Result<bool>(false, retrieveMetadataResult.ErrorMessage);
            }

            var determinedProcessor = ProcessorSelector.GetFileProcessor(retrieveMetadataResult.ResultObject);

            if (determinedProcessor is null)
            {
                return new Result<bool>(false, ErrorMessages.ProcessorSelection_FailedToDetermineProcessor);
            }

            var fileForModificationKey = await CopyFileForModification(bucketName, objectKey);
            
            // Process file
            var processResult = await determinedProcessor.ProcessFile(await S3Handler.GetFileStream(bucketName, fileForModificationKey));

            return processResult;
        }
        
        private static async Task<string> CopyFileForModification(string sourceBucketName, string sourceObjectKey)
        {
            string destinationObjectKey;
            if (sourceObjectKey.Contains('/'))
            {
                var indexOfLastSlashInObjectKey = sourceObjectKey.LastIndexOf('/') + 1;
                destinationObjectKey = sourceObjectKey[indexOfLastSlashInObjectKey..];
            }
            else
            {
                destinationObjectKey = sourceObjectKey;
            }

            _ = await S3Handler.CopyS3Object(sourceBucketName, sourceObjectKey,
                sourceBucketName, destinationObjectKey);

            return destinationObjectKey;
        }
    }
}