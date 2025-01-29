using System;
using CsvHelper.Configuration;

namespace ProsperLambda.Model
{
    public class PrsoperRecordMapping : ClassMap<ProsperRecord>
    {
        public PrsoperRecordMapping()
        {
            Map(x => x.Rating).Name("Rating");
            Map(x => x.NoteID).Name("Note ID");
            Map(x => x.InvestmentAmount).Name("Investment Amount");
            Map(x => x.Term).Name("Term");
            Map(x => x.PurchaseDate).Name("Purchase Date");
            Map(x => x.PaymentDueDate).Name("Payment Due Date");
            Map(x => x.PaymentsReceived).Name("Payments Received");
            Map(x => x.PrincipalReceived).Name("Principal Received");
            Map(x => x.Yield).Name("Yield");
            Map(x => x.Status).Name("Status");
            Map(x => x.SubStatus).Name("Sub Status");
        }
    }
}