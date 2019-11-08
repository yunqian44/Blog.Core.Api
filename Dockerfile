# 这是第一个指令，必须是FROM这里指定基础构建镜像
FROM microsoft/dotnet:2.2-sdk AS build-env  
#工作目录，即程序运行根目录
WORKDIR /blogcore

# Copy csproj and restore as distinct layers
COPY ./*.sln .
COPY ./Demo.Core.Api.Core/*.csproj ./Demo.Core.Api.Core/
COPY ./Demo.Core.Api.Data/*.csproj ./Demo.Core.Api.Data/
COPY ./Demo.Core.Api.IRepository/*.csproj ./Demo.Core.Api.IRepository/
COPY ./Demo.Core.Api.IServices/*.csproj ./Demo.Core.Api.IServices/
COPY ./Demo.Core.Api.Model/*.csproj ./Demo.Core.Api.Model/
COPY ./Demo.Core.Api.Repository/*.csproj ./Demo.Core.Api.Repository/
COPY ./Demo.Core.Api.Services/*.csproj ./Demo.Core.Api.Services/
COPY ./Demo.Core.Api.WebApi/*.csproj ./Demo.Core.Api.WebApi/

RUN dotnet restore ./Demo.Core.Api.WebApi/Demo.Core.Api.WebApi.csproj --disable-parallel

# Copy everything else and build
COPY . ./
RUN dotnet publish ./Demo.Core.Api.WebApi/Demo.Core.Api.WebApi.csproj  -c Release  -o /blogcore
# COPY ./Demo.Core.Api.Services/ dest
ENTRYPOINT ["dotnet", "Demo.Core.Api.WebApi.dll"]