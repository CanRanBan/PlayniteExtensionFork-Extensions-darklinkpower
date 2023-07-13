using System;
using System.Runtime.InteropServices;

namespace PlayState.Native
{
    public class Ntdll
    {
        private const string dllName = "ntdll.dll";

        [DllImport(dllName, PreserveSig = false)]
        public static extern void NtSuspendProcess(IntPtr processHandle);
        [DllImport(dllName, PreserveSig = false)]
        public static extern void NtResumeProcess(IntPtr processHandle);
    }
}