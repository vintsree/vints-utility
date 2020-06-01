namespace Vints.Utility.SystemInfo
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SuppressUnmanagedCodeSecurity, ComVisible(false)]
    internal sealed class UnsafeNativeMethods
    {
        [SecurityCritical]
        private UnsafeNativeMethods()
        {
        }

        [SecurityCritical, DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr LocalFree(IntPtr LocalHandle);
    }
}
