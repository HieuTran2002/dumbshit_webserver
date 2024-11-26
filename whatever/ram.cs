using System;
using System.Diagnostics;

namespace RAM
{
    public class RAM_Handle
    {
        public double getRam()
        {
            double memory = 0.0;
            using (Process proc = Process.GetCurrentProcess())
            {
                // The proc.PrivateMemorySize64 will returns the private memory usage in byte.
                // Would like to Convert it to Megabyte? divide it by 2^20
                memory = proc.PrivateMemorySize64 / (1024 * 1024);
            }
            
            return Math.Round(memory, 2);
        }
    }
}