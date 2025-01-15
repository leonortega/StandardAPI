# StandardAPI

## Overview

StandardAPI is a robust and scalable API solution built using modern technologies to ensure high performance, reliability, and maintainability. This document provides an overview of the technologies used in this solution and the rationale behind their selection.

## Technologies Used

### Clean Architecture
- **Reason**: Clean Architecture ensures that the solution is maintainable, scalable, and testable by separating concerns into different layers. It promotes a clear separation between the domain, application, and infrastructure layers, making the codebase easier to manage and evolve.

### MediatR
- **Reason**: MediatR is a simple, unambitious mediator implementation in .NET. It is used to implement the mediator pattern, which helps in decoupling the request handling logic from the application logic, promoting a clean and maintainable codebase.

### FluentValidation
- **Reason**: FluentValidation is a popular .NET library for building strongly-typed validation rules. It is used to ensure that the data entering the system is valid, providing a fluent interface for defining validation rules.

### Mapperly
- **Reason**: Mapperly is a high-performance object-to-object mapping library. It is used to map data transfer objects (DTOs) to domain models and vice versa, ensuring that data transformations are efficient and maintainable.

### Docker
- **Reason**: Docker is a platform for developing, shipping, and running applications in containers. It ensures consistency across different environments, simplifies deployment, and improves scalability. Docker Compose is used to manage multi-container applications.

### Docker Compose
- **Reason**: Docker Compose is a tool for defining and running multi-container Docker applications. It allows us to define the services, networks, and volumes required for the application in a single file, making it easy to manage and deploy the entire stack.

### CockroachDB
- **Reason**: CockroachDB is a distributed SQL database designed for high availability, scalability, and strong consistency. It is used as the primary database for the StandardAPI, providing robust data storage and management capabilities.

### Redis
- **Reason**: Redis is an in-memory data structure store used as a database, cache, and message broker. It is used in the StandardAPI for caching and improving the performance of data retrieval operations.

### Serilog
- **Reason**: Serilog is a diagnostic logging library for .NET applications. It provides a simple and flexible way to log structured data, making it easier to monitor and troubleshoot the application.

### FluentMigrator
- **Reason**: FluentMigrator is a migration framework for .NET that allows us to define database migrations in a fluent, readable manner. It is used to manage database schema changes and ensure consistency across different environments.

### HealthChecks.UI
- **Reason**: HealthChecks.UI is a UI tool for visualizing the health status of the application and its dependencies. It provides a user-friendly interface for monitoring the health checks configured in the application.

### Polly
- **Reason**: Polly is a resilience and transient-fault-handling library for .NET. It provides policies such as retries, circuit breakers, and timeouts to improve the reliability and stability of the application.

### Build and Package Props
- **Reason**: Using `Directory.Build.props` and `Directory.Packages.props` files helps in centralizing and managing project-wide settings and dependencies. This ensures consistency across all projects in the solution and simplifies dependency management.

## Running the Application

There are multiple ways to run the StandardAPI application:

### Running Locally

1. **Clone the Repository**: Clone the repository to your local machine.2. **Install Dependencies**: Ensure you have .NET 8 SDK installed on your machine. Restore the dependencies using the following command:3. **Update Configuration**: Update the `appsettings.json` file with the necessary configuration settings, such as connection strings for CockroachDB and Redis.

4. **Run the Application**: Use the following command to run the application:5. **Access the Application**: The application will be available at `https://localhost:5001` and the Swagger UI at `https://localhost:5001/swagger`.

### Running with Docker

1. **Install Docker**: Ensure Docker is installed and running on your machine.

2. **Build Docker Images**: Use Docker Compose to build the images and start the containers.3. **Start the Containers**: Use Docker Compose to start the containers.4. **Access the Application**: The application will be available at `http://localhost:5000` and the Swagger UI at `http://localhost:5000/swagger`.

### Running with Visual Studio

1. **Open the Solution**: Open the `StandardAPI.sln` solution file in Visual Studio.

2. **Set Startup Project**: Set `StandardAPI` as the startup project.

3. **Run the Application**: Press `F5` to build and run the application.

4. **Access the Application**: The application will be available at `https://localhost:5001` and the Swagger UI at `https://localhost:5001/swagger`.
1. **Clone the Repository**: Clone the repository to your local machine.

git clone https://github.com/your-repo/StandardAPI.git
cd StandardAPI

2. **Install Dependencies**: Ensure you have .NET 8 SDK installed on your machine. Restore the dependencies using the following command:

dotnet restore

3. **Update Configuration**: Update the `appsettings.json` file with the necessary configuration settings, such as connection strings for CockroachDB and Redis.

4. **Run the Application**: Use the following command to run the application:

dotnet run --project StandardAPI

5. **Access the Application**: The application will be available at `https://localhost:5001` and the Swagger UI at `https://localhost:5001/swagger`.

### Running with Docker

1. **Install Docker**: Ensure Docker is installed and running on your machine.

2. **Build Docker Images**: Use Docker Compose to build the images and start the containers.
    
docker-compose build

3. **Start the Containers**: Use Docker Compose to start the containers.

docker-compose up

4. **Access the Application**: The application will be available at `http://localhost:5000` and the Swagger UI at `http://localhost:5000/swagger`.

### Running with Visual Studio

1. **Open the Solution**: Open the `StandardAPI.sln` solution file in Visual Studio.

2. **Set Startup Project**: Set `StandardAPI` as the startup project.

3. **Run the Application**: Press `F5` to build and run the application.

4. **Access the Application**: The application will be available at `https://localhost:5001` and the Swagger UI at `https://localhost:5001/swagger`.

## Conclusion

The technologies chosen for the StandardAPI solution are selected based on their performance, reliability, scalability, and ease of use. They provide a solid foundation for building a modern, high-performance API that can handle the demands of today's applications. By leveraging these technologies, we ensure that the StandardAPI is robust, maintainable, and ready for future growth.

