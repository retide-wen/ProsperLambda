using System.Globalization;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using ProsperLambda.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProsperUploadNotesLambda
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

        public async Task<string> FunctionHandler(PayLoadRequest request, ILambdaContext context)
        {
            if (request.CSVBase64EncodedString == null)
            {
                return "Bad Request. CSV is missing";
            }

            try
            {
                byte[] csvBytes = Convert.FromBase64String(request.CSVBase64EncodedString);

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

                    var config = new DynamoDBOperationConfig
                    {
                        OverrideTableName = "ProsperNote"
                    };

                    foreach (var record in records)
                    {
                        await _context.SaveAsync(record, config);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error: {ex.Message}");
                return "Internal Server Error";
            }

            return "Success";
        }
    }
}