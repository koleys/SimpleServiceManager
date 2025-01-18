

# SSM: Simple Service Manager : Run any Windows application as a Service an NSSM alternative

## Features:
* .NET 8 alternative of NSSM (the Non-Sucking Service Manager), srvany and other service helper programs.
* Automatically handles failure of the application running as a service.
* Continuously monitors the Windows application status. 
* If says it's running, it really is. 
* Service automatically stops if the launched application exits.
* Service Logs all errors in log files to help torubleshoot the issues.
  
## Added new features in v1.1.0

- **Additional parameters**   "AppParams".
- **Restart App Automatically**    "RestartAppAutomatically".
- **Restart Delay**    "RestartDelay".
- Apart form .exe files can run both .bat and .ps1 fiels as well.
## Download
- Download from the link  [Release V1.1.0](https://github.com/koleys/SimpleServiceManager/releases/download/v1.1.0/SimpleServiceManager1.1.0.zip)

## Installation
- Extract **SimpleServiceManager.zip** in an accessible folder on your system.

- Modify the **appsettings.json file** with the path to your Application.exe which you want to run as service
```json
{
  "Configs": {
    "AppPath": "<Filepath>\\TestAPI.exe",  //can not be null
    "AppParams": "",  //"param1 param2 param3 param4 param5",
    "RestartAppAutomatically": false, //can not be null
    "RestartDelay": 5000 //can not be null. Min 1000 ms
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
