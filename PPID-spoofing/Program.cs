using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PPID_spoofing
{
    internal class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess; public IntPtr hThread; public uint dwProcessId; public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFOEX
        {
            public STARTUPINFO StartupInfo; public IntPtr lpAttributeList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength; public IntPtr lpSecurityDescriptor; public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public uint cb; public string lpReserved; public string lpDesktop; public string lpTitle; public uint dwX; public uint dwY; public uint dwXSize; public uint dwYSize; public uint dwXCountChars; public uint dwYCountChars; public uint dwFillAttribute; public uint dwFlags; public short wShowWindow; public short cbReserved2; public IntPtr lpReserved2; public IntPtr hStdInput; public IntPtr hStdOutput; public IntPtr hStdError;
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000, Reserve = 0x2000, Decommit = 0x4000, Release = 0x8000, Reset = 0x80000, Physical = 0x400000, TopDown = 0x100000, WriteWatch = 0x200000, LargePages = 0x20000000
        }
        [Flags]

        public enum MemoryProtection
        {
            Execute = 0x10, ExecuteRead = 0x20, ExecuteReadWrite = 0x40, ExecuteWriteCopy = 0x80, NoAccess = 0x01, ReadOnly = 0x02, ReadWrite = 0x04, WriteCopy = 0x08, GuardModifierflag = 0x100, NoCacheModifierflag = 0x200, WriteCombineModifierflag = 0x400
        }
  
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(
             UInt32 processAccess,
             bool bInheritHandle,
             int processId);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool InitializeProcThreadAttributeList(
                  IntPtr lpAttributeList,
                  int dwAttributeCount,
                  int dwFlags,
                  ref IntPtr lpSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UpdateProcThreadAttribute(
                IntPtr lpAttributeList,
                uint dwFlags,
                IntPtr Attribute,
                IntPtr lpValue,
                int cbSize,
                IntPtr lpPreviousValue,
                IntPtr lpReturnSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetProcessHeap();

            [DllImport("kernel32.dll", SetLastError = false)]
            public static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CreateProcess(
               string lpApplicationName,
               string lpCommandLine,
               ref SECURITY_ATTRIBUTES lpProcessAttributes,
               ref SECURITY_ATTRIBUTES lpThreadAttributes,
               bool bInheritHandles,
               uint dwCreationFlags,
               IntPtr lpEnvironment,
               string lpCurrentDirectory,
               [In] ref STARTUPINFOEX lpStartupInfo,
               out PROCESS_INFORMATION lpProcessInformation);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hHandle);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr VirtualAllocEx(
              IntPtr hProcess,
              IntPtr lpAddress,
              Int32 dwSize,
              uint flAllocationType,
              MemoryProtection flProtect);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(
              IntPtr hProcess,
              IntPtr lpBaseAddress,
              byte[] lpBuffer,
              Int32 nSize,
              out IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll")]
            public static extern bool VirtualProtectEx(
              IntPtr hProcess,
              IntPtr lpAddress,
              Int32 dwSize,
              uint flNewProtect,
              out uint lpflOldProtect);

            [DllImport("kernel32.dll")]
            public static extern IntPtr CreateRemoteThread(
              IntPtr hProcess,
              IntPtr lpThreadAttributes,
              uint dwStackSize,
              IntPtr lpStartAddress,
              IntPtr lpParameter,
              uint dwCreationFlags,
              IntPtr lpThreadId);

            [DllImport("kernel32.dll")]
            public static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

            [DllImport("kernel32.dll")]
            public static extern uint GetCurrentProcessId();

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

            [DllImport("kernel32.dll")]
            public static extern uint GetLastError();

            [DllImport("kernel32", CharSet = CharSet.Ansi)]
            public static extern IntPtr GetProcAddress(
              IntPtr hModule,
              string procName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(
              string lpModuleName);



        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcess(
string lpApplicationName,
string lpCommandLine,
ref SECURITY_ATTRIBUTES lpProcessAttributes,
ref SECURITY_ATTRIBUTES lpThreadAttributes,
bool bInheritHandles,
uint dwCreationFlags,
IntPtr lpEnvironment,
string lpCurrentDirectory,
[In] ref STARTUPINFO lpStartupInfo,
out PROCESS_INFORMATION lpProcessInformation);

        ///http://www.pinvoke.net/default.aspx/kernel32/OpenProcess.html 
        public enum ProcessAccessRights
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        #region PPID Spoofing

        // https://stackoverflow.com/questions/10554913/how-to-call-createprocess-with-startupinfoex-from-c-sharp-and-re-parent-the-ch
        public const int PROC_THREAD_ATTRIBUTE_PARENT_PROCESS = 0x00020000;
        public const int STARTF_USESTDHANDLES = 0x00000100;
        public const int STARTF_USESHOWWINDOW = 0x00000001;
        public const short SW_HIDE = 0x0000;
        public const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
        public const uint CREATE_NO_WINDOW = 0x08000000;
        public const uint CreateSuspended = 0x00000004;

        #endregion PPID Spoofing


        // GetParentProcess from https://stackoverflow.com/questions/394816/how-to-get-parent-process-in-net-in-managed-way
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
        };

        public static Process GetParentProcess()
        {
            return GetParentProcess(Process.GetCurrentProcess().Handle);
        }
        public static Process GetParentProcess(int id)
        {
            Process process = Process.GetProcessById(id);
            return GetParentProcess(process.Handle);
        }
        public static Process GetParentProcess(IntPtr handle)
        {
            ParentProcessUtilities pbi = new ParentProcessUtilities();
            int returnLength;
            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
            if (status != 0)
                throw new Win32Exception(status);

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                // not found
                return null;
            }
        }
        //

        static void Main(string[] args)
        {

            /*
            const uint NORMAL_PRIORITY_CLASS = 0x0020;

            bool retValue;
            string Application = "C:\\Windows\\System32\\calc.exe";
            PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
            STARTUPINFO sInfo = new STARTUPINFO();
            SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);

            //Open Notepad
            retValue = CreateProcess(Application, null,
            ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
            IntPtr.Zero, null, ref sInfo, out pInfo);

            Console.WriteLine("Process ID (PID): " + pInfo.dwProcessId);
            Console.WriteLine("Process Handle : " + pInfo.hProcess);
        */

            string lpApplicationName = "C:\\Windows\\System32\\calc.exe";
            var pInfo = new PROCESS_INFORMATION();
            var siEx = new STARTUPINFOEX();

            IntPtr lpValueProc = IntPtr.Zero;
            IntPtr hSourceProcessHandle = IntPtr.Zero;
            var lpSize = IntPtr.Zero;

            InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
            siEx.lpAttributeList = Marshal.AllocHGlobal(lpSize);
            InitializeProcThreadAttributeList(siEx.lpAttributeList, 1, 0, ref lpSize);

            //Get ParentProcess ID
            //int parentId = GetParentProcess().Id;
            int parentId = Process.GetProcessesByName("explorer")[0].Id;
            string parentName = Process.GetProcessById(parentId).ProcessName;

            Debug.WriteLine($"[!] Parent Process is {parentId}:{parentName}");

            IntPtr parentHandle = OpenProcess((uint)ProcessAccessRights.CreateProcess | (uint)ProcessAccessRights.DuplicateHandle, false, parentId);
            Debug.WriteLine($"[!] Handle {parentHandle} opened for parent process id.");

            lpValueProc = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(lpValueProc, parentHandle);

            UpdateProcThreadAttribute(siEx.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS, lpValueProc, IntPtr.Size, IntPtr.Zero, IntPtr.Zero);
            Debug.WriteLine($"[!] Adding attributes to a list.");

            siEx.StartupInfo.dwFlags = STARTF_USESHOWWINDOW | STARTF_USESTDHANDLES;
            siEx.StartupInfo.wShowWindow = SW_HIDE;

            // Create and Sizeof() each Security Descriptor for proc and thread
            var ps = new SECURITY_ATTRIBUTES();
            var ts = new SECURITY_ATTRIBUTES();
            ps.nLength = Marshal.SizeOf(ps);
            ts.nLength = Marshal.SizeOf(ts);

            try
            {
                bool ProcCreate = CreateProcess(lpApplicationName, null, ref ps, ref ts, true, CreateSuspended | EXTENDED_STARTUPINFO_PRESENT | CREATE_NO_WINDOW, IntPtr.Zero, null, ref siEx, out pInfo);
                if (!ProcCreate)
                {
                    Debug.WriteLine($"[-] Proccess failed to execute!");

                }
                Debug.WriteLine($"[!] New process with ID: {pInfo.dwProcessId} created in a suspended state under the defined parent process.");
                Console.WriteLine($"[!] New process with ID: {pInfo.dwProcessId} created in a suspended state under the defined parent process.");
                Debug.WriteLine("check on sysmon!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[-] " + Marshal.GetExceptionCode());
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
