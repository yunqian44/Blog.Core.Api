FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /blogcore

# Copy csproj and restore as distinct layers
COPY ["Demo.Core.Api.WebApi/Demo.Core.Api.WebApi.csproj", "Demo.Core.Api.WebApi/"]
COPY ["Demo.Core.Api.Core/Demo.Core.Api.Core.csproj", "Demo.Core.Api.Core/"]
COPY ["Demo.Core.Api.Model/Demo.Core.Api.Model.csproj", "Demo.Core.Api.Model/"]
COPY ["Demo.Core.Api.Data/Demo.Core.Api.Data.csproj", "Demo.Core.Api.Data/"]
COPY ["Demo.Core.Api.IService/Demo.Core.Api.IService.csproj", "Demo.Core.Api.IService/"]
RUN dotnet restore "Demo.Core.Api.WebApi/Demo.Core.Api.WebApi.csproj"
COPY . .
RUN dotnet build "Blog.Core.csproj" -c Release -o out

# Copy everything else and build
#COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /blogcore
COPY --from=build-env /blogcore/out .
ENTRYPOINT ["dotnet", "demo.core.api.webapi.dll"]