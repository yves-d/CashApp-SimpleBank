using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleBank.Application.Exceptions;
using SimpleBank.Application.Interfaces;
using SimpleBank.Application.Models;
using SimpleBank.Data;
using SimpleBank.Data.Interfaces;
using System;
using Xunit;

namespace SimpleBank.Application.Tests
{
    public class CustomerAccountServiceUnitTests
    {
        private ICustomerAccountService _customerAccountService;

        // dependency collection
        IServiceProvider _serviceProvider;

        private const int ACCOUNT_NUMBER_ALICE = 1;
        private const int ACCOUNT_NUMBER_BOB = 2;
        private const string ACCOUNT_NUMBER_NOT_FOUND_EXCEPTION_MESSAGE_BOB = "Customer account number '2' does not exist!";

        public CustomerAccountServiceUnitTests()
        {
            //setup dependencies
            _serviceProvider = new ServiceCollection()
                .AddTransient<ICustomerAccountService, CustomerAccountService>()
                .AddSingleton<ILedgerRepository, LedgerRepository>()
                .BuildServiceProvider();

            _customerAccountService = _serviceProvider.GetService<ICustomerAccountService>();
        }

        #region GET BALANCE

        [Fact]
        public void WHEN_Customer_Account_Does_Not_Exist_THEN_GetBalance_SHOULD_Throw_CustomerAccountNotFoundException()
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);

            // act
            Action act = () => _customerAccountService.GetBalance(ACCOUNT_NUMBER_BOB);

            // assert
            act.Should().Throw<CustomerAccountNotFoundException>()
                .WithMessage(ACCOUNT_NUMBER_NOT_FOUND_EXCEPTION_MESSAGE_BOB);
        }

        [Theory]
        [InlineData(0.00)]
        [InlineData(0.01)]
        [InlineData(0.02)]
        [InlineData(1.00)]
        [InlineData(2.00)]
        public void WHEN_Customer_Account_Exists_THEN_GetBalance_SHOULD_Return_Account_Balance_AND_TransactionOutcome_SHOULD_Be_SUCCESS(
            decimal startingBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, startingBalance);

            // act
            TransactionResult transactionResult = _customerAccountService.GetBalance(ACCOUNT_NUMBER_ALICE);

            // assert
            transactionResult.CurrentBalance.Should().Be(startingBalance);
            transactionResult.Outcome.Should().Be(TransactionOutcomeEnum.SUCCESS);
        }

        #endregion

        #region DEPOSIT

        [Fact]
        public void WHEN_Deposit_Is_Attempted_AND_Customer_Account_Does_Not_Exist_THEN_Deposit_SHOULD_Throw_CustomerAccountNotFoundException()
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);

            // act
            Action act = () => _customerAccountService.Deposit(ACCOUNT_NUMBER_BOB, 1.00m);

            // assert
            act.Should().Throw<CustomerAccountNotFoundException>()
                .WithMessage(ACCOUNT_NUMBER_NOT_FOUND_EXCEPTION_MESSAGE_BOB);
        }

        [Theory]
        [InlineData(0.00)]
        [InlineData(-0.01)]
        [InlineData(-1.00)]
        public void WHEN_Deposits_Amount_Is_Below_Minimum_Transaction_Amount_THEN_Balance_SHOULD_Remain_Unchanged_AND_TransactionOutcome_SHOULD_Be_TRANSACTION_BELOW_MINIMUM_AMOUNT(
            decimal depositAmount)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);

            // act
            TransactionResult transactionResult = _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, depositAmount);

            // assert
            transactionResult.CurrentBalance.Should().Be(0.00m);
            transactionResult.Outcome.Should().Be(TransactionOutcomeEnum.TRANSACTION_BELOW_MINIMUM_AMOUNT);
        }

        [Theory]
        [InlineData(0.00, 0.01, 0.01)]
        [InlineData(0.01, 0.01, 0.02)]
        [InlineData(0.00, 1.00, 1.00)]
        [InlineData(1.00, 1.00, 2.00)]
        public void WHEN_Customer_Deposits_Amount_Into_Account_THEN_Resulting_Balance_SHOULD_Be_Sum_Of_Deposit_AND_Original_Balance(
            decimal startingBalance,
            decimal depositAmount,
            decimal resultingBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, startingBalance);

            // act
            TransactionResult transactionResult = _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, depositAmount);

            // assert
            transactionResult.CurrentBalance.Should().Be(resultingBalance);
        }

        #endregion

        #region WITHDRAWAL

        [Fact]
        public void WHEN_Withdraw_Is_Attempted_AND_Customer_Account_Does_Not_Exist_THEN_Withdraw_SHOULD_Throw_CustomerAccountNotFoundException()
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);

            // act
            Action act = () => _customerAccountService.Withdraw(ACCOUNT_NUMBER_BOB, 1.00m);

            // assert
            act.Should().Throw<CustomerAccountNotFoundException>()
                .WithMessage(ACCOUNT_NUMBER_NOT_FOUND_EXCEPTION_MESSAGE_BOB);
        }

        [Theory]
        [InlineData(0.00, 0.01)]
        [InlineData(0.01, 0.02)]
        [InlineData(0.00, 1.00)]
        [InlineData(1.00, 2.00)]
        public void WHEN_Customer_Attempts_To_Overdraw_Account_THEN_Balance_SHOULD_Remain_Unchanged_AND_TransactionOutcome_SHOULD_Be_OVERDRAW_PREVENTED(
            decimal startingBalance,
            decimal withdrawalAmount)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, startingBalance);

            // act
            TransactionResult transactionResult = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, withdrawalAmount);

            // assert
            transactionResult.CurrentBalance.Should().Be(startingBalance);
            transactionResult.Outcome.Should().Be(TransactionOutcomeEnum.OVERDRAW_PREVENTED);
        }

        [Theory]
        [InlineData(0.00)]
        [InlineData(-0.01)]
        [InlineData(-1.00)]
        public void WHEN_Withdraw_Amount_Is_Below_Minimum_Transaction_Amount_THEN_Balance_SHOULD_Remain_Unchanged_AND_TransactionOutcome_SHOULD_Be_TRANSACTION_BELOW_MINIMUM_AMOUNT(
            decimal withdrawalAmount)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);

            // act
            TransactionResult transactionResult = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, withdrawalAmount);

            // assert
            transactionResult.CurrentBalance.Should().Be(0.00m);
            transactionResult.Outcome.Should().Be(TransactionOutcomeEnum.TRANSACTION_BELOW_MINIMUM_AMOUNT);
        }

        [Theory]
        [InlineData(1.00, 1.00, 0.00)]
        [InlineData(2.00, 1.00, 1.00)]
        public void WHEN_Customer_Withdraws_Amount_From_Account_THEN_Resulting_Balance_SHOULD_Be_Original_Balance_Minus_Withdrawal_Amount(
            decimal startingBalance,
            decimal withdrawalAmount,
            decimal resultingBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, startingBalance);

            // act
            TransactionResult transactionResult = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, withdrawalAmount);

            // assert
            transactionResult.CurrentBalance.Should().Be(resultingBalance);
        }

        #endregion
    }
}