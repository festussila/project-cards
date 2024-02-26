# ProjectCards

## Introduction 
ProjectCards is a RESTful WebAPI that allows users to create and manage tasks in the form of cards.

## Getting Started
1. Editor
Use either VS Code or Visual Studio 2022(latest release) to contribute.

2.	Dependencies
 1. Target Framework - .NET 8.0.1
 2. Runtime - .NET Core 8.0.1
 3. SDK - .NET 8 Version 8.0.101
 4. Latest(stable) Nuget Packages 

## Structure
The project leverages C# Language, MS SQL Server for database. The project uses Code-First approach to define data models.

The project is structured logically into 3 parts:-
	1. Domain - Entities and DB Scaffolding
	2. Management - Business Logic
	3. Services - RESTful WebAPI to expose the endpoints

## Documentation
The API is documented using Swagger based on OpenAPI specification. To access the documentation run the API and access {{host_address}}/swagger.
Further, an OpenAPI V3 specification file is generated that can be imported in other tools that support the specification, such as, Postman and access the documentation.

## Solution Items
There 1 file under docs folder: **Error Codes**  Markdown file which enumerates all codes prefixed with **CA**.
E.g. *Error Code 1 shall be abbreviated as ***CA001***

*appsettings.Development.json* is specific to each contributor with configuration matching their private dev box.
