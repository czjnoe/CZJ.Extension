@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:menu
cls
echo ========================================
echo           Git 操作菜单
echo ========================================
echo 1. 查看状态 (git status)
echo 2. 添加所有文件 (git add .)
echo 3. 提交更改 (git commit)
echo 4. 推送到远程 (git push)
echo 5. 拉取远程更新 (git pull)
echo 6. 查看提交历史 (git log)
echo 7. 查看分支 (git branch)
echo 8. 切换分支 (git checkout)
echo 9. 创建新分支 (git checkout -b)
echo 10. 快速提交推送 (add + commit + push)
echo 0. 退出
echo ========================================
set /p choice=请选择操作 (0-10): 

if "%choice%"=="1" goto status
if "%choice%"=="2" goto add
if "%choice%"=="3" goto commit
if "%choice%"=="4" goto push
if "%choice%"=="5" goto pull
if "%choice%"=="6" goto log
if "%choice%"=="7" goto branch
if "%choice%"=="8" goto checkout
if "%choice%"=="9" goto newbranch
if "%choice%"=="10" goto quick
if "%choice%"=="0" goto end
echo 无效选择，请重新输入！
pause
goto menu

:status
echo.
echo 正在查看状态...
git status
pause
goto menu

:add
echo.
echo 正在添加所有文件...
git add .
echo 文件添加完成！
pause
goto menu

:commit
echo.
set /p msg=请输入提交信息: 
if "%msg%"=="" (
    echo 提交信息不能为空！
    pause
    goto menu
)
git commit -m "%msg%"
echo 提交完成！
pause
goto menu

:push
echo.
echo 正在推送到远程仓库...
git push
if errorlevel 1 (
    echo 推送失败！请检查网络或权限设置。
) else (
    echo 推送成功！
)
pause
goto menu

:pull
echo.
echo 正在拉取远程更新...
git pull
if errorlevel 1 (
    echo 拉取失败！可能存在冲突。
) else (
    echo 拉取成功！
)
pause
goto menu

:log
echo.
echo 最近的提交历史：
git log --oneline -10
pause
goto menu

:branch
echo.
echo 当前分支列表：
git branch -a
pause
goto menu

:checkout
echo.
set /p branchname=请输入要切换的分支名: 
if "%branchname%"=="" (
    echo 分支名不能为空！
    pause
    goto menu
)
git checkout %branchname%
pause
goto menu

:newbranch
echo.
set /p newbranch=请输入新分支名: 
if "%newbranch%"=="" (
    echo 分支名不能为空！
    pause
    goto menu
)
git checkout -b %newbranch%
echo 新分支创建并切换成功！
pause
goto menu

:quick
echo.
set /p quickmsg=请输入提交信息: 
if "%quickmsg%"=="" (
    echo 提交信息不能为空！
    pause
    goto menu
)
echo 正在执行快速提交推送...
git add .
git commit -m "%quickmsg%"
git push
if errorlevel 1 (
    echo 操作过程中出现错误！
) else (
    echo 所有操作完成！
)
pause
goto menu

:end
echo 感谢使用，再见！
endlocal
exit /b