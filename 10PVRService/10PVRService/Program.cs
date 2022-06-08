using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace _10PVRService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static void Main()
        {

            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new PVRService()
            };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {

                PVRService.Logger(ex.Message+" "+DateTime.Now + " Error");
            }

            //PVRService service = new PVRService(args[0].ToString());
            //service.runBrowserThread();


        }
    }
}
