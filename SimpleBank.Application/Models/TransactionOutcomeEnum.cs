namespace SimpleBank.Application.Models
{
    public enum TransactionOutcomeEnum
    {
        SUCCESS,
        TRANSACTION_BELOW_MINIMUM_AMOUNT,
        OVERDRAW_PREVENTED
    }
}
