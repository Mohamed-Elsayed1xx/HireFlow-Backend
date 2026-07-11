FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/HireFlow.Domain/HireFlow.Domain.csproj HireFlow.Domain/
COPY src/HireFlow.Application/HireFlow.Application.csproj HireFlow.Application/
COPY src/HireFlow.Infrastructure/HireFlow.Infrastructure.csproj HireFlow.Infrastructure/
COPY src/HireFlow.API/HireFlow.API.csproj HireFlow.API/
RUN dotnet restore HireFlow.API/HireFlow.API.csproj

COPY src/ .
RUN dotnet publish HireFlow.API/HireFlow.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet HireFlow.API.dll --urls http://0.0.0.0:${PORT}"]
