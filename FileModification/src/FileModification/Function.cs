using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FileModification
{

    public class Function
    {

        public async Task FunctionHandler(S3Event s3Event, ILambdaContext context)
        {
            
        }
    }
}
