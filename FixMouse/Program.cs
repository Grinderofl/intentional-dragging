using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FixMouse
{
    class Program
    {

        static void Main(string[] args)
        {
            var fixer = new MouseFixer();
            fixer.Start();
            MsgProc(System.Diagnostics.Process.GetCurrentProcess().Handle,
                  IntPtr.Zero,
                  string.Empty,
                  (int) WINAPI.ShowWindowCommands.Normal);
            fixer.Stop();
        }

        static bool MsgProc(IntPtr hinstance, IntPtr hPrevInstance, string lpCmdLine, int nCmdShow)
        {
            WINAPI.MSG msg;

            sbyte hasMessage;

            while ((hasMessage = WINAPI.GetMessage(out msg, IntPtr.Zero, 0, 0)) != 0 && hasMessage != -1)
            {
                WINAPI.TranslateMessage(ref msg);
                WINAPI.DispatchMessage(ref msg);
            }

            return msg.wParam == UIntPtr.Zero;
        }

    }
}
