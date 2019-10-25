
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["Demo.Core.Api.WebApi/Demo.Core.Api.WebApi.csproj", "Demo.Core.Api.WebApi/"]
COPY ["Demo.Core.Api.Core/Demo.Core.Api.Core.csproj", "Demo.Core.Api.Core/"]
COPY ["Demo.Core.Api.Model/Demo.Core.Api.Model.csproj", "Demo.Core.Api.Model/"]
COPY ["Demo.Core.Api.Data/Demo.Core.Api.Data.csproj", "Demo.Core.Api.Data/"]
COPY ["Demo.Core.Api.IService/Demo.Core.Api.IService.csproj", "Demo.Core.Api.IService/"]
RUN dotnet restore "Demo.Core.Api.WebApi/Demo.Core.Api.WebApi.csproj"
COPY . .
WORKDIR "/src/Demo.Core.Api.WebApi"
RUN dotnet build "Demo.Core.Api.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Core.Api.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Core.Api.WebApi.dll"]