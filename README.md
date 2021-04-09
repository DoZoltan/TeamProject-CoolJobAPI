# Project name: CoolJob API

## The project is still under development so maybe some of the features is not available.

## Project description
#### This is a ASP.Net WEB API server for our job advertisement web application.
#### It contains the business logic layer and the database.
#### You can find the frontend part here: https://github.com/DoZoltan/TeamProject-CoolJobReact

## Features and aims of the whole project (frontend + backend)
#### In this web application the users can:
- Register, modify and delete the own registration
- Add new advertisements, modify and delete the own advertisement
- Find and filter the advertisements
- Add advertisements to favorites, and remove from it
- Apply for a job

## Used technologies
- .Net C#
- ASP.Net core
- Microsoft Entity Framework Core
- Microsoft SQL server

## How to launch while it is not deployed
1. Download the project
2. Open it with an IDE (for example with Visual Studio 2019)
3. Generate the database by the migration file (you can find it in the Migrations folder)
   - With Visual Studio 2019 you can do this with the following steps:
     - Open the Package Manager Console (Tools --> NuGet Package Manager)
     - Type Update-Database, and press Enter
4. Start the application with F5
5. Open and start the frontend project (you can find the steps of it at its repo)
