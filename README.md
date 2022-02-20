# CashApp-SimpleBank
CASH CODING EXERCISE V5

## Language
This coding exercise was implemented in C#, running in Dot Net Core 6.

## Running
Provided below are three options for running the code, depending on your requirements and local machine setup.

The easiest way to build, run, and crucially debug this solution, is to install Microsoft VisualStudio (not VisualStudio Code), and run it in the IDE.

It may be necessary to install the .Net Core 6 SDK on your machine (if not already), to get the solution running in Visual Studio.

To open the solution, select the ``CashApp-SimpleBank.sln`` file in VisualStudio.

Alternatively, if debugging the code isn't a concern, and you just wish to run the unit tests to confirm they pass, there are two alternate options: 
- a command-line option (only requiring the installation of the .Net Core SDK), or...
- a Docker option (requiring the installation of Docker Desktop), with instructions detailed below.

### Running Options

#### Visual Studio
Within Visual Studio, the solution can be debugged as a .NetCore app, by running or debugging the unit tests from the Test Explorer (this can be found in the 'View' menu).

You would need to download the Community version of Visual Studio, from the [Visual Studio website](https://visualstudio.microsoft.com/downloads/).

#### Command Line
The command line option is probably the most straight-forward method of running the unit tests, and viewing the pass/fail outcome.

If you haven't already, install the [.Net Core 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) from Microsoft.

Once installed, using your favourite console / terminal application, navigate to the root folder of where the solution has been copied to (the folder where the ``CashApp-SimpleBank.sln`` file is located), and type: ``dotnet test "./SimpleBank.Application.Tests/SimpleBank.Application.Tests.csproj"``, and hit Enter.

Exact syntax may very, depending on your machine (Windows / Mac / Linux). The above syntax is for a Windows machine.

This should build and run the unit test project, and output the test results to the console.

#### Docker
Alternatively, if you already have Docker Desktop installed and don't want to bother with installing VisualStudio or the .Net Core 6 SDK, there is the option of running the unit tests inside a docker container, by following these instructions:

1. Make sure you have Docker Desktop running, as mentioned previously.

2. Open your terminal / command-line of choice, and navigate to the solution's root directory (where the ``CashApp-SimpleBank.sln`` file is located).

3. Type ``docker build -t cashapp-simplebank-image -f SimpleBank.Application.Tests\Dockerfile .`` and hit Enter (syntax may vary, depending on your machine - this is for Windows).

4. The image should now appear in Docker Desktop, alongside your other images.

5. In Docker Desktop, selected the newly created image ``cashapp-simplebank-image``, and click 'Run'.

6. Still in Docker Desktop, if you nabigate to the 'Containers / Apps' menu item, you should be able to find the container running.

7. Selecting the container should open the 'Logs' tab for the now running ``cashapp-simplebank-image``.

8. With any luck, the unit tests have all passed.

9. (Optional) If you wish to create an image file to copy to another machine, type ``docker save -o cashapp-simplebank-image.tar cashapp-simplebank-image``, to create a *.tar file. The *.tar file should appear in the solution's root directory.

## Analysis and development approach

### Analysis - Initial Observations, Thoughts, and Questions:
1. In asking for balances, do we return the customer's and bank's balance in the same transaction?
- Decided to separate as discreet methods.

2. Should we return the customer's and bank's balance after depositing and withdrawing?
- Customer shouldn't be able to view the bank's total balance. Return only customer's balance.

3. How to store the ledger of accounts?
- A simple dictionary should suffice to replicate a database context. It would also eliminate the need for mocked/substituted resources in the unit tests. This can be wrapped inside a 'ledger repository'.

4. Requirements don't specify account creation.
- We'll need a method to create new accounts, so that the unit tests can interact with multiple accounts. Unit tests can pre-load account data straight into the ledger repository.

5. In terms of validation and exception handling... 
    
    a. What happens when incorrect input is entered?
    - First thought is we would throw an exception, however since we're already down in the 'business layer', likely that input validation would have already occurred. Additionally, the methods of our service are strongly typed, so providing invalid input (such as a string or characters instead of a decimal) is not possible.  

    b. Is there anything exceptional about attempting to overdraw an account?
    - No. That's a business rule. Nothing has actually gone wrong with the attempted transaction, and the requirement to halt the transaction from proceeding is a business decision, so we just wish to prevent an overdraw from occurring. Therefore no need to throw an exception.

    c. Is there anything exceptional about attempting a transaction on an unkown account number?
    - We can't transact on an account that doesn't exist, so yes, that's a circumstance where the application cannot perform what it has been asked to do. Throw an exception to the caller.

    d. Do we need to validate transactions with a zero amount, or negative amount for that matter?
    - Probably for zero amounts, in the real world, that would save an extra call to the database. There's no point to a zero amount transaction.
    - Definitely for negative amounts. Can't have negative withdrawals artifically inflating a user's account.

6. Do we logically separate (different classes) the fetching of the bank's total balance, from that of the individual customer account actions, into a distinct service?
- Initially I thought yes, as it sounds like something you would want a logical separation of (assuming there would be more functioanlity at the bank-wide level). However on second thought, it would be overkill for these purposes, as fetching the bank's total balance is something that would likely leverage the actual database employed (an SQL Sum method, for example). Creating an additional service just for that one method which contains only retrieval logic (nothing calculated in code) is therefore not warranted.

7. The provided example test scenario describes the expected behaviour of multiple interactions under the one unit test. 
- I would draw a distinction here with this being a more complex test, separate from a straight unit tests (testing one method only), and so it warrants separation in the test project, into its own distinct test class.


### Order Of development
1. Started the README.md to commence jotting down these thoughts.
2. Created the 'SimpleBank.Application' project, to represent the application (business logic) layer. Normally this would sit behind a server such as a web-api project.
3. Added the interface for interacting with the customer's account - ICustomerAccountService.
4. Created the 'TransactionResult' class and 'TransactionOutcomeEnum' to represent the result of a transaction.
5. Defined the methods for ICustomerAccountService, all returning 'TransactionResult'.
6. Created a service class to inherit ICustomerAccountService, and implemented the methods with default responses that throw 'NotImplementedException' (temporary).
7. Created the test project 'SimpleBank.Application.Tests'.
8. Created 'CustomerAccountServiceBehaviourTests' and 'CustomerAccountServiceUnitTests'.
9. Began populating tests.
10. Created a repository interface 'ILedgerRepository' to represent interaction with the data persistence layer for the ledger of accounts.
11. Created repository class 'LedgerRepository' to inherit 'ILedgerRepository', and implemented the methods with default responses that throw 'NotImplementedException' (temproary).
12. Start populating LedgerRepository.
13. Created custom exception for when an account does not exist.
14. Commenced populating 'CustomerAccountService'.
15. Tidied up the unit tests.
16. Created the Dockerfile (within the SimpleBank.Application.Tests project), as an option for running the tests.
17. Filled out the rest of the README.md


## CashApp-SimpleBank Solution
The solution is divided into three projects - two concerned with the application itself, and the third is the testing project.

The projects are: 

1. SimpleBank.Application

2. SimpleBank.Data

3. SimpleBank.Application.Tests


### SimpleBank.Application
The SimpleBank.Application project exists as the core domain of the 'SimpleBank' business logic, and houses the main service, models, interface, and custom exception, needed to process input from the test class.

Ordinarily, if this solution were running in a server environment (for example), the SimpleBank.Application project would be referenced by an API (or other presentation) layer in a 'ports and connector' type pattern.

The main entry-point into the SimpleBank.Application functionality is through the CustomerAccountService. 

The CustomerAccountService itself has a single dependency, the 'ILedgerRepository' - an interface to represent the data layer (more on the ILedgerRepository below). In production code, the passing of this interface to the CustomerAccountService would ordinarily be handled via dependency injection, through the dependency resolver of the running API project. However in this case, the unit test project is setting this class up and passing it in at runtime.

The CustomerAccountService implements the main methods specified in the requirements, namely the abilities to:
- Deposit.
- Withdraw.
- Get Customer Account Balance.
- Get the Bank's total balance.
- Create a new customer account (not specified by the requirements, but implied).

Depending on the requirements of the method being invoked, various validations and checks are employed. A combination of error-preventing checks and business rules are performed, some explicitly mentioned in the requirements, while others are implicitly required to ensure proper running of the solution, namely:
- Validate an account exists (implicit) - the solution can't perform certain actions if the account doesn't exist.
    - Deposit
    - Withdraw
    - GetBalance
- Validate an account has sufficient funds to withdraw (explcitly required).
    - Withdraw
- Validate the transaction amount is greater than zero (implicit) - this seemed like an appropriate check to employ. While a negative deposit could overdraw an account, more worryingly, a negative withdrawal could create funds that never existed, posing a real risk for the SimpleBank operators. It's also a way to eliminate any unnecessary database calls, if a zero amount was entered.
    - Deposit
    - Withdraw

These checks fall into two categories - error validation checks, and business logic checks. Each type warrants a different response from the system.

1. Validating if an account exists falls into the category of an error validation check, as effectively the system cannot perform the task it was asked to do if that crucial piece of information is not present. Therefore it warrants throwing an exception back to the caller.

2. On the other hand, validating sufficent funds for a withdrawal is a business logic check. There are account types (in the real world) that can overdraw on the basis of extending a line of credit. In this instance, the requirements decided this account is a pure debit account (if one were to find a real-world analogue), and not have an overdraw facility. Therefore there's nothing exceptional about attempting to overdraw the account, the CustomerAccountService can just prevent the user from doing so, and pass enough information in the TransactionResult model for the presentation layer to represent the outcome accordingly.

3. Validating for zero or negative amounts could be considered a sub-category of a business logic check, as it's performing a basic data cleansing check to prevent the operators of the bank (the business) from potentially getting into serious trouble. It's a practical, common-sense check to perform. Similar to preventing a transaction from overdrawing an account, it's sufficient to block the transaction from proceeding, and return the TransactionResult model to the caller, with appropriate information as to why the transaction did not proceed.

The distinction between an error validation and a business logic validation can often be subtle. Essentially it comes down to what can be considered as something the system cannot be expected to handle, vs. what kind of situations we want to prevent (or put guardrails around) the system handling.


### SimpleBank.Data
As the name implies, the SimpleBank.Data project is the stand-in for a data layer. The requirement specified that there would be no data persistence, therefore this implementation of the data layer only holds on to data during runtime, or in this case, within the running of a single unit test.

Ordinarily this layer would be populated with 'repository' classes to interact with the various tables that the solution would be required to persist data to, along with numerous data models that would represent the data contained in those tables.

In this case however, the requirements were straightforward enough that it was only necessary to store an integer as a stand-in for an account number, and a decimal as the current account balance for each customer. This eliminated the need to store complex data models to represent a customer's account.

Where normally there would be a database context, instead the database itself is represented by a simple Dictionary, with the account number (integer) as the lookup key, and value is the current account balance (decimal).

The 'LedgerRepository' (the implementation of ILedgerRepository, mentioned previously), contains the logic for persisting all the data. It performs no validations on data flowing in or out. It is expected tha the 'appropriateness' of a transaction is to be determined in the business layer. Any failure that occurs at the data layer should be database / persistence related. 


### SimpleBank.Application.Tests
SimpleBank.Application.Tests is where we can find the unit tests for the 'SimpleBank.Application' project.

Within, there are two unit test files, 'CustomerAccountServiceBehaviourTests.cs' and 'CustomerAccountServiceUnitTests.cs'.

Given the requirements explicitly required the inclusion of a complex behaviour driven test, it seemed natural to draw a distinction betweeen more complex tests involving numerous interactions in a single test that assert multiple conditions, vs. simpler, fine-grained unit tests that attempt to assert a single method's behaviour.

Approaching it this way - broader multiple interaction tests, AND targetted unit tests, helps answer the question "what is our application doing at the level of the smallest unit, and does it all come together in a cohesive approach?"

#### CustomerAccountServiceBehaviourTests
CustomerAccountServiceBehaviourTests runs the more complex tests, involving numerous interactions, be they multiple deposit and withdrawal scenarios on the same account, or across different accounts.

#### CustomerAccountServiceUnitTests
CustomerAccountServiceUnitTests runs targetted, fine-grained unit tests, designed to only test the behaviour and response of a single method. Although the tests here may rely on numerous methods to setup the test scenario, they are only concerned with the specific output of a single method.

#### On how in-memory data influenced the testing approach
In many ways, the choice to not persist data made the testing approach much simpler.

Ordinarily in a real-world application, the data layer (ILedgerRepository, in this case) would have been represented by .Net Core's Entity Framework database context (or similar). Therefore when approaching unit tests, 'ILedgerRepository' would potentially have had to have been substituted with a mock implementation via an external mocking library. This would have meant that every interaction with the data layer would have required a mocked response, which would have blown out the size of test project.

By using in-memory data only, it was possible to side-step that approach, and use actual data for the running of the unit tests. 


## Assumptions, Trade-offs & Additional Notes
1. For the purposes of keeping this exercise simpler, authentication and authorization were assumed to not be required, therefore each request is assumed to be valid for the consumer.

2. No logging was implemented, as there was no specific requirement.

3. As there was no data persistence required, I thought the use of in-memory data made the most sense. This streamlined the testing approach, as we could use actual data, as opposed to a mocked data store and stubbed responses from that data store.

4. I didn't devote any time to defensive coding of the data store, i.e. wrapping data store logic in try/catch blocks, as it would have been overkill for the in-memory data-store that was implemented. In short, the defensive code would have been defending against a scenario that would never happen. I would have had to mock that un-happy path in unit tests, and in effect it would not be a real test of the code implemented, and therefore not provide any additional value.

5. *If* an exception were to occur at the data store layer, the 'CustomerAccountService' would have no way of catching it, and so that exception would flow on up to the caller, which is obviously not a situation we would want. To resolve this, I would throw a custom exception from the data layer, catch that exception in the CustomerAccountService, log the error, then throw a more generic exception so that no stack trace makes it out of the system to the caller.

6. If there were additional validation requirements - be they error handling, business logic, or data integrity, I would have separated out the validation logic from the 'CustomerAccountService', into its own dedicated class.

7. In creating the method to create new customer accounts, I have taken the slimmest approach possible, in requiring the account number to be passed in. In an actual scenario, I imagine the database or perhaps some other mechanism, would determine the next appropriate customer identifier to be used, and that would be returned to the caller upon creation.


## Production - Things to consider
I would consider several options for a production environment:

1. Ordinarily I would include a pre-commit hook in an exercise such as this, to run the unit tests before attempting to commit, to prevent erroneos code from making its way into the repository. However as there were instructions to not share the solution via Git repository, I skipped this inclusion. It should go without saying that in a production setting, it would be necessary to run those build checks and run the unit tests in a proper deployment pipeline, as a gate-keeper for deployment to any environment. 

2. My assumption is that this solution would sit behind an API server in a production environment. Given the light-weight nature of this solution (in particular, no database with no Entity Framework models to deal with), I would consider an AWS Lambda (or Azure Functions equiavalent) sitting behind an API Gateway, to run it in production.

3. I would liked to have included logging of incoming requests, to provide increased visibilty in case of errors that arise.

4. Defensive coding to catch failures at the datastore level, so that stack-traces do not flow up to the calling client.