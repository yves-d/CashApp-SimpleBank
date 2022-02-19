# CashApp-SimpleBank
CASH CODING EXERCISE V5

## Language
This coding exercise was implemented in C#, running in Dot Net Core.

## Running
The easiest way to build, run, and debug this solution is to install Microsoft VisualStudio (not VisualStudio Code), and run it in the IDE.

It may be necessary to install .Net Core on your machine (if not already), to get the solution running in Visual Studio.

To open the solution, select the ``CashApp-SimpleBank.sln`` file in VisualStudio.

Alternatively, if you just wish to run the unit tests to confirm they pass, there is a Docker option, with instructions detailed below.


### Running Options
Within Visual Studio, the solution can be debugged as a .NetCore app, by running the unit tests from the Test Explorer (this can be found in the 'View' menu).

#### Docker (Optional)
If you just wish to run the unit tests without installing VisualStudio, this can be accomplished by running the unit tests inside a docker container, by following these instructions:

1. Make sure you have Docker Desktop running, as mentioned previously.

2. Open your terminal / command-line of choice, and navigate to the solution's root directory (where the ``CashApp-SimpleBank.sln`` file is located).

3. Type ``docker build -t cashapp-simplebank-image -f SimpleBank.Application.Tests\Dockerfile .`` and hit enter. 

4. The image should now appear in Docker Desktop, alongside your other images.

5. In Docker Desktop, selected the newly created image ``cashapp-simplebank-image``, and click 'Run'.

6. Still in Docker Desktop, if you nabigate to the 'Containers / Apps' menu item, you should be able to find the container running.

7. Selecting the container should open the 'Logs' tab for the now running ``cashapp-simplebank-image``.

8. With any luck, the unit tests have all passed.

9. (Optional) If you wish to create an image file to copy to another machine, type ``docker save -o cashapp-simplebank-image.tar cashapp-simplebank-image``, to create a *.tar file. The *.tar file should appear in the solution's root directory.



# Initial Observations, Thoughts, and Questions:
1. In asking for balances, do we return the customer's and bank's balance in the same transaction?
- decided to separate as discreet methods.

2. Should we return the customer's and bank's balance after depositing and withdrawing?
- Customer shouldn't be able to view the bank's total balance. Return only customer's balance.

3. How to store the ledger of accounts?
- a simple dictionary should suffice to replicate a database context. It would also eliminate the need for mocked/substituted resources in the unit tests. This can be wrapped inside a 'ledger repository'.

4. Requirements don't specify account creation.
- We'll need a method to create new accounts, so that the unit tests can interact with multiple accounts. Unit tests can pre-load account data straight into the ledger repository.

5. In terms of validation and exception handling... 
    
    a. What happens when incorrect input is entered?
    - first thought is we would throw an exception, however since we're already down in the 'business layer', likely that input validation would have already occurred. Additionally, the methods of our service are strongly typed, so providing invalid input (such as a string or characters instead of a decimal) is not possible.  

    b. is there anything exceptional about attempting to overdraw an account?
    - no. That's a business rule. Nothing has actually gone wrong with the attempted transaction, we just wish to prevent an overdraw from occurring. Therefore no need to throw an exception.

    c. is there anything exceptional about attempting a transaction on an unkown account number?
    - we can't transact on an account that doesn't exist, so yes. throw an exception to the caller

    d. do we need to validate transactions with a zero amount, or negative amount for that matter?
    - probably for zero amounts, in the real world, that would save an extra call to the db
    - definitely for negative amounts. can't have negative withdrawals.


6. Do we separate the fetching of the bank's total balance from that of the individual customer account actions, into a distinct service?
- Initially I thought yes, as it sounds like something you would want a logical separation of (assuming there would be more functioanlity at the bank-wide level), however on second thought it would be overkill, as fetching the bank's total balance is something that would likely leverage the actual database employed. Creating an additional service just for that one method (with no logical) is therefore not warranted.

7. The provided example test scenario describes the interaction of multiple interactions under the one unit test. 
- I would classify this as a behavioural test, as distinct from straight unit tests (testing one method only), and so it warrants separation in the test project.


# Order Of development
1. Created the 'SimpleBank.Application' project, to represent the application (business logic) layer. Normally this would sit behind a server such as a web-api project.
2. Added the interface for interacting with the customer's account - ICustomerAccountService.
3. Created the 'TransactionResult' class and 'TransactionOutcomeEnum' to represent the result of a transaction.
4. Defined the methods for ICustomerAccountService, all returning 'TransactionResult'.
5. Created a service class to inherit ICustomerAccountService, and implemented the methods with default responses that throw 'NotImplementedException'.
6. Created the test project 'SimpleBank.Application.Tests'.
7. Created 'CustomerAccountServiceBehaviourTests' and 'CustomerAccountServiceUnitTests', with an accompanying 'CustomerAccountServiceTestHarness' to consolidate test setup and execution logic.
8. Began populating tests.
9. Created a repository interface 'ILedgerRepository' to represent interaction with the data persistence layer for the ledger of accounts.
10. Created repository class 'LedgerRepository' to inherit 'ILedgerRepository', and implemented the methods with default responses that throw 'NotImplementedException'.
11. Start populating LedgerRepository.
12. Created custom exception for when an account does not exist.
13. Commenced populating 'CustomerAccountService'.
