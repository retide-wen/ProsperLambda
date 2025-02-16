using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProsperLambda
{
    public class Function
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDBContext _context;

        public Function()
        {
            try
            {
                LambdaLogger.Log("Initializing DynamoDB client and context");
                _dynamoDbClient = new AmazonDynamoDBClient();
                _context = new DynamoDBContext(_dynamoDbClient);
                LambdaLogger.Log("Initialization successful");
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"Initialization error: {ex.Message}");
                throw;
            }
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("FunctionHandler invoked");

            APIGatewayProxyResponse response;

            if (request == null || string.IsNullOrEmpty(request.Body))
            {
                context.Logger.LogLine("Invalid request: request body is null or empty");
                response = new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Invalid request: request body is null or empty",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            else
            {
                try
                {
                    var config = new DynamoDBOperationConfig
                    {
                        OverrideTableName = "ProsperNote"
                    };
                    var conditions = new List<ScanCondition>();
                    // you can add scan conditions, or leave empty
                    List<ProsperRecord> allRecords = await _context.ScanAsync<ProsperRecord>(conditions, config).GetRemainingAsync();

                    response = new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Body = JsonConvert.SerializeObject(allRecords),
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };

                    context.Logger.LogLine("FunctionHandler completed successfully");
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Error processing request: {ex.Message}");
                    response = new APIGatewayProxyResponse
                    {
                        StatusCode = 500,
                        Body = "Internal server error",
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }
            }

            return response;
        }
    }

}