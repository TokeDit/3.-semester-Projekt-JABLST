# 3.Semester Projekt - Sikkerhedssystem

Dette er et simplet sikkerhedssystem, som registre bevægelse og sender billedet til brugern.
---

## Table of Contents

- [About](#about)
- [Built With](#built-with)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Deployment](#deployment)
- [Testing](#testing)
- [Contributing](#contributing)
- [Contributors](#contributors)
- [License](#license)

---

## About

<!-- A more detailed explanation of the project. What problem does it solve? What is the context? -->
Projektet har til formål at udvikle et simpelt, men effektivt, smart sikkerhedssystem ved hjælp af en Raspberry Pi. Systemet registrerer bevægelse, tager et billede og sender det øjeblikkeligt til brugeren. Den langsigtede vision er at understøtte ansigtsgenkendelse for at kunne skelne mellem husstandsmedlemmer og ukendte personer samt at gemme de optagne billeder sikkert i skyen.
Målet er et pålideligt system med lav vedligeholdelse, der øger hjemme sikkerheden uden behov for konstant overvågning.

---

## System Architecture

```mermaid
graph TB
    subgraph Client["Client Layer"]
        Browser["Browser\nVue 3 SPA (served from ASP.NET wwwroot)"]
    end

    subgraph Firebase["Firebase (Google Cloud)"]
        FirebaseAuth["Firebase Auth\nsecurity-system-login"]
    end

    subgraph Pi["Raspberry Pi"]
        PiCam["PiCamera2\n1280×720"]
        TFLite["TFLite Model\nperson detection"]
        PyScript["Updated_motion_detection.py\nmotion + upload + heartbeat"]
        PiCam --> TFLite
        TFLite --> PyScript
    end

    subgraph Telegram["Telegram"]
        TelegramBot["Telegram Bot API"]
        TelegramUser["User\n(mobile app)"]
        TelegramUser <--> TelegramBot
    end

    subgraph API["ASP.NET Core REST API (Azure)"]
        AuthCtrl["AuthController\nGET /api/auth/me"]
        PICtrl["PIController\nPOST /api/PI\nPOST /api/PI/heartbeat\nGET /api/PI/status"]
        SikkerCtrl["SikkerController\nGET|POST /Sikker/*"]
        ImageCtrl["ImageController\nGET /api/Image/*"]
        TelegramCtrl["TelegramController\nPOST /telegram/update\nGET /telegram/status"]
        TelegramBotCtrl["TelegramBotController\nPOST /api/TelegramBot/webhook|message|link"]
        AdminReportsCtrl["AdminReportsController\nGET /api/admin/reports/ping"]

        TelegramSvc["TelegramService"]
        TelegramBotSvc["TelegramBotService\nsend alerts/reports"]
        TelegramCmdHandler["TelegramCommandHandler\n/on /off /status /ping /help"]
        ReportSvc["ReportService (Background)\nhourly check, sends scheduled reports"]

        SikkerRepo["SikkerRepo\nimages/users/messages + system state"]
        AppDbContext["AppDbContext\nEF Core"]
    end

    subgraph DB["SQL Server (Azure)"]
        ImagesTable["Images"]
        UsersTable["Users\n(OwnerUid, TelegramChatId,\nReportFrequency, ReportEnabled)"]
        TelegramMessagesTable["TelegramMessages"]
    end

    Browser -->|"signIn/register"| FirebaseAuth
    FirebaseAuth -->|"ID Token"| Browser
    Browser -->|"Bearer token\nGET /api/auth/me"| AuthCtrl
    Browser -->|"GET /api/Image/user/{uid}\nGET /api/Image/user/{uid}/monthly"| ImageCtrl
    Browser -->|"GET /Sikker/status"| SikkerCtrl
    Browser -->|"GET /telegram/status"| TelegramCtrl
    Browser -->|"GET /api/PI/status"| PICtrl

    PyScript -->|"POST /api/PI (image + metadata)"| PICtrl
    PyScript -->|"POST /api/PI/heartbeat"| PICtrl
    PICtrl --> SikkerRepo
    PICtrl --> TelegramBotSvc

    TelegramBot -->|"POST /telegram/update"| TelegramCtrl
    TelegramBot -->|"POST /api/TelegramBot/webhook"| TelegramBotCtrl
    TelegramCtrl --> TelegramCmdHandler
    TelegramCmdHandler -->|"/on /off /status"| SikkerCtrl
    TelegramCmdHandler --> TelegramSvc
    TelegramBotCtrl --> SikkerRepo
    ReportSvc --> SikkerRepo
    ReportSvc --> TelegramBotSvc
    TelegramSvc -->|"sendMessage"| TelegramBot
    TelegramBotSvc -->|"sendMessage"| TelegramBot

    SikkerRepo --> AppDbContext --> ImagesTable
    AppDbContext --> UsersTable
    AppDbContext --> TelegramMessagesTable
    ImageCtrl --> SikkerRepo
```

---

## Built With

- [Vue.js](https://vuejs.org)
- [Axios](https://axios-http.com)
- [REST API](https://restfulapi.net)
- [Azure Web Apps](https://azure.microsoft.com/en-us/products/app-service/web)
- [Azure Database](https://azure.microsoft.com/en-us/products/azure-sql/database)
- [GitHub Actions](https://github.com/features/actions)
- [GitHub Projects](https://docs.github.com/en/issues/planning-and-tracking-with-projects/learning-about-projects/about-projects)
- [Raspberry Pi](https://www.raspberrypi.com)
- [Postman](https://www.postman.com)
- [Swagger](https://swagger.io)
- [Visual Studio Code](https://code.visualstudio.com)

---

## Getting Started

### Prerequisites

- prerequisit 1: downlaoded Node.js
- prerequisit 2

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/TokeDit/3.-semester-Projekt-JABLST.git
   ```

2. Navigate to the project directory
   ```bash
   cd 3.-semester-Projekt-JABLST
   ```

---

## Usage

<!-- How do you run or use the project? Include examples, screenshots, or code snippets -->
### Local host af projected

1. Etabler en local DB med følgene sruktur:

benyt nederstående SQL qurie til at etablere din egen Database.

```SQL
-- Create Users table
CREATE TABLE Users (
    OwnerUid VARCHAR(128) NOT NULL PRIMARY KEY,
    TelegramChatId VARCHAR(128) NULL,
    ReportFrequency INT NOT NULL DEFAULT 7,
    ReportEnabled BIT NOT NULL DEFAULT 1
);

-- Create Images table
CREATE TABLE Images (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    TimeStamp VARCHAR(50) NOT NULL,
    ImageType VARCHAR(50) NOT NULL,
    ImageData VARBINARY(MAX) NOT NULL,
    Description VARCHAR(500) NULL,
    Confidence REAL NULL DEFAULT 0.0,
    DetectedObject VARCHAR(200) NULL,
    OwnerUid VARCHAR(128) NULL,
    
    -- Foreign key constraint to Users table
    CONSTRAINT FK_Images_Users_OwnerUid 
        FOREIGN KEY (OwnerUid) 
        REFERENCES Users(OwnerUid)
        ON DELETE SET NULL
);

-- Create index on OwnerUid for efficient querying by user
CREATE INDEX IX_Images_OwnerUid ON Images(OwnerUid);

-- Create index on TimeStamp for efficient querying by date
CREATE INDEX IX_Images_TimeStamp ON Images(TimeStamp);
```
2. Set din appsettings eller user secrets up:

Billedet viser at den chekker efter en key i Apsettings eller user secrets som heder DbCornnectionLokal, hvis det er et development envirement (localhost)
<img width="2217" height="477" alt="image" src="https://github.com/user-attachments/assets/37cada7a-9880-477c-9371-53ae6f628044" />

Du skal tilføje dette til dine user secrets eller appsettings.json.
```json
"ConnectionStrings": {
  "DbConnectionLocal": "Din lokale connection string her"
}
```
3. Kør back end
    1. cd in i back end projectet:
    ```bash
    cd Rest_SikkerApi/Rest_SikkerApi
    ```
    2. kør Not net projekt
   ```bash
    dotnet run
    ```

4. Kør Front end

    1. cd in i front end projectet:
    ```bash
    cd FrontEnd/SikkerhedsFrontEnd
    ```
    2. kør Node.js
   ```bash
    npm run dev
    ```
   

## Deployment

<!-- How is the project deployed? Describe the CI/CD pipeline, environments, and any special steps -->

This application is deployed as an Azure Web App via a CI/CD pipeline powered by GitHub Actions.
All changes must be submitted through a pull request targeting the `main` branch. Direct pushes
to `main` are restricted. Pull requests require review approval and must pass all required status
checks before merging. Upon merge, the pipeline automatically builds and deploys the application
to Azure.

---


## Testing

The project contains both backend unit tests and frontend UI tests.
The purpose of the tests is to verify important logic and user flows before changes are merged into the `main` branch.

### xUnit Tests

Backend tests are written with **xUnit**. These tests focus on isolated logic in the API and model layer.

Examples of what is tested:

- Image model behavior, including Base64 conversion between `byte[]` and string
- Controller logic and expected HTTP responses
- Service behavior through interfaces and dependency injection
- Edge cases such as empty data, invalid input, and failed operations

To run the xUnit tests locally, use:

```bash
dotnet test
```

To generate a code coverage report locally, use:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

This generates a `coverage.cobertura.xml` file inside the `TestResults` folder.

### Selenium UI Tests

Frontend UI tests are written with **Selenium WebDriver**. These tests verify that important user flows work in the browser.

Examples of what is tested:

- Login page loads correctly
- Required UI elements are visible, such as email input, password input, and login button
- User interaction with input fields and buttons
- Basic login flow behavior and validation

Before running Selenium tests locally, make sure the frontend is running:

```bash
npm run dev
```

Then run the Selenium tests from the frontend test project/folder:

```bash
npm test
```

Selenium tests require Google Chrome and a compatible ChromeDriver/browser driver setup.

---

## Contributing

<!-- How can others contribute? Link to a CONTRIBUTING.md if you have one -->

Contributions are welcome. Please follow the branching strategy outlined above and ensure all
code has been tested locally before opening a pull request. For significant changes, consider
opening an issue first to discuss the proposed approach.

---

## Contributors

- [Jonas Lolk Knudsen](https://github.com/Jonaslk727)
- [Anders Hornbøll Godsk Rasmussen](https://github.com/Andershgras)
- [Toke Hønning Ditlevsen](https://github.com/TokeDit)
- [Hafiz Muhammad Bilal Sarwar](https://github.com/bilalsarwar2907)
- [Stefan Ansbjerg Selchou Hansen](https://github.com/HumongusLaser)
- [Lars Jørgen Vedelslund](https://github.com/Omniform)

---

## License

This project is licensed under the [MIT License](LICENSE).

---

*For infrastructure or deployment issues, refer to the Azure Portal or contact the project maintainer.*
