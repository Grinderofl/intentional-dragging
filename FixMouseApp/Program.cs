using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FixMouse;

namespace FixMouseApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit +=
                (sender, args) =>
                {
                    //Hook.LeftButtonUp -= HookOnLeftButtonUp;
                    //Hook.LeftButtonDown -= HookOnLeftButtonDown;
                    //Hook.MouseMove -= HookOnMouseMove;
                    Hook.Uninstall(); 

                };

            Application.Run(new Form1());

        }

        public static MouseHook Hook= new MouseHook();
    }
}
