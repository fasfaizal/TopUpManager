## Top Up Manager

### Enhancements

- `Authorization` can be implemented, and we don't need to send the user's details in every request.
- `Logging` can be improved by using some external logging services, rather than using console logging.
- Usage of real services and functions in real world scenario. Currenly a mock implementation is added for `FinanceService` service(external service) and `DoTopUp` function(credit top up to phone).
- Tools like `AutoMapper` can be used to remove fileds which are not required in the response.

### Setup

To run the TopupManager API, ensure you .NET Core SDK(Version 8) installed on your machine.

### Running the Application using command line

- Clone the repository.
- Navigate to the project root directory.
- Configure the database connection string under the section `ConnectionStrings` in `appsettings.json`
- The following items are also made configurable, the values can be chaged in `appsettings.json`

```js
  "Configurations": {
        "MaxBeneficiaryCount": 5,
        "TopUpOptions": [ 5, 10, 20, 30, 50, 75, 100 ],
        "MonthlyLimitForUser": 3000,
        "VerifiedUserMonthlyBeneficiaryLimit": 500,
        "UnverifiedUserMonthlyBeneficiaryLimit": 1000,
        "TransactionCharge": 1,
        "FinanceServiceBaseUrl": "http://localhost:5002"
}
```

- Restore the dependencies using `dotnet restore`.
- Run the ef migrations by running the command `dotnet ef database update --startup-project TopUpManager.API --project TopUpManager.DataAccess`.
- Run the Extenal Http Service(debit/credit) by running `dotnet run --project FinanceService.API --urls=http://localhost:5002/` in a new terminal.
- Run the application using `dotnet run --project TopUpManager.API --urls=http://localhost:5001/`, in another terminal.
- The app will be available at `http://localhost:5001`.
- Open `http://localhost:5001/swagger/index.html` to view the swagger API documentation.
