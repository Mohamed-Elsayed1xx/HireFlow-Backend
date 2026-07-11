# Multi-stage build. Railway's automatic build detection (Railpack) couldn't
# figure out how to build this on its own — the API project isn't at the repo
# root and depends on three sibling projects (Domain/Application/Infrastructure)
# via project references, which auto-detection tools generally don't follow.
# This Dockerfile is explicit about it instead.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy just the .csproj files first and restore — this layer only
# invalidates (and re-downloads NuGet packages) when a .csproj actually
# changes, not on every code edit, so rebuilds are much faster.
COPY src/HireFlow.Domain/HireFlow.Domain.csproj HireFlow.Domain/
COPY src/HireFlow.Application/HireFlow.Application.csproj HireFlow.Application/
COPY src/HireFlow.Infrastructure/HireFlow.Infrastructure.csproj HireFlow.Infrastructure/
COPY src/HireFlow.API/HireFlow.API.csproj HireFlow.API/
RUN dotnet restore HireFlow.API/HireFlow.API.csproj

# Now copy everything else and publish.
COPY src/ .
RUN dotnet publish HireFlow.API/HireFlow.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
# Railway assigns a container port at runtime via $PORT — the app has to
# bind to whatever that is, not a fixed port. Defaulting to 8080 covers
# running this image somewhere that doesn't set $PORT (e.g. testing it
# locally with `docker run`).
ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet HireFlow.API.dll --urls http://0.0.0.0:${PORT}"]
