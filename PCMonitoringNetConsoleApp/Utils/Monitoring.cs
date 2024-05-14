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
        static Computer computer = new Computer()
        {
            IsGpuEnabled = true,
            IsCpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsPsuEnabled = true,
            IsStorageEnabled = true,
        };

        public Monitoring()
        {
            computer.Open();
            computer.Accept(new Visitor());
        }

        protected IHardware? getFirstMatchingHardware(HardwareType type)
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == type)
                {
                    return hardware;
                }

            }

            return null;
        }

        protected IHardware? getFirstMatchingHardware(HardwareType[] types)
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (HardwareType type in types)
                {
                    if (hardware.HardwareType == type)
                    {
                        return hardware;
                    }
                }

            }

            return null;
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
