using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMonitoringConsoleApp.Utils
{

    abstract class Monitoring
    {
        protected static Computer computer = new Computer()
        {
            IsGpuEnabled = true,
            IsCpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsPsuEnabled = true,
            IsStorageEnabled = true,
        };

        protected IHardware hardware;

        public Monitoring()
        {
            computer.Open();
            computer.Accept(new Visitor());
        }

        public static void listAllHardware()
        {
            computer.Open();
            computer.Accept(new Visitor());
            foreach (IHardware hardware in computer.Hardware)
            {
                Console.WriteLine("Hardware: {0}, Type: {1}", hardware.Name, hardware.HardwareType);
            }
        }

        public void listAllSensors()
        {
            if (hardware == null)
            {
                return;
            }
            hardware.Update();

            foreach (ISensor sensor in hardware.Sensors)
            {
                Console.WriteLine("Sensors name: {0}, Type: {1}, Value: {2}", sensor.Name, sensor.SensorType, sensor.Value.GetValueOrDefault());
            }
        }

        abstract public void updateState();
    }


    public class Visitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}
