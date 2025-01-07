# Introduction 
The project is a .NET API Project with a hub located at /discountHub, it has capabilities for generating codes, generating codes and replying with the code list(for testing purposes) and using codes(one time-only)

### Backend available at:
https://demo-discounts.gab16.com/status
### Frontend available at:
https://demo-discounts-client.gab16.com

# Getting Started

1.	Installation process - make sure to have .net 8 runtime installed before running the project
2.	Software dependencies - uses a postgresql database hosted at localhost:5432, called demo-discounts-db with a user=defaultUser and pass=defaultPassword

# Build and Debug
Open the project in Visual Studio(2022) and hit run on the https profile. The debug process should be as simple as that.
By default, it tries to use the connection string located in appsettings:ConnectionStrings:DemoDiscountsDb

If the connectionString is not available, or the database is inaccessible, an in-file repository is used for debug purposes. 

This does have persistance but it is IO bound and locks the file. Multiple requests can be started at once, but will wait for access using a locking mechanism. I should be considered the fallback mechanism, but full functionality is expected.(the hosted version has a database)

# Tests
There is a test project called demo-discounts.Tests which tests the discount code service for validations, and the repository for functionality. 

These tests run as part of the CI/CD pipeline every time a new commit hits the main branch. 

# Deployment
Every time the main branch receives an update(commit) the [Azure Pipeline](https://dev.azure.com/demo-org-bg/demo-discounts/_build?definitionId=1) will pick up the changes, rebuild and test the service, and if that is successful, it will deploy the new service version at [demo-discounts.gab16.com](https://demo-discounts.gab16.com/status)