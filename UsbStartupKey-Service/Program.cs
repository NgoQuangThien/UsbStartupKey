using System.ServiceProcess;

namespace UsbStartupKey_Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UsbStartupKey(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
