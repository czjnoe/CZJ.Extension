@echo off
setlocal enabledelayedexpansion

:: ====================================
:: NuGet 发布脚本
:: ====================================

:: 配置nuget包目录 - 请根据实际情况修改
set "PACKAGE_DIR=..\"
set "NUGET_SOURCE=https://api.nuget.org/v3/index.json"

:: ====================================
:: 主流程
:: ====================================

echo ========================================
echo NuGet 发布工具
echo ========================================
echo.

:: 检查包目录是否存在
if not exist "%PACKAGE_DIR%" (
    echo [错误] 包目录不存在: %PACKAGE_DIR%
    echo 请先运行 pack.bat 打包项目
    pause
    exit /b 1
)

:: 检查是否有包文件
set "PACKAGE_COUNT=0"
for %%f in ("%PACKAGE_DIR%\*.nupkg") do (
    set /a PACKAGE_COUNT+=1
)

if %PACKAGE_COUNT%==0 (
    echo [错误] 在 %PACKAGE_DIR% 目录中未找到 .nupkg 文件
    echo 请先运行 pack.bat 打包项目
    pause
    exit /b 1
)

:: 列出待发布的包
echo [信息] 找到 %PACKAGE_COUNT% 个包文件:
echo ========================================
for %%f in ("%PACKAGE_DIR%\*.nupkg") do (
    echo   - %%~nxf
)
echo ========================================
echo.

:: 输入 API Key
echo [步骤 1/2] 请输入 NuGet API Key
echo.
echo 提示: 
echo   1. 访问 https://www.nuget.org/account/apikeys 获取 API Key
echo   2. 登录你的 NuGet 账号
echo   3. 创建新的 API Key 或使用现有的
echo.
set /p API_KEY=请输入 API Key: 

if "%API_KEY%"=="" (
    echo.
    echo [错误] API Key 不能为空
    pause
    exit /b 1
)

:: 确认发布
echo.
echo [步骤 2/2] 确认发布
echo ========================================
set /p CONFIRM=确认要发布这些包到 NuGet.org 吗? (Y/N): 

if /i not "%CONFIRM%"=="Y" (
    echo.
    echo [取消] 已取消发布操作
    pause
    exit /b 0
)

:: 发布所有包
echo.
echo [信息] 正在发布包到 NuGet.org...
echo ========================================
echo.

set "SUCCESS_COUNT=0"
set "FAIL_COUNT=0"

for %%f in ("%PACKAGE_DIR%\*.nupkg") do (
    set "FULL_PATH=%%~ff"
    echo 发布: %%~nxf
    echo 路径: !FULL_PATH!
    dotnet nuget push "!FULL_PATH!" --api-key "%API_KEY%" --source "%NUGET_SOURCE%" --skip-duplicate
    
    if errorlevel 1 (
        echo [失败] %%~nxf 发布失败
        set /a FAIL_COUNT+=1
    ) else (
        echo [成功] %%~nxf 发布成功
        set /a SUCCESS_COUNT+=1
    )
    echo.
)

:: 显示发布结果
echo ========================================
echo [完成] 发布流程结束
echo ========================================
echo.
echo 发布统计:
echo   - 成功: %SUCCESS_COUNT% 个
echo   - 失败: %FAIL_COUNT% 个
echo   - 总计: %PACKAGE_COUNT% 个
echo.
echo 说明:
echo   - 包发布后可能需要几分钟才能在 NuGet.org 上搜索到
echo   - 访问 https://www.nuget.org/packages 查看你的包
echo   - 如果提示包已存在，这是正常的（使用了 --skip-duplicate）
echo.

pause
exit /b 0