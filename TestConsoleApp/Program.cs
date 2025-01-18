// See https://aka.ms/new-console-template for more information
#if (DEBUG)
System.Diagnostics.Debugger.Launch();
#endif
string logFilePath = "C:\\test\\log.txt";

if (args.Length > 0)
{
    using (StreamWriter writer = new StreamWriter(logFilePath, true))
    {
        foreach (string parameter in args)
        {
            writer.WriteLine("Parameter: " + parameter);
        }
    }

    //Console.WriteLine("Parameters written to log file.");
}
else
{
    using (StreamWriter writer = new StreamWriter(logFilePath, true))
    {
        writer.WriteLine("No parameters provided.");
    }

    //Console.WriteLine("No parameters provided. Logged to log file.");
}