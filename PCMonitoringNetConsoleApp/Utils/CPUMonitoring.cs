using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace PCMonitoringConsoleApp.Utils
{
    internal class CPUMonitoring : Monitoring
    {
        private int temp;
        private int load;
        private int consumption;
        private double avgFrequency;

        public CPUMonitoring() : base()
        {
            updateState();

            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    this.hardware = hardware;
                }
            }
        }




        public override void updateState()
        {
            if (hardware == null)
            {
                return;
            }
            //Console.WriteLine("Sensors count: {0}", hardware.Sensors.Length);
            hardware.Update();

            int SumOfFrequency = 0;
            int coresCount = 0;

            foreach (ISensor sensor in hardware.Sensors)
            {
                //Console.WriteLine("Sensors name: {0}, Type: {1}, Value: {2}", sensor.Name, sensor.SensorType, sensor.Value.GetValueOrDefault());
                if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core"))
                {
                    // store
                    temp = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                    // print to console
                    //Console.WriteLine("cpuTemp: " + sensor.Value.GetValueOrDefault());

                }
                else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                {
                    // store
                    load = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                    // print to console
                    //Console.WriteLine("cpuUsage: " + sensor.Value.GetValueOrDefault());

                }
                else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("Package"))
                {
                    // store
                    consumption = (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                    // print to console
                    //Console.WriteLine("CPU Power Draw - Package: " + sensor.Value.GetValueOrDefault());


                }
                else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("Core #"))
                {
                    // store
                    SumOfFrequency += (int)Math.Round(sensor.Value.GetValueOrDefault(), 0);
                    coresCount++;
                    // print to console
                    //Console.WriteLine("cpuFrequency: " + sensor.Value.GetValueOrDefault());
                }

            }
            avgFrequency = Math.Round(SumOfFrequency / coresCount / 1000d, 2);

        }

        public int Temp
        {
            get { return temp; }
        }

        public int Load
        {
            get { return load; }
        }

        public int Consumption
        {
            get { return consumption; }
        }

        public double AvgFrequency
        {
            get { return avgFrequency; }
        }
    }
}
