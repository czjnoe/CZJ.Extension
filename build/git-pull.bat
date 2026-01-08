@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo     批量 Git 仓库拉取工具
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
REM 扫描子目录（一级）
REM -------------------------------
for /d %%d in (*) do (
    if exist "%%d\.git" (
        set /a repo_count+=1
        call :process_repo "%%~fd"
    )
)

echo.
echo ========================================
echo 拉取完成
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

REM 确认是 Git 仓库（双保险）
git rev-parse --is-inside-work-tree >nul 2>&1
if errorlevel 1 (
    echo   错误: 不是有效的 Git 仓库
    set /a skip_count+=1
    goto :eof
)

REM 检查是否有未提交的更改
git diff --quiet
if errorlevel 1 (
    echo   警告: 存在未提交的更改，已跳过
    set /a skip_count+=1
    goto :eof
)

REM 获取当前分支
for /f "delims=" %%b in ('git branch --show-current 2^>nul') do set "branch=%%b"

if "!branch!"=="" (
    echo   错误: 无法确定当前分支
    set /a fail_count+=1
    goto :eof
)

echo   分支: !branch!

REM 执行 pull
echo   正在拉取更新...
git pull
if errorlevel 1 (
    echo   拉取失败（可能有冲突）
    set /a fail_count+=1
) else (
    echo   拉取成功
    set /a success_count+=1
)

goto :eof
