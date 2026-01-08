@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:menu
cls
echo ========================================
echo       批量 Git 仓库拉取工具
echo ========================================
echo 1. 使用预设列表拉取
echo 2. 扫描当前目录拉取
echo 3. 编辑预设列表
echo 0. 退出
echo ========================================
set /p choice=请选择操作 (0-3): 

if "%choice%"=="1" goto preset_list
if "%choice%"=="2" goto scan_repos
if "%choice%"=="3" goto edit_list
if "%choice%"=="0" goto end
echo 无效选择，请重新输入！
pause
goto menu

:preset_list
cls
echo ========================================
echo       使用预设列表拉取
echo ========================================
echo.

REM 保存脚本所在目录
set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM 设置仓库列表，格式：仓库路径或URL|分支名（分支名可选）
REM 支持本地路径和远程URL两种方式
set "repos[0]=https://jihulab.com/aimisw/mc-api.git|"
set "repos[1]=https://jihulab.com/aimisw/pto-jdp.git|main"
set "repos[2]=https://jihulab.com/aimisw/mcm.git|"

REM 自动计算仓库数量
set repo_count=0
:count_repos
if defined repos[!repo_count!] (
    set /a repo_count+=1
    goto count_repos
)

if !repo_count! EQU 0 (
    echo 预设列表为空，请先配置仓库列表！
    pause
    goto menu
)

echo 找到 !repo_count! 个预设仓库
echo.
goto process_repos

:scan_repos
cls
echo ========================================
echo       扫描当前目录拉取
echo ========================================
echo.
echo 当前目录: %CD%
echo.
echo 正在扫描 Git 仓库...
set repo_count=0

REM 扫描子目录中的 Git 仓库
for /d %%d in (*) do (
    if exist "%%d\.git" (
        set "repos[!repo_count!]=%%~fd|"
        echo 找到: %%d
        set /a repo_count+=1
    )
)
echo.
echo 共找到 !repo_count! 个 Git 仓库
echo.

if !repo_count! EQU 0 (
    echo 未找到任何 Git 仓库！
    pause
    goto menu
)

set /p confirm=是否继续拉取？(Y/N): 
if /i not "%confirm%"=="Y" goto menu

goto process_repos

:edit_list
cls
echo ========================================
echo       预设列表配置说明
echo ========================================
echo.
echo 请编辑脚本文件中的以下部分:
echo.
echo 支持本地路径:
echo set "repos[0]=C:\Projects\project1|"
echo.
echo 支持远程URL:
echo set "repos[1]=https://github.com/user/repo.git|main"
echo.
echo 格式说明:
echo - 路径或URL和分支用竖线分隔
echo - 分支留空会使用最近提交的分支
echo - URL会自动克隆到当前目录
echo.
pause
goto menu

:process_repos
echo.
echo 开始处理 !repo_count! 个仓库...
echo ========================================
echo.

set success_count=0
set fail_count=0
set skip_count=0

for /l %%i in (0,1,!repo_count!) do (
    set "current_repo=!repos[%%i]!"
    if defined current_repo (
        call :process_single_repo %%i
    )
)

echo ========================================
echo 处理完成
echo   OK: !success_count!
echo   FAIL: !fail_count!  
echo   SKIP: !skip_count!
echo ========================================
echo.

pause
goto menu

:process_single_repo
set idx=%1
set /a idx_display=idx+1
set "current_repo=!repos[%idx%]!"

REM 分割路径和分支
for /f "tokens=1* delims=|" %%a in ("!current_repo!") do (
    set "repo_input=%%a"
    set "branch=%%b"
)

REM 判断是URL还是本地路径
echo !repo_input! | findstr /i "http https git@" >nul
if !errorlevel! EQU 0 (
    REM 是远程URL
    REM 从URL提取仓库名
    for /f "tokens=* delims=/" %%f in ("!repo_input!") do set "temp=%%f"
    for %%f in ("!repo_input!") do set "repo_name=%%~nxf"
    set "repo_name=!repo_name:.git=!"
    set "repo_path=%SCRIPT_DIR%!repo_name!"
    
    echo [%idx_display%/!repo_count!] 处理: !repo_name!
    
    REM 检查本地是否已存在
    if exist "!repo_path!\.git" (
        echo   本地仓库已存在
        cd /d "!repo_path!"
    ) else (
        echo   克隆仓库到本地...
        cd /d "%SCRIPT_DIR%"
        git clone "!repo_input!" "!repo_name!"
        if errorlevel 1 (
            echo   克隆失败！
            set /a fail_count+=1
            goto end_process_single
        )
        cd /d "!repo_path!"
        echo   克隆成功
    )
) else (
    REM 是本地路径
    set "repo_path=!repo_input!"
    echo [%idx_display%/!repo_count!] 处理: !repo_path!
    
    if not exist "!repo_path!\.git" (
        echo   错误: 不是有效的 Git 仓库
        set /a fail_count+=1
        goto end_process_single
    )
    cd /d "!repo_path!"
)

REM 如果没有指定分支，获取远程最近修改的分支
if "!branch!"=="" (
    echo   检测远程最近修改的分支...
    
    REM 先更新远程分支信息
    git fetch --all 2>nul
    
    REM 获取远程最近修改的分支
    set "branch_found="
    for /f "delims=" %%x in ('git for-each-ref --sort^=-committerdate --format^="%%(refname:short)" refs/remotes/origin/ 2^>nul') do (
        if not defined branch_found (
            REM 提取分支名，移除 origin/ 前缀
            set "temp_branch=%%x"
            set "branch=!temp_branch:origin/=!"
            REM 排除 HEAD
            if not "!branch!"=="HEAD" (
                set "branch_found=1"
            ) else (
                set "branch="
            )
        )
    )
    
    if "!branch!"=="" (
        REM 如果还是没找到，使用当前分支
        for /f "delims=" %%x in ('git branch --show-current 2^>nul') do set "branch=%%x"
    )
)

if "!branch!"=="" (
    echo   错误: 无法确定分支
    set /a fail_count+=1
    goto end_process_single
)

echo   分支: !branch!

REM 获取当前分支
for /f "delims=" %%x in ('git branch --show-current 2^>nul') do set "current_branch=%%x"

REM 如果不在目标分支，先切换
if not "!current_branch!"=="!branch!" (
    echo   切换到分支 !branch!...
    git checkout !branch! 2>nul
    if errorlevel 1 (
        echo   切换分支失败！
        set /a fail_count+=1
        goto end_process_single
    )
)

REM 检查是否有未提交的更改
git diff --quiet 2>nul
if errorlevel 1 (
    echo   警告: 存在未提交的更改，跳过拉取
    set /a skip_count+=1
    goto end_process_single
)

REM 执行 pull
echo   正在拉取更新...
git pull 2>nul
if errorlevel 1 (
    echo   拉取失败！
    set /a fail_count+=1
) else (
    echo   拉取成功
    set /a success_count+=1
)

:end_process_single
echo.
exit /b

:end
echo 感谢使用，再见！
endlocal
exit /b