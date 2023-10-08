# Dissertation Interface Backend
This is a [.NET Core](https://learn.microsoft.com/en-us/dotnet/core/introduction) application with version [.NET7](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-7).

## Getting Started

### Requirements
-   [ Docker and Docker Compose](https://www.docker.com/products/docker-desktop/)

### Clone the Project
- `git clone https://github.com/Olaheavy2021/dissertation_interface`

### Setup Https Dotnet Certificate

#### Remove your existing local development self-signed certificate.

- `dotnet dev-certs https --clean`

#### Create a new local development self-signed certificate (note that this is PowerShell syntax for getting the user profile path, modify as needed for other shells) and set a password
- `dotnet dev-certs https -ep $env:USERPROFILE/.aspnet/https/aspnetapp.pfx -p <password>`
- `dotnet dev-certs https --trust`
- `dir ~/.aspnet/https`

### Docker Setup
#### Setup Environment Variables
- Make a copy of the following and set the appropriate credentials in the files. Please note that the Certificate Password should be same as set above :
    - .env.notification.sample to .env.notification
    - .env.dissertation.sample to .env.dissertation
    - .env.database.sample to .env.database
  
#### Create a network for the application
    - docker network create dissertation_app_network

#### Start up application and build images

    -   docker compose up -d --build

#### Start the application

    -   docker compose up

#### Stop the application

    -   docker compose stop

#### Stops and removes all the containers

    -   docker compose down

### Linting and Formatting the Codebase
    -   dotnet format
### Running Database Migrations

#### Install Dotnet EF Tools
  -   ```dotnet tool install --global dotnet-ef```

#### Add Migration
  - ```dotnet ef migrations  add <migration-name>```

#### Update Database
  -  ```dotnet ef database update```

### Unit tests
#### Install the dotnet coverage tool
  - ``` dotnet tool install --global dotnet-coverage ```
#### Install the report generator tool
 - ``` dotnet tool install --global dotnet-reportgenerator-globaltool ```
####  Execute the tests and collect code coverage into a file
 - ```dotnet coverage collect dotnet test --output .\Tests\CodeCoverage --output-format cobertura --settings CodeCoverage.runsettings ```
#### Generate the report
- ``` reportgenerator -reports:.\Tests\CodeCoverage  -targetdir:".\Tests\CoverageReport" -reporttypes:Html  ```

