using SimpleBank.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBank.Data
{
    public class LedgerRepository : ILedgerRepository
    {
        private readonly Dictionary<int, decimal> _ledgerOfAccounts = new Dictionary<int, decimal>();
        
        public void CreateCustomerAccount(int accountNumber)
        {
            _ledgerOfAccounts.Add(accountNumber, 0.00m);
        }

        public bool CustomerAccountExists(int accountNumber)
        {
            return _ledgerOfAccounts.ContainsKey(accountNumber);
        }

        public decimal GetCustomerAccountBalance(int accountNumber)
        {
            return _ledgerOfAccounts[accountNumber];
        }

        public void DepositToCustomerAccount(int accountNumber, decimal amount)
        {
            _ledgerOfAccounts[accountNumber] += amount;
        }

        public void WithdrawFromCustomerAccount(int accountNumber, decimal amount)
        {
            _ledgerOfAccounts[accountNumber] -= amount;
        }

        public decimal GetBankTotalBalance()
        {
            return _ledgerOfAccounts.Sum(customerAccount => customerAccount.Value);
        }
    }
}
