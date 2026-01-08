@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo     批量 Git 仓库推送工具
echo ========================================
echo.

set success_count=0
set fail_count=0
set skip_count=0
set repo_count=0

REM -------------------------------
REM 处理当前目录
REM -------------------------------
if exist ".git" (
    set /a repo_count+=1
    call :process_repo "%CD%"
)

REM -------------------------------
REM 扫描子目录
REM -------------------------------
for /d %%d in (*) do (
    if exist "%%d\.git" (
        set /a repo_count+=1
        call :process_repo "%%~fd"
    )
)

echo.
echo ========================================
echo 推送完成
echo   总仓库: !repo_count!
echo   成功:   !success_count!
echo   失败:   !fail_count!
echo   跳过:   !skip_count!
echo ========================================
echo.

pause
exit /b


REM ==================================================
REM 处理单个仓库
REM ==================================================
:process_repo
set "repo_path=%~1"
set /a current_index=success_count+fail_count+skip_count+1

echo.
echo [%current_index%] 处理仓库: %repo_path%
cd /d "%repo_path%"

REM 检查是否有未提交的更改
git diff --quiet
if errorlevel 1 (
    echo   警告: 存在未提交的更改，已跳过
    set /a skip_count+=1
    goto :eof
)

REM 检查是否有待推送提交
git status -sb | findstr /c:"ahead" >nul
if errorlevel 1 (
    echo   无需推送（没有新提交）
    set /a skip_count+=1
    goto :eof
)

REM 执行 push
echo   正在推送...
git push
if errorlevel 1 (
    echo   推送失败！
    set /a fail_count+=1
) else (
    echo   推送成功
    set /a success_count+=1
)

goto :eof
