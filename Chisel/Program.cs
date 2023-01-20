using System.Runtime.InteropServices;
using System.Text;

namespace Chisel
{
    public class Program
    {
        [DllImport("chisel", EntryPoint = "RunMe")]
        public extern static void RunMe(byte[] test);
        public static void Main(string[] args)
        {
            var strArgs = string.Join(" ", args);
            RunMe(Encoding.ASCII.GetBytes(strArgs));
        }
    }
}