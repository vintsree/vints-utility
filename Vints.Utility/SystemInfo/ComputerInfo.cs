namespace Vints.Utility.SystemInfo
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;

    [DebuggerTypeProxy(typeof(ComputerInfoDebugView)), HostProtection(SecurityAction.LinkDemand, Resources = HostProtectionResource.ExternalProcessMgmt)]
    public class ComputerInfo
    {
        private InternalMemoryStatus internalMemoryStatus = null;

        /// <summary>
        /// Total Available Physical Memory in Bytes
        /// </summary>
        public ulong AvailablePhysicalMemory
        {
            [SecuritySafeCritical]
            get
            {
                return this.MemoryStatus.AvailablePhysicalMemory;
            }
        }

        public CultureInfo InstalledUICulture
        {
            get
            {
                return CultureInfo.InstalledUICulture;
            }
        }

        private InternalMemoryStatus MemoryStatus
        {
            get
            {
                if (this.internalMemoryStatus == null)
                {
                    this.internalMemoryStatus = new InternalMemoryStatus();
                }
                return this.internalMemoryStatus;
            }
        }

        public string OSPlatform
        {
            get
            {
                return Environment.OSVersion.Platform.ToString();
            }
        }

        public string OSVersion
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }

        /// <summary>
        /// Total Physical Memory in Bytes
        /// </summary>
        public ulong TotalPhysicalMemory
        {
            [SecuritySafeCritical]
            get
            {
                return this.MemoryStatus.TotalPhysicalMemory;
            }
        }

        internal sealed class ComputerInfoDebugView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ComputerInfo instanceBeingWatched;

            public ComputerInfoDebugView(ComputerInfo RealClass)
            {
                this.instanceBeingWatched = RealClass;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ulong AvailablePhysicalMemory
            {
                get
                {
                    return this.instanceBeingWatched.AvailablePhysicalMemory;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public CultureInfo InstalledUICulture
            {
                get
                {
                    return this.instanceBeingWatched.InstalledUICulture;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public string OSPlatform
            {
                get
                {
                    return this.instanceBeingWatched.OSPlatform;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public string OSVersion
            {
                get
                {
                    return this.instanceBeingWatched.OSVersion;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ulong TotalPhysicalMemory
            {
                get
                {
                    return this.instanceBeingWatched.TotalPhysicalMemory;
                }
            }
        }

        private class InternalMemoryStatus
        {
            private bool isOldOS = (Environment.OSVersion.Version.Major < 5);
            private NativeMethods.MEMORYSTATUS memoryStatus;
            private NativeMethods.MEMORYSTATUSEX memoryStatusEx;

            internal InternalMemoryStatus()
            {
            }

            [SecurityCritical]
            private void Refresh()
            {
                if (this.isOldOS)
                {
                    this.memoryStatus = new NativeMethods.MEMORYSTATUS();
                    NativeMethods.GlobalMemoryStatus(ref this.memoryStatus);
                }
                else
                {
                    this.memoryStatusEx = new NativeMethods.MEMORYSTATUSEX();
                    this.memoryStatusEx.Init();
                    if (!NativeMethods.GlobalMemoryStatusEx(ref this.memoryStatusEx))
                    {
                        throw new Exception("DiagnosticInfo Memory");
                    }
                }
            }

            internal ulong AvailablePhysicalMemory
            {
                [SecurityCritical]
                get
                {
                    this.Refresh();
                    if (this.isOldOS)
                    {
                        return (ulong)this.memoryStatus.dwAvailPhys;
                    }
                    return this.memoryStatusEx.ullAvailPhys;
                }
            }

            internal ulong TotalPhysicalMemory
            {
                [SecurityCritical]
                get
                {
                    this.Refresh();
                    if (this.isOldOS)
                    {
                        return (ulong)this.memoryStatus.dwTotalPhys;
                    }
                    return this.memoryStatusEx.ullTotalPhys;
                }
            }
        }
    }
}
