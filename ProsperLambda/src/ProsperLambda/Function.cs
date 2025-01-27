using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

public class Function
{
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        ///var task = JsonConvert.DeserializeObject<Task>(request.Body);
        //task.Id = Guid.NewGuid().ToString();
        ///task.CreatedAt = DateTime.UtcNow.ToString("o");

        //using var client = new AmazonDynamoDBClient();
        //var table = Table.LoadTable(client, "Tasks");
        //await table.PutItemAsync(task);

        await Task.Delay(1000);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = "this is from my testing Lambda",
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}

