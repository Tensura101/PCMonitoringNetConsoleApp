using LibreHardwareMonitor.Hardware;
using System;

namespace PCMonitoringConsoleApp.Utils
{
    internal class MemoryMonitoring : Monitoring
    {
        private double memoryUsed;
        private double maxMemory = 0;

        public MemoryMonitoring() : base()
        {
            
            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Memory)
                {
                    this.hardware = hardware;
                }
            }

            updateState();
        }
        public override void updateState()
        {
            if (hardware == null)
            {
                return;
            }

            maxMemory = 0;

            foreach (ISensor sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Data && sensor.Name == "Memory Used")
                {
                    memoryUsed = Math.Round(sensor.Value.GetValueOrDefault(), 1);
                    maxMemory += sensor.Value.GetValueOrDefault();
                }
                else if (sensor.SensorType == SensorType.Data && sensor.Name == "Memory Available")
                {
                    maxMemory += sensor.Value.GetValueOrDefault();
                }
            }

            maxMemory = Math.Round(maxMemory, 1);

        }


        public double MemoryUsed { get { return memoryUsed; } }
        public double MaxMemory { get { return maxMemory; } }
    }
}
