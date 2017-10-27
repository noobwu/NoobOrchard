@echo off
call "%~dp0\_environment"
::"%wcatfiles%\wcat.wsf" -terminate -run -clients localhost -t ".\Scripts\dashboard.txt" -f ".\settings.txt" -s localhost -p 10050 -singleip -x
:: "%wcatfiles%\wcat.wsf" -terminate -run -clients localhost -t ".\Scripts\noob.txt" -f ".\settings.txt" -s localhost -p 10066  -x

"%wcatfiles%\wcat.wsf" -terminate -run -clients localhost -t ".\Scripts\ExceptionlessAspCore.txt" -f ".\settings.txt" -s localhost -p 10066  -x