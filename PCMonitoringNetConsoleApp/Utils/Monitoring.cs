using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        protected static List<Monitoring> monitorings = new List<Monitoring>();

        protected IHardware hardware;

        public Monitoring()
        {
            computer.Open();
            computer.Accept(new Visitor());
            monitorings.Add(this);
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

        public static void updateStates()
        {
            foreach (Monitoring monitoring in monitorings)
            {
                monitoring.updateState(); 
            }
        }

        public static String monitorsToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            foreach (Monitoring monitoring in monitorings)
            {
                sb.Append("\"");
                sb.Append(monitoring.GetType().Name);
                sb.Append("\" :");
                sb.Append(monitoring.toJson());
                sb.Append(",");
            }

            sb.Length--;

            sb.Append("}");

            return sb.ToString();
        }
        protected String toJson()
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] properties = GetType().GetProperties();
            sb.Append("{");
            foreach (PropertyInfo property in properties)
            {
                sb.Append("\"");
                sb.Append(property.Name);
                sb.Append("\" :");
                if(property.PropertyType.Name == "String")
                {
                    sb.Append("\"");
                    sb.Append(property.GetValue(this));
                    sb.Append("\",");
                }
                else if (property.PropertyType.Name == "Int32")
                {
                    sb.Append(property.GetValue(this));
                    sb.Append(",");
                }
                else if(property.PropertyType.Name == "Double")
                {
                    sb.Append(property.GetValue(this).ToString().Replace(',', '.'));
                    sb.Append(",");
                }


            }
            sb.Length--;
            sb.Append("}");

            return sb.ToString();
        }

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
