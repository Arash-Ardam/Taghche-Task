#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Book-Api/Book-Api.csproj", "Book-Api/"]
COPY ["Book-Application/Book-Application.csproj", "Book-Application/"]
COPY ["Book-Infra/Book-Infra.csproj", "Book-Infra/"]
COPY ["Book-Domain/Book-Domain.csproj", "Book-Domain/"]
RUN dotnet restore "./Book-Api/./Book-Api.csproj"
COPY . .
WORKDIR "/src/Book-Api"
RUN dotnet build "./Book-Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Book-Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Book-Api.dll"]