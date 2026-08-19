using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SchulApp.Services
{
    public sealed class PingPongBackgroundKeyboardInput : IDisposable
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeyDown = 0x0100;
        private const int WmKeyUp = 0x0101;
        private const int WmSysKeyDown = 0x0104;
        private const int WmSysKeyUp = 0x0105;

        private readonly HashSet<Keys> allowedKeys;
        private readonly LowLevelKeyboardProc hookCallback;
        private IntPtr hookHandle;
        private bool disposed;

        public PingPongBackgroundKeyboardInput(IEnumerable<Keys> allowedKeys)
        {
            ArgumentNullException.ThrowIfNull(allowedKeys);

            this.allowedKeys = new HashSet<Keys>(allowedKeys);
            hookCallback = HookCallback;
        }

        public event EventHandler<PingPongBackgroundKeyEventArgs>? KeyStateChanged;

        public bool IsRunning => hookHandle != IntPtr.Zero;

        public void Start()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (IsRunning)
            {
                return;
            }

            using Process currentProcess = Process.GetCurrentProcess();
            ProcessModule? module = currentProcess.MainModule;
            IntPtr moduleHandle = module == null
                ? IntPtr.Zero
                : GetModuleHandle(module.ModuleName);

            hookHandle = SetWindowsHookEx(
                WhKeyboardLl,
                hookCallback,
                moduleHandle,
                0
            );

            if (hookHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    "Die Hintergrund-Steuerung konnte nicht aktiviert werden."
                );
            }
        }

        public void Stop()
        {
            if (hookHandle == IntPtr.Zero)
            {
                return;
            }

            UnhookWindowsHookEx(hookHandle);
            hookHandle = IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int message = wParam.ToInt32();
                bool? isDown = message switch
                {
                    WmKeyDown => true,
                    WmSysKeyDown => true,
                    WmKeyUp => false,
                    WmSysKeyUp => false,
                    _ => null
                };

                if (isDown.HasValue)
                {
                    int virtualKey = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)virtualKey;

                    if (allowedKeys.Contains(key))
                    {
                        KeyStateChanged?.Invoke(
                            this,
                            new PingPongBackgroundKeyEventArgs(key, isDown.Value)
                        );
                    }
                }
            }

            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Stop();
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId
        );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);
    }

    public sealed class PingPongBackgroundKeyEventArgs : EventArgs
    {
        public PingPongBackgroundKeyEventArgs(Keys key, bool isDown)
        {
            Key = key;
            IsDown = isDown;
        }

        public Keys Key { get; }
        public bool IsDown { get; }
    }
}
