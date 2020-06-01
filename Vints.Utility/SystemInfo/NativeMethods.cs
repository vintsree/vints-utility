namespace Vints.Utility.SystemInfo
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    [ComVisible(false)]
    internal sealed class NativeMethods
    {
        private NativeMethods()
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityCritical, DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int CloseHandle(IntPtr hObject);

        [SecurityCritical, DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical, DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential)]
        internal struct MEMORYSTATUS
        {
            internal uint dwLength;
            internal uint dwMemoryLoad;
            internal uint dwTotalPhys;
            internal uint dwAvailPhys;
            internal uint dwTotalPageFile;
            internal uint dwAvailPageFile;
            internal uint dwTotalVirtual;
            internal uint dwAvailVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MEMORYSTATUSEX
        {
            internal uint dwLength;
            internal uint dwMemoryLoad;
            internal ulong ullTotalPhys;
            internal ulong ullAvailPhys;
            internal ulong ullTotalPageFile;
            internal ulong ullAvailPageFile;
            internal ulong ullTotalVirtual;
            internal ulong ullAvailVirtual;
            internal ulong ullAvailExtendedVirtual;
            internal void Init()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(NativeMethods.MEMORYSTATUSEX));
            }
        }
    }
}
