@echo off

:: 切换为UTF-8编码
chcp 65001 >nul  

setlocal enabledelayedexpansion

:: 当前目录
cd /d "%~dp0"

:: 配置区域
set "SOLUTION_PATH=../src/CZJ.Extension.sln"
set "PROJECT_PATH=../src/CZJ.Extension/CZJ.Extension.csproj"

echo ========================================
echo 支持多目标框架构建和发布脚本
echo ========================================
echo.

:: 还原项目
echo [步骤 1/4] 正在还原项目...
echo ========================================
dotnet restore "%SOLUTION_PATH%"
if errorlevel 1 (
    echo [错误] 还原失败！
    pause
    exit /b 1
)
echo [成功] 还原完成
echo.

:: 编译项目（针对所有目标框架）
echo [步骤 2/4] 正在编译项目（所有目标框架）...
echo ========================================
dotnet build "%PROJECT_PATH%" -c Release
if errorlevel 1 (
    echo [错误] 编译失败！
    pause
    exit /b 1
)
echo [成功] 编译完成
echo.

:: 自动读取目标框架
echo [步骤 3/4] 正在读取目标框架...
echo ========================================

set "FRAMEWORKS="
for /f "tokens=*" %%i in ('dotnet msbuild "%PROJECT_PATH%" -getProperty:TargetFrameworks -nologo') do (
    set "FRAMEWORKS=%%i"
)

if "%FRAMEWORKS%"=="" (
    echo [警告] 未检测到多目标框架，尝试读取单一目标框架...
    for /f "tokens=*" %%i in ('dotnet msbuild "%PROJECT_PATH%" -getProperty:TargetFramework -nologo') do (
        set "FRAMEWORKS=%%i"
    )
)

if "%FRAMEWORKS%"=="" (
    echo [错误] 无法读取目标框架！
    pause
    exit /b 1
)

:: 将分号替换为空格（处理多目标框架）
set "FRAMEWORKS=%FRAMEWORKS:;= %"

echo [信息] 检测到目标框架: %FRAMEWORKS%
echo.

:: 发布项目（针对每个目标框架）
echo [步骤 4/4] 正在发布项目...
echo ========================================

for %%f in (%FRAMEWORKS%) do (
    echo.
    echo 发布目标框架: %%f
    echo ----------------------------------------
    dotnet publish "%PROJECT_PATH%" -c Release -f %%f --no-build
    
    if errorlevel 1 (
        echo [失败] %%f 发布失败
    ) else (
        echo [成功] %%f 发布成功
    )
)

echo.
echo ========================================
echo [完成] 所有操作已完成
echo ========================================
echo.

pause
exit /b 0