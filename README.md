# .NET Web API - JonasCodingTest

This project is a simple .NET Web API designed to manage company and employee information. It demonstrates key concepts like asynchronous programming, repository pattern, proper error handling with logging, and separation of concerns across different layers.

## Table of Contents

- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Setup and Installation](#setup-and-installation)
- [Layers Description](#layers-description)
- [Features](#features)
- [API Endpoints](#api-endpoints)
- [Error Handling and Logging](#error-handling-and-logging)

## Project Structure

```plaintext
├── BusinessLayer.Model
├── BusinessLayer
├── DataAccessLayer
├── DataLayer.Model
├── WebApi
├── JonasCodingTest.sln
```

## Technologies Used

- **.NET Framework 4.7.2**
- **Web API**
- **C#**
- **Serilog**

## Prerequisites

- **Visual Studio 2019 or higher**
- **.NET Framework 4.7.2 SDK**

## Setup and Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/baqarmangrani/JonasCodingTest
   cd JonasCodingTest
   ```

2. **Open Solution**

   Open `JonasCodingTest.sln` using Visual Studio.

3. **Build the Solution**

   Ensure all projects compile successfully.

4. **Run the Web API**

   Set `WebApi` as the startup project and run it.

## Layers Description

- **BusinessLayer.Model**: Contains business-specific business models and Interfaces.
- **BusinessLayer**: Contains service classes responsible for business logic, working, Exceptions and mapping data models to business models.
- **DataLayer.Model**: Defines database entities and interfaces for repositories.
- **DataAccessLayer**: Contains InMemoryDatabase and implements repository interfaces to perform CRUD operations on the in-memory database.
- **WebApi**: Hosts the API controllers and manages dependency injection, routing, and configuration.

## Features

- **Implemented Company Controller Functions**: Completed all CRUD operations for the Company controller, ensuring full integration down to the data access layer.
- **Asynchronous Company Controller**: Refactored all Company controller functions to be asynchronous, improving performance and scalability.
- **Employee Repository**: Created a new repository to manage employee information with the following data model properties:
  - `string SiteId`
  - `string CompanyCode`
  - `string EmployeeCode`
  - `string EmployeeName`
  - `string Occupation`
  - `string EmployeeStatus`
  - `string EmailAddress`
  - `string Phone`
  - `DateTime LastModified`
- **Employee Controller**: Developed a new controller to retrieve employee information for the client side with the following properties:
  - `string EmployeeCode`
  - `string EmployeeName`
  - `string CompanyName`
  - `string OccupationName`
  - `string EmployeeStatus`
  - `string EmailAddress`
  - `string PhoneNumber`
  - `string LastModifiedDateTime`
- **Logging and Error Handling**: Integrated a logging framework (Serilog) into the solution and implemented comprehensive error handling to ensure all exceptions are logged and user-friendly responses are returned.

## API Endpoints

### Companies Controller

The `CompaniesController` provides endpoints to manage company information. Below are the available API endpoints:

#### Get All Companies

- **URL**: `/api/companies`
- **Method**: `GET`
- **Description**: Retrieves all companies.
- **Response**: `200 OK` with a list of company DTOs.

#### Get Company by Code

- **URL**: `/api/companies/{companyCode}`
- **Method**: `GET`
- **Description**: Retrieves a company by its code.
- **Response**:
  - `200 OK` with the company DTO.
  - `404 Not Found` if the company does not exist.

#### Add New Company

- **URL**: `/api/companies`
- **Method**: `POST`
- **Description**: Adds a new company.
- **Request Body**: `CompanyDto` object.
- **Response**:
  - `201 Created` with the location of the new company.
  - `400 Bad Request` if the request body is null.
  - `409 Conflict` if there is a conflict while adding the company.

#### Update Company by Code

- **URL**: `/api/companies/{companyCode}`
- **Method**: `PUT`
- **Description**: Updates an existing company by its code.
- **Request Body**: `CompanyDto` object.
- **Response**:
  - `200 OK` with a success message.
  - `400 Bad Request` if the request body is null.
  - `404 Not Found` if the company does not exist.

#### Delete Company by Code

- **URL**: `/api/companies/{companyCode}`
- **Method**: `DELETE`
- **Description**: Deletes a company by its code.
- **Response**:
  - `200 OK` with a success message.
  - `404 Not Found` if the company does not exist.

### Employees Controller

The `EmployeesController` provides endpoints to manage employee information. Below are the available API endpoints:

#### Get All Employees

- **URL**: `/api/employees`
- **Method**: `GET`
- **Description**: Retrieves all employees.
- **Response**: `200 OK` with a list of employee DTOs.

#### Get Employee by Code

- **URL**: `/api/employees/{employeeCode}`
- **Method**: `GET`
- **Description**: Retrieves an employee by their code.
- **Response**:
  - `200 OK` with the employee DTO.
  - `404 Not Found` if the employee does not exist.

#### Add New Employee

- **URL**: `/api/employees`
- **Method**: `POST`
- **Description**: Adds a new employee.
- **Request Body**: `EmployeeDto` object.
- **Response**:
  - `201 Created` with the location of the new employee.
  - `400 Bad Request` if the request body is null.
  - `409 Conflict` if there is a conflict while adding the employee.

#### Update Employee by Code

- **URL**: `/api/employees/{employeeCode}`
- **Method**: `PUT`
- **Description**: Updates an existing employee by their code.
- **Request Body**: `EmployeeDto` object.
- **Response**:
  - `204 No Content` on successful update.
  - `400 Bad Request` if the request body is null.
  - `404 Not Found` if the employee does not exist.

#### Delete Employee by Code

- **URL**: `/api/employees/{employeeCode}`
- **Method**: `DELETE`
- **Description**: Deletes an employee by their code.
- **Response**:
  - `200 OK` with a success message.
  - `404 Not Found` if the employee does not exist.

## Error Handling and Logging

The solution uses a logging framework i.e. Serilog to capture and log errors. Error handling is done centrally, ensuring that every exception is logged, and the API returns a user-friendly response with relevant status codes.
