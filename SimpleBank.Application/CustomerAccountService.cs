using SimpleBank.Application.Exceptions;
using SimpleBank.Application.Interfaces;
using SimpleBank.Application.Models;
using SimpleBank.Data.Interfaces;

namespace SimpleBank.Application
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly ILedgerRepository _ledgerRepository;

        private const decimal NEW_ACCOUNT_STARTING_BALANCE = 0.00m;
        private const decimal MINIMUM_TRANSACTION_AMOUNT = 0.01m;
        private const decimal MINIMUM_ACCOUNT_BALANCE = 0.00m;
        private const string ACCOUNT_NUMBER_NOT_FOUND_MESSAGE = "Customer account number '{0}' does not exist!";

        public CustomerAccountService(ILedgerRepository ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }

        public void CreateCustomerAccount(int accountNumber)
        {
            _ledgerRepository.CreateCustomerAccount(accountNumber, NEW_ACCOUNT_STARTING_BALANCE);
        }

        public TransactionResult Deposit(int accountNumber, decimal amount)
        {
            ValidateAccountExists(accountNumber);

            if(TransactionAmountIsZeroOrLess(amount))
                return BuildTransactionResult(_ledgerRepository.GetCustomerAccountBalance(accountNumber), TransactionOutcomeEnum.TRANSACTION_BELOW_MINIMUM_AMOUNT);

            _ledgerRepository.DepositToCustomerAccount(accountNumber, amount);
            var accountBalance = _ledgerRepository.GetCustomerAccountBalance(accountNumber);

            return BuildTransactionResult(accountBalance, TransactionOutcomeEnum.SUCCESS);
        }

        public TransactionResult Withdraw(int accountNumber, decimal amount)
        {
            ValidateAccountExists(accountNumber);

            if (TransactionAmountIsZeroOrLess(amount))
                return BuildTransactionResult(_ledgerRepository.GetCustomerAccountBalance(accountNumber), TransactionOutcomeEnum.TRANSACTION_BELOW_MINIMUM_AMOUNT);

            var accountBalanceBeforeWithdrawal = _ledgerRepository.GetCustomerAccountBalance(accountNumber);
            if (AccountHasInsufficientFundsToWithdraw(accountBalanceBeforeWithdrawal, amount))
                return BuildTransactionResult(accountBalanceBeforeWithdrawal, TransactionOutcomeEnum.OVERDRAW_PREVENTED);
            
            _ledgerRepository.WithdrawFromCustomerAccount(accountNumber, amount);
            var accountBalanceAfterWithdrawal = _ledgerRepository.GetCustomerAccountBalance(accountNumber);

            return BuildTransactionResult(accountBalanceAfterWithdrawal, TransactionOutcomeEnum.SUCCESS);
        }

        public TransactionResult GetBalance(int accountNumber)
        {
            ValidateAccountExists(accountNumber);

            var accountBalance = _ledgerRepository.GetCustomerAccountBalance(accountNumber);

            return BuildTransactionResult(accountBalance, TransactionOutcomeEnum.SUCCESS);
        }

        public decimal GetBankTotalBalance()
        {
            return _ledgerRepository.GetBankTotalBalance();
        }

        private TransactionResult BuildTransactionResult(decimal accountBalance, TransactionOutcomeEnum transactionOutcome)
        {
            return new TransactionResult()
            {
                CurrentBalance = accountBalance,
                Outcome = transactionOutcome
            };
        }

        private void ValidateAccountExists(int accountNumber)
        {
            if (!_ledgerRepository.CustomerAccountExists(accountNumber))
                throw new CustomerAccountNotFoundException(string.Format(ACCOUNT_NUMBER_NOT_FOUND_MESSAGE, accountNumber));
        }

        private bool TransactionAmountIsZeroOrLess(decimal transactionAmount)
        {
            return transactionAmount < MINIMUM_TRANSACTION_AMOUNT;
        }

        private bool AccountHasInsufficientFundsToWithdraw(decimal accountBalance, decimal withdrawalAmount)
        {
            return (accountBalance - withdrawalAmount) < MINIMUM_ACCOUNT_BALANCE;
        }
    }
}
