# Setup

1. Update the connection string for DatabaseContext in Program.cs, line 10
1. If running on a fresh system, you'll want to run 'update-database' from the Package Manager Console. [More Details](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs)
1. Data hydration
1. Build and run the Api project
1. Try out the AddEmployee endpoint on the Swagger page
1. Use the json payload at the end of this readme and submit your request
1. Stop the Api project
1. From powershell, navigate to the Api project folder and run the following command to launch the Api:
   `dotnet run .\Api.csproj --launch-profile TestingApi`
1. From Visual Studio, navigate to the Test Explorer and run all tests

# Choices

- MSSQLServer - I chose this because Lance mentioned it was the relational database technology used at Paylocity (local server is running at 2019)
- EFCore - I've used a few ORM solutions over the years. This one seemed appropriate for the exercise and I have recent familiarity with. Version 7 installed by default.
- Migrations - If this had been schema first, I probably would have gone with EFCorePowerTools. It has a very nice reverse engineer feature. Since this was modeled already, I went with Entity Framework Migrations
  - If running on a fresh system, you'll want to run 'update-database' from the Package Manager Console
- Api Endpoints/Actions
  - It looks like we were using a DTO pattern so I replicated that, including wrapping the responses in a standard response object
  - For the sake of brevity, I didn't implement fully RESTful controllers. Once I added batch add endpoints, I moved on
  - For 4xx error scenarios, I was surfacing id's as part of the response messaging. I stopped doing that since I wasn't sure if that was desirable for this project. We may have wanted some security through obscurity protections. If it was desirable, I'd probably revisit id handling in custom exceptions and response message for batch endpoints
- Data service layer
  - I added a service layer between the controller and the dbcontext. This provided insulation between our data models and controller dto's and a place for business logic
  - I added some custom exceptions so the controller could catch them explicitly and provide the appropriate response codes and response messaging under these conditions
  - I added some duplication of business logic in the DependentService.AddDependentsAsync method. I got a little side-tracked trying to enforce the single partner requirement at the database level. It works, but handling that gracefully wasn't pretty. I added some explicit code for handling that at the service level, but kept the conditional exception catch for demonstrative purposes
    - I did not implement similar safeguards in the EmployeeService.AddEmployeesAsync method. Mostly for the sake of brevity, but it did have me consider a middleware solution for general exception handling
- Unit Testing / Paycheck Calculation
  - I put compensation and deductions as separate calculation methods in a PaycheckService. It's more of a helper class right now so I made it a static method instead of injecting it.
  - I have some notes in the PaycheckService describing my desire for solutioning the use of magic numbers in that class. We should probably have some flexibility for regional benefit cost variance
  - I went with simple compensation and deduction values for the response. I wasn't sure if I should be providing more details for each line item.
  - Unit tests handle mostly reasonable inputs for those methods, but could probably use more edge case analysis
  - I also included a unit test for the age calculation method which services the deduction cost calculation, but disabled it because of its reliance on Datetime.Now.

# tasks

- //task: update your port if necessary
  - It wasn't necessary, but I did add another profile so I could have an instance running in powershell while I ran unit test
- //task: make test pass
  - All tests pass

# Data hydration

- I performed a GetAllEmployees call with the original code to get a json payload of the data
- Once I implemented my AddEmployees endpoint, I used the json payload to hydrate the database
- json payload follows:
  [
  {
  "id": 1,
  "firstName": "LeBron",
  "lastName": "James",
  "salary": 75420.99,
  "dateOfBirth": "1984-12-30T00:00:00",
  "dependents": []
  },
  {
  "id": 2,
  "firstName": "Ja",
  "lastName": "Morant",
  "salary": 92365.22,
  "dateOfBirth": "1999-08-10T00:00:00",
  "dependents": [
  {
  "id": 1,
  "firstName": "Spouse",
  "lastName": "Morant",
  "dateOfBirth": "1998-03-03T00:00:00",
  "relationship": 1
  },
  {
  "id": 2,
  "firstName": "Child1",
  "lastName": "Morant",
  "dateOfBirth": "2020-06-23T00:00:00",
  "relationship": 3
  },
  {
  "id": 3,
  "firstName": "Child2",
  "lastName": "Morant",
  "dateOfBirth": "2021-05-18T00:00:00",
  "relationship": 3
  }
  ]
  },
  {
  "id": 3,
  "firstName": "Michael",
  "lastName": "Jordan",
  "salary": 143211.12,
  "dateOfBirth": "1963-02-17T00:00:00",
  "dependents": [
  {
  "id": 4,
  "firstName": "DP",
  "lastName": "Jordan",
  "dateOfBirth": "1974-01-02T00:00:00",
  "relationship": 2
  }
  ]
  }
  ]
