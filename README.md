# Netcorext.Grpc.Mediator
Mediator Pattern for gRPC

## Example for ASP.NET 6
### Step 1
Add package reference
```
dotnet add package Netcorext.Grpc.Mediator
```
### Step 2
Configure in Program.cs
```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediator()
                .AddPerformancePipeline()
                .AddValidatorPipeline();
```