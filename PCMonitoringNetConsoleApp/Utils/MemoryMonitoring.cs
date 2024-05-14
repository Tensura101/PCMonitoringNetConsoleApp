using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCMonitoringConsoleApp.Utils
{
    internal class MemoryMonitoring : Monitoring
    {
        private double memoryUsed;
        private double maxMemory;

        public MemoryMonitoring() : base()
        {
            maxMemory = new Microsoft.VisualBasic.D;
        }
        public override void updateState()
        {
            throw new NotImplementedException();
        }
    }
}
