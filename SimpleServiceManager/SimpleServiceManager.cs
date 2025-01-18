using System.Diagnostics;

namespace ServiceWrapper
{
    public class ServiceManager : BackgroundService
    {
        private readonly ILogger<ServiceManager> _logger;
        public IConfigurationRoot Configuration { get; set; }

        public ServiceManager(ILogger<ServiceManager> logger, IConfiguration config)
        {
            _logger = logger;
            Configuration = (IConfigurationRoot)config;
        }
        private string GetAppParams()
        {
            string retval = null;
            string tempath = Configuration.GetSection("Configs:AppParams").Value.ToString();
            if (!String.IsNullOrEmpty(tempath))
            {
                retval = tempath;
            }
            else
            {
                _logger.LogInformation("no AppParams : {time}", DateTimeOffset.Now);
            }
            return retval;
        }

        private string GetPath()
        {
            string retval = null;
            string tempath = Configuration.GetSection("Configs:AppPath").Value.ToString();
            if (!String.IsNullOrEmpty(tempath))
            {
                if (CheckPath(tempath))
                {
                    retval = Path.GetFullPath(tempath);
                }
            }
            else
            {
                _logger.LogInformation("Simple Service Manager Exception Wrong APP path : {time}", DateTimeOffset.Now);
            }
            return retval;
        }
        public string GetFileName(string path)
        {
            if (CheckPath(path))
            {
                return Path.GetFileName(path);
            }
            else
            {
                return null;
            }
        }
        private bool CheckPath(string path)
        {
            return File.Exists(path);
        }
        private static void StartProcess(string filePath, string appParams)
        {
            string fileExtension = Path.GetExtension(filePath);
            if (fileExtension.ToLower() == ".ps1")
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-File \"{filePath}\" {appParams}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();                    
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(appParams))
                {
                    Process.Start(filePath, appParams);
                }
                else
                {
                    Process.Start(filePath);
                }
            }
        }
        // Executes the service asynchronously, starting a process defined by the file path obtained from GetPath. 
        // The process is monitored by checking if it is running every second. 
        // If the process stops, the service is also stopped. 
        // Logs the start, running status, and stop of the service along with any errors encountered.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
#if (DEBUG)
            System.Diagnostics.Debugger.Launch();
#endif
            Process myproc;
            string filePath = GetPath();
            string appParams = GetAppParams();
            //string fileName = GetFileName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            bool restartAppAutomatically = bool.Parse(Configuration.GetSection("Configs:RestartAppAutomatically").Value.ToString());
            int restartDelay = int.Parse(Configuration.GetSection("Configs:RestartDelay").Value.ToString());

            // Add null check for filePath and fileName
            if (filePath == null || fileName == null)
            {
                return;
            }

            _logger.LogInformation("Simple Service Manager starting");
            try
            {
                StartProcess(filePath, appParams);
                await Task.Delay(1000, stoppingToken);//1 sec delay before looping
                _logger.LogInformation("Simple Service Manager started running at: {time}", DateTimeOffset.Now);

                bool isProcessRunning = false;
                while (!stoppingToken.IsCancellationRequested)
                {
                    myproc = Process.GetProcessesByName(fileName).FirstOrDefault();
                    if (myproc != null) //check if "fileName" is running every 1 sec
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));

                        if (!isProcessRunning)
                        {
                            _logger.LogInformation("Process is running");
                            isProcessRunning = true;
                        }
                    }
                    else
                    {
                        if(restartAppAutomatically)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(restartDelay));
                            StartProcess(filePath, appParams);
                            continue;
                        }
                        Token.mytoken.Cancel();
                        _logger.LogInformation("Process stopped");
                        break; //"fileName" process stopped so service is also stopped
                    }
                }

                _logger.LogInformation("Simple Service Manager killed at: {time}", DateTimeOffset.Now);
                myproc = Process.GetProcessesByName(fileName).FirstOrDefault();
                if (myproc != null)
                {
                    myproc.Kill();//Kill "fileName" If service is stopping.
                    _logger.LogInformation("Simple Service Manager killed server at: {time}", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SimpleServiceManager");
                throw;
            }
        }

        
    }
}
