using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FixMouse;

namespace FixMouseApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.Hook.LeftButtonUp += HookOnLeftButtonUp;
            Program.Hook.LeftButtonDown += HookOnLeftButtonDown;
            Program.Hook.MouseMove += HookOnMouseMove;
            Program.Hook.Install();
        }

                private static bool _down;
        private static Stopwatch _timer;
        private static int _x = -1;
        private static int _y = -1;
        


        /// <summary>
        /// Mouse has to stay still for this long to be allowed to stay down
        /// </summary>
        private const int SteadyThresholdMilliseconds = 150;

        /// <summary>
        /// How much the mouse is allowed to move for it to be considered "steady"
        /// </summary>
        private const int SteadyDistancePixels = 4;

        private static void HookOnMouseMove(WINAPI.MSLLHOOKSTRUCT mouseStruct)
        {
            if (!_down || _x == -1 || _y == -1)
            {
                return;
            }

            if (_timer.ElapsedMilliseconds > SteadyThresholdMilliseconds)
            {
                Debug.WriteLine($"STEADY THRESHOLD");
                Reset();
                return;
            }

            var deltaX = mouseStruct.pt.x - _x;
            var deltaY = mouseStruct.pt.y - _y;
            var distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            
            if (distance > SteadyDistancePixels)
            {
                Debug.WriteLine($"DISTANCE THRESHOLD");
                WINAPI.mouse_event((int)WINAPI.MouseFlags.MOUSEEVENTF_LEFTUP, (uint) mouseStruct.pt.x, (uint) mouseStruct.pt.y, 0, 0);

                Reset();
                return;
            }
            Debug.WriteLine($"MOVED: {distance} px");
        }

        private static void HookOnLeftButtonDown(WINAPI.MSLLHOOKSTRUCT mouseStruct)
        {
            _down = true;
            _timer = Stopwatch.StartNew();
            _x = mouseStruct.pt.x;
            _y = mouseStruct.pt.y;
            Debug.WriteLine("MOUSEDOWN");
        }

        private static void HookOnLeftButtonUp(WINAPI.MSLLHOOKSTRUCT mouseStruct)
        {
            Reset();
            Debug.WriteLine("MOUSEUP");
        }

        private static void Reset()
        {


            _down = false;
            _x = _y = -1;
            _timer?.Stop();
            _timer = null;
        }
    }
}
