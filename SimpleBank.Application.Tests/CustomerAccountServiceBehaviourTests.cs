using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleBank.Application.Interfaces;
using SimpleBank.Application.Models;
using SimpleBank.Data;
using SimpleBank.Data.Interfaces;
using System;
using Xunit;

namespace SimpleBank.Application.Tests
{
    public class CustomerAccountServiceBehaviourTests
    {
        private ICustomerAccountService _customerAccountService;

        // dependency collection
        IServiceProvider _serviceProvider;

        private const int ACCOUNT_NUMBER_ALICE = 1;
        private const int ACCOUNT_NUMBER_BOB = 2;
        private const int ACCOUNT_NUMBER_SATOSHI = 3;

        public CustomerAccountServiceBehaviourTests()
        {
            //setup dependencies
            _serviceProvider = new ServiceCollection()
                .AddTransient<ICustomerAccountService, CustomerAccountService>()
                .AddSingleton<ILedgerRepository, LedgerRepository>()
                .BuildServiceProvider();

            _customerAccountService = _serviceProvider.GetService<ICustomerAccountService>();
        }

        #region MULTIPLE ACTION COMBO TESTS

        [Fact]
        public void WHEN_Alice_Deposits_30_AND_Withdraws_20_THEN_Alices_Balance_SHOULD_Be_10_AND_Banks_Balance_Should_Be_10_AND_Alice_Cannot_Withdraw_11()
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);

