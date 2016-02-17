using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace DiaryDaemon
{
    class GlobalHotkeys
    { 
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Maybe not hard-code this? I have no idea what would cause collisions. 
        private const int HotkeyStart = 9000;
        private int _currentHotkey;

        private HwndSource Source { get; }
        private IntPtr Handle { get;  }
        private const int WmHotkey = 0x0312;

        private readonly Dictionary<int, Tuple<Keys, ModifierKeys>> _registeredHotKeys = new Dictionary<int, Tuple<Keys, ModifierKeys>>();
        private readonly Dictionary<int, Action> _callbacks = new Dictionary<int, Action>(); 
        private readonly List<int> _cleanupList = new List<int>(); 

        public GlobalHotkeys(Window parentWindow)
        {
            Handle = new WindowInteropHelper(parentWindow).EnsureHandle();
            Source = HwndSource.FromHwnd(Handle);
            Source.AddHook(HwndHook);

            _currentHotkey = HotkeyStart;
        }

        public void RegisterGlobalHotkey(Keys hotkey, ModifierKeys mods, Action callback)
        {
            var success = RegisterHotKey(Handle, _currentHotkey, (uint) mods, (uint) hotkey);

            _registeredHotKeys.Add(_currentHotkey, new Tuple<Keys, ModifierKeys>(hotkey, mods));
            _callbacks.Add(_currentHotkey, callback);
            _cleanupList.Add(_currentHotkey);

            _currentHotkey++;
        }

        private void UnregisterGlobalHotkey(int hotkeyId)
        {
            UnregisterHotKey(Handle, hotkeyId);
            _registeredHotKeys.Remove(hotkeyId);
            _callbacks.Remove(hotkeyId); 
        }

        public void Cleanup()
        {
            foreach (var key in _cleanupList)
            {
                UnregisterGlobalHotkey(key);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // If the message has the type 0x0312, then it's a global hotkey, or what we're looking for. 
            if (msg == WmHotkey)
            {
                var wpar = wParam.ToInt32();

                if (_cleanupList.Contains(wpar))
                {
                    // The windows API hides modifier keys and the actual key code in LOWERWORD and UPPERWORD, 
                    // meaning you have to separate the two before you can work with it. 
                    var vkey = (((int) lParam >> 16) & 0xFFFF);
                    var hotkey = _registeredHotKeys[wpar].Item1;

                    if (vkey == (int) hotkey)
                    {
                        _callbacks[wpar].Invoke();
                    }
                    handled = true; 
                }
            }
            return IntPtr.Zero;
        }
    }
}
