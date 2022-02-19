using SimpleBank.Application.Interfaces;
using SimpleBank.Application.Models;
using SimpleBank.Data;
using SimpleBank.Data.Interfaces;

namespace SimpleBank.Application.Tests
{
    public class CustomerAccountServiceTestHarness
    {
        private ICustomerAccountService _customerAccountService;
        
        // injectables
        private ILedgerRepository _ledgerRepository;

        public CustomerAccountServiceTestHarness()
        {
            _ledgerRepository = new LedgerRepository();
            _customerAccountService = new CustomerAccountService(_ledgerRepository);
        }

        #region TEST SETUP

        public CustomerAccountServiceTestHarness WithNewEmptyAccount(int accountNumber)
        {
            _ledgerRepository.CreateCustomerAccount(accountNumber);
            return this;
        }

        #endregion

        #region ACT

        public TransactionResult Execute_Deposit(int accountNumber, decimal amount)
        {
            return _customerAccountService.Deposit(accountNumber, amount);
        }

        public TransactionResult Execute_Withdraw(int accountNumber, decimal amount)
        {
            return _customerAccountService.Withdraw(accountNumber, amount);
        }

        public TransactionResult Execute_GetBalance(int accountNumber)
        {
            return _customerAccountService.GetBalance(accountNumber);
        }

        public decimal Execute_GetBankTotalBalance()
        {
            return _customerAccountService.GetBankTotalBalance();
        }

        #endregion
    }
}
