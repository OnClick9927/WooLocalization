@echo off
set b="version"
set version ="1"
REM 获取版本号
for /f "tokens=1,2* delims=:," %%a in (Assets/WooLocalization/package.json) do (
    echo %%a| findstr %b% >nul && (
       set version=  %%b
    ) || (
        @REM echo %%a nnn %b%
    )
)


set version=%version: =%
echo on
git subtree split --prefix=Assets/WooLocalization --branch upm
git push origin upm:upm
git tag %version% upm
git push origin upm --tags
pause