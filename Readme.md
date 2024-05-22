# OrcShackApi

OrcShackApi is a REST API for managing dishes and users. It's built with .NET 8.0.

## Features

### Task 1 

- **Dish Management:** API users can create, view, list, update, and delete dishes. Dishes have a name, description, price, and image.
- **Customer Interactions:** Customers can search, view, and rate dishes.

### Task 2  
- **User Management:** Users can register and login. All functionality of the API requires a logged in user (except Registration).
- **Authentication:** The system supports password-based authentication.
- **Validation:** Data entities in the API have validation.
- **Security:** The API includes functionality to defend against brute force password attacks.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- Docker
- .NET 8.0 SDK

### Installing

1. Clone the repository
2. Navigate to the project directory
3. Update the default connection string with your SQL Server connection string
4. Run the migrations using the .NET EF command: `dotnet ef database update`
5. Build the Docker image: `docker build -t orcshackapi .`
6. Run the Docker container: `docker run -p 8080:8080 orcshackapi`

## Built With

- .NET 8.0
- Docker
- SQL Server 
- Fluent Validations
- Moq
- Nunit

## Authors

- Abhijit Achare
 