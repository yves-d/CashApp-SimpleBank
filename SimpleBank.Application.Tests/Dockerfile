#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["SimpleBank.Application.Tests/SimpleBank.Application.Tests.csproj", "SimpleBank.Application.Tests/"]
COPY ["SimpleBank.Application/SimpleBank.Application.csproj", "SimpleBank.Application/"]
COPY ["SimpleBank.Data/SimpleBank.Data.csproj", "SimpleBank.Data/"]
RUN dotnet restore "SimpleBank.Application.Tests/SimpleBank.Application.Tests.csproj"

COPY . .
RUN dotnet publish -c Release -o out

# run tests on docker run
ENTRYPOINT ["dotnet", "test"]