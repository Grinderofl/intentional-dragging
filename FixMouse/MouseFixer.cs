using System;
using System.Diagnostics;

namespace FixMouse
{
    public class MouseFixer
    {
        private Stopwatch _timer;
        private WINAPI.POINT? _downPosition;
        private readonly MouseHook _hook = new MouseHook();


        /// <summary>
        /// Mouse has to stay still for this long to be allowed to stay down
        /// </summary>
        private readonly int _steadyThresholdMilliseconds;

        /// <summary>
        /// How much the mouse is allowed to move for it to be considered "steady"
        /// </summary>
        private readonly int _steadyDistancePixels;

        public MouseFixer(int steadyThresholdMilliseconds = 100, int steadyDistancePixels = 4)
        {
            _steadyThresholdMilliseconds = steadyThresholdMilliseconds;
            _steadyDistancePixels = steadyDistancePixels;
        }

        public void Start()
        {
            _hook.LeftButtonDown += HookOnLeftButtonDown;
            _hook.LeftButtonUp += HookOnLeftButtonUp;
            _hook.MouseMove += HookOnMouseMove;
            _hook.Install();
        }

        public void Stop()
        {
            _hook.Uninstall();
        }

        private void HookOnMouseMove(WINAPI.MSLLHOOKSTRUCT mouseStruct)
        {
            if (_downPosition == null || _timer  == null)
            {
                return;
            }

            if (_timer.ElapsedMilliseconds > _steadyThresholdMilliseconds)
            {
                Console.WriteLine($"STEADY THRESHOLD");
                Reset();
                return;
            }

            var deltaX = mouseStruct.pt.x - _downPosition.Value.x;
            var deltaY = mouseStruct.pt.y - _downPosition.Value.y;
            var distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            
            if (distance > _steadyDistancePixels)
            {
                Console.WriteLine($"DISTANCE THRESHOLD");
                WINAPI.mouse_event((int)WINAPI.MouseFlags.MOUSEEVENTF_LEFTUP, (uint) mouseStruct.pt.x, (uint) mouseStruct.pt.y, 0, 0);

                Reset();
                return;
            }

            Console.WriteLine($"MOVED: {distance} px");
        }

        private void HookOnLeftButtonDown(WINAPI.MSLLHOOKSTRUCT mouseStruct)
        {
            _timer = Stopwatch.StartNew();
            _downPosition = mouseStruct.pt;
            Console.WriteLine("MOUSEDOWN");
        }

        private void HookOnLeftButtonUp(WINAPI.MSLLHOOKSTRUCT mouseStruct)
        {
            Reset();
            Console.WriteLine("MOUSEUP");
        }

        private void Reset()
        {
            _downPosition = null;
            _timer?.Stop();
            _timer = null;
        }
    }
}