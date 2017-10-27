echo "运行OrchardNorthwind.Services.OrleansHosting-Slave开始"
%~d0
cd %~dp0
bin\Debug\OrchardNorthwind.Services.OrleansHosting.exe -postfix:-Slave -port:38080
PAUSE