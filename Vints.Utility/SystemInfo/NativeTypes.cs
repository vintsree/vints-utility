namespace Vints.Utility.SystemInfo
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.ComponentModel;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class NativeTypes
    {
        private NativeTypes()
        {
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        internal sealed class LateInitSafeHandleZeroOrMinusOneIsInvalid : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityCritical]
            internal LateInitSafeHandleZeroOrMinusOneIsInvalid() : base(true)
            {
            }

            [SecurityCritical]
            protected override bool ReleaseHandle()
            {
                return (NativeMethods.CloseHandle(base.handle) != 0);
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        internal sealed class SECURITY_ATTRIBUTES : IDisposable
        {
            public int nLength = Marshal.SizeOf(typeof(NativeTypes.SECURITY_ATTRIBUTES));
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecuritySafeCritical]
            public void Dispose()
            {
                if (this.lpSecurityDescriptor != IntPtr.Zero)
                {
                    UnsafeNativeMethods.LocalFree(this.lpSecurityDescriptor);
                    this.lpSecurityDescriptor = IntPtr.Zero;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}
