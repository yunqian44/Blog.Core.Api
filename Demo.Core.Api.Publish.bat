color B

echo "*************开始删除旧文件***************"
del  PublishFiles\*.*   /s /q

echo "*************开始还原工程***************"
dotnet restore

echo "*************开始编译工程***************"
dotnet build

cd Demo.Core.Api.WebApi

echo "*************开始发布工程***************"
dotnet publish -o ..\Demo.Core.Api.WebApi\bin\Debug\netcoreapp2.2\

md ..\PublishFiles

xcopy ..\Demo.Core.Api.WebApi\bin\Debug\netcoreapp2.2\*.* ..\PublishFiles\ /s /e 

echo "Successfully!!!! ^ please see the file PublishFiles"

cmd