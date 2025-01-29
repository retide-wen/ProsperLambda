using Amazon.DynamoDBv2.DataModel;



namespace ProsperLambda.Model
{
    [DynamoDBTable("YourTableName")]
    public class ProsperRecord
    {
        [DynamoDBHashKey]
        public string? NoteID { get; set; }
        public string? Rating { get; set; }
        public string? InvestmentAmount { get; set; }
        public string? Term { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string? PaymentsReceived { get; set; }
        public string? PrincipalReceived { get; set; }
        public string? Yield { get; set; }
        public string? Status { get; set; }
        public string? SubStatus { get; set; }
    }
}
