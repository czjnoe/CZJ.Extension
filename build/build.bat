@echo off
 
:: 当前目录
cd /d "%~dp0"
 
:: 编译项目
dotnet build ../src/CZJ.Extension.sln
 
cmd
 
 