            // act - initial deposit and withdrawal
            TransactionResult transactionResult = _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, 30.00m);
            transactionResult = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, 20.00m);
            decimal bankTotalBalance = _customerAccountService.GetBankTotalBalance();

            // assert
            transactionResult.CurrentBalance.Should().Be(10.00m);
            bankTotalBalance.Should().Be(10.00m);

            // act - follow up withdrawal attempt
            transactionResult = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, 11.00m);
            bankTotalBalance = _customerAccountService.GetBankTotalBalance();

            // assert - nothing should have changed
            transactionResult.CurrentBalance.Should().Be(10.00m);
            transactionResult.Outcome.Should().Be(TransactionOutcomeEnum.OVERDRAW_PREVENTED);
            bankTotalBalance.Should().Be(10.00m);
        }

        #endregion

        #region DEPOSITS TO MULTIPLE ACCOUNTS

        [Theory]
        [InlineData(0.01, 0.01, 0.02)]
        [InlineData(0.01, 0.02, 0.03)]
        [InlineData(1.00, 0.01, 1.01)]
        [InlineData(1.00, 1.00, 2.00)]
        [InlineData(1.00, 2.00, 3.00)]
        public void WHEN_Deposits_Are_Made_By_Two_Customers_THEN_Customer_Balances_Should_Equal_Deposits_AND_Bank_Total_Balance_Should_Equal_Sum_Of_All_Customer_Balances(
            decimal depositAmountForAlice,
            decimal depositAmountForBob,
            decimal banksTotalBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_BOB);

            // act
            TransactionResult transactionResultAlice = _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, depositAmountForAlice);
            TransactionResult transactionResultBob = _customerAccountService.Deposit(ACCOUNT_NUMBER_BOB, depositAmountForBob);
            decimal bankTotalBalance = _customerAccountService.GetBankTotalBalance();

            // assert
            transactionResultAlice.CurrentBalance.Should().Be(depositAmountForAlice);
            transactionResultBob.CurrentBalance.Should().Be(depositAmountForBob);
            bankTotalBalance.Should().Be(banksTotalBalance);
        }

        [Theory]
        [InlineData(0.01, 0.01, 0.01, 0.03)]
        [InlineData(0.01, 0.01, 0.02, 0.04)]
        [InlineData(0.01, 0.02, 0.02, 0.05)]
        [InlineData(0.02, 0.02, 0.02, 0.06)]
        [InlineData(0.01, 0.01, 1.00, 1.02)]
        [InlineData(0.01, 1.00, 1.00, 2.01)]
        [InlineData(1.00, 1.00, 1.00, 3.00)]
        public void WHEN_Deposits_Are_Made_By_Three_Customers_THEN_Customer_Balances_Should_Equal_Deposits_AND_Bank_Total_Balance_Should_Equal_Sum_Of_All_Customer_Balances(
            decimal depositAmountForAlice,
            decimal depositAmountForBob,
            decimal depositAmountForSatoshi,
            decimal banksTotalBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_BOB);
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_SATOSHI);

            // act
            TransactionResult transactionResultAlice = _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, depositAmountForAlice);
            TransactionResult transactionResultBob = _customerAccountService.Deposit(ACCOUNT_NUMBER_BOB, depositAmountForBob);
            TransactionResult transactionResultSatoshi = _customerAccountService.Deposit(ACCOUNT_NUMBER_SATOSHI, depositAmountForSatoshi);
            decimal bankTotalBalance = _customerAccountService.GetBankTotalBalance();

            // assert
            transactionResultAlice.CurrentBalance.Should().Be(depositAmountForAlice);
            transactionResultBob.CurrentBalance.Should().Be(depositAmountForBob);
            transactionResultSatoshi.CurrentBalance.Should().Be(depositAmountForSatoshi);
            bankTotalBalance.Should().Be(banksTotalBalance);
        }

        #endregion

        #region WITHDRAWALS FROM MULTIPLE ACCOUNTS

        [Theory]
        [InlineData(0.01, 0.01, 0.01, 0.01, 0.00)]
        [InlineData(0.02, 0.01, 0.02, 0.01, 0.00)]
        [InlineData(0.03, 0.01, 0.02, 0.01, 0.01)]
        [InlineData(1.01, 1.01, 1.01, 1.01, 0.00)]
        [InlineData(1.02, 1.01, 1.02, 1.01, 0.00)]
        [InlineData(1.03, 1.01, 1.02, 1.01, 0.01)]
        [InlineData(1.00, 1.00, 1.00, 1.00, 0.00)]
        [InlineData(2.00, 1.00, 2.00, 1.00, 0.00)]
        [InlineData(3.00, 1.00, 2.00, 1.00, 1.00)]
        public void WHEN_Withdrawals_Are_Made_By_Two_Customers_THEN_Customer_Balances_Should_Drop_By_Withdrawal_Amount_AND_Bank_Total_Balance_Should_Equal_Sum_Of_All_Customer_Balances(
            decimal startingBalanceForAlice,
            decimal startingBalanceForBob,
            decimal withdrawalAmountForAlice,
            decimal withdrawalAmountForBob,
            decimal banksTotalBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_BOB);

            _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, startingBalanceForAlice);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_BOB, startingBalanceForBob);

            // act
            TransactionResult transactionResultAlice = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, withdrawalAmountForAlice);
            TransactionResult transactionResultBob = _customerAccountService.Withdraw(ACCOUNT_NUMBER_BOB, withdrawalAmountForBob);
            decimal bankTotalBalance = _customerAccountService.GetBankTotalBalance();

            // assert
            transactionResultAlice.CurrentBalance.Should().Be(startingBalanceForAlice - withdrawalAmountForAlice);
            transactionResultBob.CurrentBalance.Should().Be(startingBalanceForBob - withdrawalAmountForBob);
            bankTotalBalance.Should().Be(banksTotalBalance);
        }

        [Theory]
        [InlineData(0.01, 0.01, 0.01, 0.01, 0.01, 0.01, 0.00)]
        [InlineData(0.02, 0.01, 0.01, 0.02, 0.01, 0.01, 0.00)]
        [InlineData(0.03, 0.01, 0.01, 0.02, 0.01, 0.01, 0.01)]
        [InlineData(1.01, 1.01, 1.01, 1.01, 1.01, 1.01, 0.00)]
        [InlineData(1.02, 1.01, 1.01, 1.02, 1.01, 1.01, 0.00)]
        [InlineData(1.03, 1.01, 1.01, 1.02, 1.01, 1.01, 0.01)]
        [InlineData(1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 0.00)]
        [InlineData(2.00, 1.00, 1.00, 2.00, 1.00, 1.00, 0.00)]
        [InlineData(3.00, 1.00, 1.00, 2.00, 1.00, 1.00, 1.00)]
        public void WHEN_Withdrawals_Are_Made_By_Three_Customers_THEN_Customer_Balances_Should_Drop_By_Withdrawal_Amount_AND_Bank_Total_Balance_Should_Equal_Sum_Of_All_Customer_Balances(
            decimal startingBalanceForAlice,
            decimal startingBalanceForBob,
            decimal startingBalanceForSatoshi,
            decimal withdrawalAmountForAlice,
            decimal withdrawalAmountForBob,
            decimal withdrawalAmountForSatoshi,
            decimal banksTotalBalance)
        {
            // arrange
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_ALICE);
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_BOB);
            _customerAccountService.CreateCustomerAccount(ACCOUNT_NUMBER_SATOSHI);

            _customerAccountService.Deposit(ACCOUNT_NUMBER_ALICE, startingBalanceForAlice);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_BOB, startingBalanceForBob);
            _customerAccountService.Deposit(ACCOUNT_NUMBER_SATOSHI, startingBalanceForSatoshi);

            // act
            TransactionResult transactionResultAlice = _customerAccountService.Withdraw(ACCOUNT_NUMBER_ALICE, withdrawalAmountForAlice);
            TransactionResult transactionResultBob = _customerAccountService.Withdraw(ACCOUNT_NUMBER_BOB, withdrawalAmountForBob);
            TransactionResult transactionResultSatoshi = _customerAccountService.Withdraw(ACCOUNT_NUMBER_SATOSHI, withdrawalAmountForSatoshi);
            decimal bankTotalBalance = _customerAccountService.GetBankTotalBalance();

            // assert
            transactionResultAlice.CurrentBalance.Should().Be(startingBalanceForAlice - withdrawalAmountForAlice);
            transactionResultBob.CurrentBalance.Should().Be(startingBalanceForBob - withdrawalAmountForBob);
            transactionResultSatoshi.CurrentBalance.Should().Be(startingBalanceForSatoshi - withdrawalAmountForSatoshi);
            bankTotalBalance.Should().Be(banksTotalBalance);
        }

        #endregion
    }
}