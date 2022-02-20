using SimpleBank.Application.Models;

namespace SimpleBank.Application.Interfaces
{
    public interface ICustomerAccountService
    {
        /// <summary>
        /// Create a new customer account with the specified account number.
        /// </summary>
        /// <param name="accountNumber"></param>
        void CreateCustomerAccount(int accountNumber);

        /// <summary>
        /// Deposit into a customer's account.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        TransactionResult Deposit(int accountNumber, decimal amount);

        /// <summary>
        /// Withdraw from a customer's account.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        TransactionResult Withdraw(int accountNumber, decimal amount);

        /// <summary>
        /// Get a customer's account balance.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        TransactionResult GetBalance(int accountNumber);

        /// <summary>
        /// Get the total of all the bank's customer accounts.
        /// </summary>
        /// <returns></returns>
        decimal GetBankTotalBalance();
    }
}
