# FinancialKDS - Financial Decision Support System

FinancialKDS is a web-based decision support system developed to perform feasibility analysis for investment projects. The system analyzes historical cash flow data entered by the user using the linear regression method, forecasts future cash flows, and supports investment decisions through Net Present Value (NPV) calculation.

## Purpose of the Project

The main purpose of this project is to digitalize the financial evaluation process of investment projects, reduce manual calculation errors, and provide users with fast, understandable, and graph-supported feasibility reports.

With this system, users can:

* Create a new investment project.
* Enter the initial investment cost, discount rate, and forecast period information of the project.
* Save historical cash flow data.
* Forecast future cash flows using linear regression.
* Determine whether the investment is financially acceptable through NPV calculation.
* Analyze results visually with charts.
* Export the report as a PDF.

## Technologies Used

* C#
* ASP.NET Core MVC
* Entity Framework Core
* Microsoft SQL Server LocalDB
* HTML5
* CSS3
* Bootstrap
* JavaScript
* Chart.js
* Visual Studio 2022

## Project Features

### Project Management

Users can create new investment projects, edit existing projects, and delete projects within the system. For each project, information such as project name, description, initial investment cost, discount rate, and the number of forecast periods is stored.

### Cash Flow Management

Historical cash flow data can be entered for each investment project. The system preserves data integrity by preventing duplicate period entries for the same project.

### Forecasting with Linear Regression

The entered historical cash flow data is analyzed using the linear regression method. The system calculates regression coefficients and generates estimated cash flows for future periods.

### NPV Calculation

The forecasted future cash flows are discounted to their present value using the discount rate specified by the user. The Net Present Value is calculated by subtracting the initial investment cost from the total present value.

Based on the NPV result, the system produces the following decisions:

* If NPV > 0, the investment is financially acceptable.
* If NPV < 0, the investment is not financially suitable.
* If NPV = 0, the investment is at the break-even point.

### Graphical Reporting

Analysis results are presented to the user through tables and charts. Historical and forecasted cash flows are visualized comparatively using Chart.js.

### PDF / Printable Report

Analysis results can be reported in PDF format using the browser’s print feature. This output can be used as an investment feasibility report.

## Project Structure

```text
FinancialKDS
│
├── Controllers
│   ├── ProjectsController.cs
│   └── CashFlowsController.cs
│
├── Data
│   └── ApplicationDbContext.cs
│
├── Models
│   ├── Project.cs
│   ├── CashFlow.cs
│   ├── ForecastResult.cs
│   └── AnalysisResult.cs
│
├── Services
│   ├── RegressionService.cs
│   └── NpvService.cs
│
├── Views
│   ├── Projects
│   ├── CashFlows
│   └── Shared
│
├── wwwroot
├── Program.cs
└── appsettings.json
```

## Database Structure

The project uses the Entity Framework Core Code First approach. The main tables are:

* Projects
* CashFlows
* ForecastResults
* AnalysisResults

## Installation

To run the project, follow the steps below:

1. Clone the repository to your computer.

```bash
git clone https://github.com/your-username/FinancialKDS.git
```

2. Navigate to the project folder.

```bash
cd FinancialKDS
```

3. Open the `FinancialKDS.sln` file with Visual Studio 2022.

4. Make sure the NuGet packages are installed.

Main packages used:

```text
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
```

5. To create the database, run the following command in the Package Manager Console:

```powershell
Update-Database
```

6. Run the project.

```text
https
```

or click the green run button in Visual Studio.

## Sample Usage Scenario

For a sample investment project, the following information can be entered:

```text
Project Name: E-Commerce Warehouse Automation Investment
Initial Investment Cost: 750000
Discount Rate: 20
Forecast Period: 5
```

Historical cash flows:

```text
Period 1: 160000
Period 2: 210000
Period 3: 260000
Period 4: 320000
```

Based on this data, the system forecasts future cash flows, calculates the NPV value, and presents the investment decision to the user.

## Developer

İrem Nisa Sözen
Software Engineering
Manisa Celal Bayar University
