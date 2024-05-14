using LibreHardwareMonitor.Hardware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace PCMonitoringConsoleApp.Utils
{
    internal class GPUMonitoring : Monitoring
    {
        private int fps;
        private int avgFps = 0;
        private int temp;
        private int hotSpot;
        private int load;
        private int consumption;
        private double frequency;
        private double memoryUsed;
        private double maxMemory;
        private double memoryFrequency;
       
        private HardwareType[] gpuTypes = {HardwareType.GpuAmd, HardwareType.GpuNvidia, HardwareType.GpuIntel };

        public GPUMonitoring() : base()
        {
            updateState();

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000"))
                {
                    if (key != null)
                    {
                        object o = key.GetValue("HardwareInformation.qwMemorySize");
                        if (o != null)
                            MaxMemory = Math.Round((long)o / (1024 * 1024) / 1024d, 2);
                    }
                }
            }
            catch { }
        }

        public override void updateState()
        {
            IHardware? hardware = getFirstMatchingHardware(gpuTypes);
            if (hardware == null)
            {
                return;
            }
            hardware.Update();
            foreach (ISensor sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("Dedicated Memory Used"))
                {
                    memoryUsed = Math.Round(sensor.Value.GetValueOrDefault() / 1024d, 0);
                    //Console.WriteLine("memoryUsed: " + sensor.Value.GetValueOrDefault
                }
                else if (sensor.SensorType == SensorType.Factor && sensor.Name.Contains("FPS"))
                {
                    fps = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                    if(avgFps == 0 && fps > 0)
                    {
                        fps = avgFps;
                    }
                    else if(Fps > 0)
                    {
                        avgFps = (int)Math.Round((AvgFps + Fps) / 2d, 2);
                    }
                    //Console.WriteLine("fps: " + sensor.Value.GetValueOrDefault
                }
                else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("Core"))
                {
                    frequency = Math.Round(sensor.Value.GetValueOrDefault() / 1000d, 2);
                }
                else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("Memory"))
                {
                    memoryFrequency = Math.Round(sensor.Value.GetValueOrDefault() / 1000d, 2);
                }
                else if (sensor.SensorType == SensorType.Power&& sensor.Name.Contains("Package"))
                {
                    consumption = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                }
                else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Core"))
                {
                    load = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                }
                else if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core"))
                {
                    temp = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                }
                else if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Hot Spot"))
                {
                    hotSpot = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                }
            }

        }

        public void listAllSensors()
        {
            IHardware? hardware = getFirstMatchingHardware(gpuTypes);
            if (hardware == null)
            {
                return;
            }
            hardware.Update();
            Console.WriteLine("Properties length: {0}", hardware.Properties.ToArray().Length);

            foreach (var property in hardware.Properties.ToArray()) 
            {
                Console.WriteLine("Property key: {0}", property.Key);
            }

            foreach (ISensor sensor in hardware.Sensors)
            {
                Console.WriteLine("Sensors name: {0}, Type: {1}, Value: {2}", sensor.Name, sensor.SensorType, sensor.Value.GetValueOrDefault());
            }
        }



        public int Fps { get; }
        public int AvgFps { get; }
        public int Temp { get; }
        public int HotSpot { get; }
        public int Load { get; }
        public int Consumption { get; }
        public double Frequency { get; }
        public double MemoryUsed { get; }
        public double MaxMemory { get; }
        public double MemoryFrequency { get; }
    }
}
