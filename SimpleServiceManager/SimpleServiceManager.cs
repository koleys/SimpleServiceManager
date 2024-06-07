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
            string fileName = GetFileName(filePath);

            // Add null check for filePath and fileName
            if (filePath == null || fileName == null)
            {
                return;
            }

            _logger.LogInformation("Simple Service Manager starting");
            try
            {
                Process.Start(filePath);
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
