using PCMonitoringConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMonitoringNetConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MemoryMonitoring memoryMonitoring = new MemoryMonitoring();
            memoryMonitoring.listAllSensors();
            Console.WriteLine("MaxMemory:" + memoryMonitoring.MaxMemory);
        }
    }
}
