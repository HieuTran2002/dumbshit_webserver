using System;
using System.Diagnostics;
using System.Management;

namespace whatever2
{
    public class RAMGetter
    {
        public float[] GetRAM()
        {
            // Get total and available memory in megabytes
            float totalMemory = GetTotalMemory();
            float availableMemory = GetAvailableMemory();
            float usedMemory = totalMemory - availableMemory;
            float usedPercentage = (usedMemory / totalMemory) * 100;

            // Console.WriteLine($"Total Memory: {totalMemory:F2} GB");
            // Console.WriteLine($"Available Memory: {availableMemory:F2} GB");
            // Console.WriteLine($"Used Memory: {usedMemory:F2} GB");
            // Console.WriteLine($"Used Percenetage: {usedPercentage:F2} %");

            return new float[] { usedMemory, totalMemory, availableMemory, usedPercentage };
        }



        static float GetTotalMemory()
        {
            var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            foreach (var obj in searcher.Get())
            {
                return (float)(Convert.ToDouble(obj["TotalPhysicalMemory"]) / (1024 * 1024 * 1024));
            }
            return 0;
        }

        static float GetAvailableMemory()
        {
            var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (var obj in searcher.Get())
            {
                return (float)(Convert.ToDouble(obj["FreePhysicalMemory"]) / (1024 * 1024));
            }
            return 0;
        }
    }
}
