
# SSM: Simple Service Manager : Run any Windows application as a Service

## Features:
* .NET 8 alternative of NSSM (the Non-Sucking Service Manager), srvany and other service helper programs.
* Automatically handles failure of the application running as a service.
* Continuously monitors the Windows application status. 
* If says it's running, it really is. 
* Service automatically stops if the launched application exits.
* Service Logs all errors in log files to help torubleshoot the issues.

## Download
- Download from the link

## Installation
- Place **SimpleServiceManager.exe** in an accessible folder on your system.

- Modify the **appsettings.json file** with the path to your Application.exe which you want to run as service
```json
{
  "Configs": {
    "AppPath": "C:\\Path\\To\\Your\\Application.exe"
  }
}
```
- Install it as a service from an **Elevated (Administrator) Command Prompt**:
```winbatch
sc create "MyServiceName" start= auto binPath= "C:\Path\To\SimpleServiceManager.exe"
sc description MyServiceName "My services description"
sc start MyServiceName
```
Note the spaces between `start=`, `binPath=` and their parameters. This is intended.

## Uninstallation
```winbatch
sc stop MyServiceName
sc delete MyServiceName
```
