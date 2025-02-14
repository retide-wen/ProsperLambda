using System.Globalization;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using ProsperLambda.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProsperLambda
{
    public class Function
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDBContext _context;

        public Function()
        {
            _dynamoDbClient = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_dynamoDbClient);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (request.Body == null)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Bad Request. The Request Body is empty",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            PayLoadRequest? payLoadRequest = JsonConvert.DeserializeObject<PayLoadRequest>(request.Body);

            if (payLoadRequest == null || payLoadRequest.CSVBase64EncodedString == null)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Bad Request. CSV is missing",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            try
            {
                byte[] csvBytes = Convert.FromBase64String(payLoadRequest.CSVBase64EncodedString);

                using (var memoryStream = new MemoryStream(csvBytes))
                using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                }))
                {
                    csv.Context.RegisterClassMap<PrsoperRecordMapping>();
                    var records = csv.GetRecords<ProsperRecord>().ToList();
                    foreach (var record in records)
                    {
                        await _context.SaveAsync(record);
                    }
                }
            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = $"ex= {ex}",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = 202,
                Body = "Request accepted for processing",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}