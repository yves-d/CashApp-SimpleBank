namespace SimpleBank.Application.Models
{
    public class TransactionResult
    {
        public decimal CurrentBalance { get; init; }
        public TransactionOutcomeEnum Outcome { get; init;}
    }
}
