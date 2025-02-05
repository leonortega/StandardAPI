# StandardAPI

## Goals
This solution composed of a REST API and a website in Angular, is the result of an experiment where:
- As a developer. use different AI (Gemini, Claude, ChatGPT, etc) to create the applications and without entering a line of code.
- Apply best practices (Clean Architecture, DDD, SOLID, etc) and add as many libraries as possible (listed below).
- You get applications that work, they are not the most optimal or with the best code.

## Overview
- StandardAPI is a robust and scalable API solution built using modern technologies to ensure high performance, reliability, and maintainability. This document provides an overview of the technologies used in this solution and the rationale behind their selection.
- DemoWebsite is a simple website that consumes the StandardAPI to demonstrate how to interact with the API using a web application.

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

### Running with Docker

1. **Install Docker**: Ensure Docker is installed and running on your machine.

2. **Start the Containers**: In the docker-compose file folder, use Docker Compose to start the containers.

```
docker-compose up
```

3. **Access the Application**: The application will be available at `http://localhost:5000` and the Swagger UI at `http://localhost:5000/swagger`.

4. **Access the Site**: The site will be available at `http://localhost:5002`

### Running with Visual Studio

1. **Open the Solution**: Open the `StandardAPI.sln` solution file in Visual Studio.

2. **Set Startup project**: Set `docker-compose` as the startup project.

3. **Run the Application**: Press `F5` to build and run the application.

4. **Access the Application**: The application will be available at `http://localhost:5000` and the Swagger UI at `http://localhost:5000/swagger`.

5. **Access the Site**: The site will be available at `http://localhost:5002`

## Conclusion

It is possible to create any application just using the code provided by the AIs, and as a programmer, define the guidelines and the functionality of the code. The quality of the code and the time invested in this process could have been optimized by taking into account better practices in the writing of prompts, although some concepts were learned with the use of each AI. Final note: All the resulting code is subject to improvement and some implementations have better ways to be solved, e.g. dependency injections.

