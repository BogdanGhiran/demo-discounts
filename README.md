# Introduction 
The project is a .NET API Project with a hub located at /discountHub, it has capabilities for generating codes, generating codes and replying with the code list(for testing purposes) and using codes(one time-only)

# Getting Started

1.	Installation process - make sure to have .net 8 runtime installed before running the project
2.	Software dependencies - uses a postgresql database hosted at localhost:5432, called demo-discounts-db with a user=defaultUser and pass=defaultPassword

# Build and Test
Open the project in Visual Studio(2022) and hit run on the https profile. 
By default, it tries to use the connection string located in appsettings:ConnectionStrings:DemoDiscountsDb
If that is not available, or the database is inaccessible, an in-memory repository is used for debug purposes. This does not have persistance so it should not be relied upon


# Deployment
Every time the main branch receives an update(commit) the [Azure Pipeline](https://dev.azure.com/demo-org-bg/demo-discounts/_build?definitionId=1) will pick up the changes, rebuild and test the service, and if that is successful, it will deploy the new service version at [demo-discounts.gab16.com](https://demo-discounts.gab16.com/status)