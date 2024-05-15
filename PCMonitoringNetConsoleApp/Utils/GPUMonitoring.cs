using LibreHardwareMonitor.Hardware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000"))
                {
                    if (key != null)
                    {
                        object o = key.GetValue("HardwareInformation.qwMemorySize");
                        if (o != null)
                            maxMemory = Math.Round((long)o / (1024 * 1024) / 1024d, 2);
                    }
                }
            }
            catch { }


            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (HardwareType type in gpuTypes)
                {
                    if (hardware.HardwareType == type)
                    {
                        this.hardware = hardware;
                    }
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

        public void toArray()
        {
            this.GetType().GetProperties();
            Console.WriteLine("asd");
        }



        public int Fps { get { return fps; } }
        public int AvgFps { get {return avgFps;} }
        public int Temp { get {return temp;} }
        public int HotSpot { get {return hotSpot;} }
        public int Load { get {return load;} }
        public int Consumption { get {return consumption;} }
        public double Frequency { get {return frequency;} }
        public double MemoryUsed { get {return memoryUsed;} }
        public double MaxMemory { get {return maxMemory;} }
        public double MemoryFrequency { get {return memoryFrequency;} }
    }
}
