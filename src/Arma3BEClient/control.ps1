do
{
Start-Process Arma3BEClient.exe
Start-Sleep -s 10800
Stop-Process -processname Arma3BEClient
Start-Sleep -s 5
}
while($true)

// set-executionpolicy remotesigned