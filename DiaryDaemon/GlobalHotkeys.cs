using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace DiaryDaemon
{
    class GlobalHotkeys : IDisposable
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

        private readonly Dictionary<int, Tuple<Keys, ModifierKeys>> _registeredHotKeys;
        private readonly Dictionary<int, Action> _callbacks; 

        public GlobalHotkeys(Window parentWindow)
        {
            Handle = new WindowInteropHelper(parentWindow).Handle;
            Source = HwndSource.FromHwnd(Handle);
            Source.AddHook(HwndHook);

            _registeredHotKeys = new Dictionary<int, Tuple<Keys, ModifierKeys>>();
            _callbacks = new Dictionary<int, Action>();

            _currentHotkey = HotkeyStart;
        }

        public int RegisterGlobalHotkey(Keys hotkey, ModifierKeys mods, Action callback)
        {
            RegisterHotKey(Handle, _currentHotkey, (uint) mods, (uint) hotkey);
            _registeredHotKeys.Add(_currentHotkey, new Tuple<Keys, ModifierKeys>(hotkey, mods));
            _callbacks.Add(_currentHotkey, callback);

            _currentHotkey++;
            return _currentHotkey - 1;
        }

        public void UnregisterGlobalHotkey(Keys hotkey, ModifierKeys mods)
        {
            var hotkeyId = _registeredHotKeys
                .FirstOrDefault(x => x.Value.Equals(new Tuple<Keys, ModifierKeys>(hotkey, mods)))
                .Key;

            UnregisterHotKey(Handle, hotkeyId);
            _registeredHotKeys.Remove(hotkeyId);
            _callbacks.Remove(hotkeyId);
        }

        public void UnregisterGlobalHotkey(int hotkeyId)
        {
            UnregisterHotKey(Handle, hotkeyId);
            _registeredHotKeys.Remove(hotkeyId);
            _callbacks.Remove(hotkeyId); 
        }

        public void Dispose()
        {
            foreach (var key in _registeredHotKeys.Keys)
            {
                UnregisterGlobalHotkey(key);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        { 
            if (msg == WmHotkey)
            {
                var wpar = wParam.ToInt32();
                if (_callbacks.ContainsKey(wpar))
                {
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
