namespace SimpleBank.Data.Interfaces
{
    public interface ILedgerRepository
    {
        /// <summary>
        /// Create a new customer account record in the database, with a zero balance.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="startingBalance"></param>
        void CreateCustomerAccount(int accountNumber);

        /// <summary>
        /// Check if a customer account exists in the database, for the provided account number.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        bool CustomerAccountExists(int accountNumber);

        /// <summary>
        /// Get the persisted account balance for the provided account number.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        decimal GetCustomerAccountBalance(int accountNumber);

        /// <summary>
        /// Update the persisted customer account balance with the deposited amount.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="amount"></param>
        void DepositToCustomerAccount(int accountNumber, decimal amount);

        /// <summary>
        /// Update the persisted customer account balance with the withdrawn amount.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="amount"></param>
        void WithdrawFromCustomerAccount(int accountNumber, decimal amount);

        /// <summary>
        /// Get the sum total amount of all persisted customer account balances.
        /// </summary>
        /// <returns></returns>
        decimal GetBankTotalBalance();
    }
}
