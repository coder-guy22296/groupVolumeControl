﻿/* I am not the author of this code */

using System;
using System.Windows.Forms;

namespace GlobalHotkeys
{
    public class GlobalHotkey : IDisposable
    {
        public Modifiers Modifier { get; private set; }
        public int Key { get; private set; }
        public int Id { get; private set; }

        static int nextId = 0;

        private IntPtr hWnd = IntPtr.Zero;
        private bool registered;

        /// <summary>
        /// Creates a GlobalHotkey object.
        /// </summary>
        /// <param name="modifier">Hotkey modifier keys</param>
        /// <param name="key">Hotkey</param>
        /// <param name="window">The Window that the hotkey should be registered to</param>
        /// <param name="registerImmediately"> </param>
        public GlobalHotkey(Modifiers modifier, Keys key, IWin32Window window, bool registerImmediately = false)
        {
            // if (window == null && registerImmediately) throw new ArgumentNullException("window", "You must provide a form or window to register the hotkey against.");
            Modifier = modifier;
            Key = (int)key;
            if (window != null)
                hWnd = window.Handle;
            Id = GetHashCode();
            if (registerImmediately) Register();
            nextId+=10;
        }
        public void setAssocatedWindow(IWin32Window window)
        {
            hWnd = window.Handle;
        }

        /// <summary>
        /// Registers the current hotkey with Windows.
        /// Note! You must override the WndProc method in your window that registers the hotkey, or you will not receive any hotkey notifications.
        /// </summary>
        public void Register()
        {
            if (!NativeMethods.RegisterHotKey(hWnd, Id, (int)Modifier, Key))
            {
                string errorMessage = new System.ComponentModel.Win32Exception(NativeMethods.GetLastError()).Message;
                //uint errcode = NativeMethods.GetLastError();
                throw new GlobalHotkeyException("Hotkey failed to register: " + errorMessage);
            }
            registered = true;
        }

        /// <summary>
        /// Unregisters the current hotkey with Windows.
        /// </summary>
        public void Unregister()
        {
            if (!registered) return;
            if (!NativeMethods.UnregisterHotKey(hWnd, Id))
            {
                string errorMessage = new System.ComponentModel.Win32Exception(NativeMethods.GetLastError()).Message;
                //uint errcode = NativeMethods.GetLastError();
                throw new GlobalHotkeyException("Hotkey failed to unregister: " + errorMessage);
            }
            registered = false;
        }

        #region IDisposable Members / Finalizer

        public void Dispose()
        {
            Unregister();
            GC.SuppressFinalize(this);
        }

        ~GlobalHotkey()
        {
            Unregister();
        }

        #endregion

        #region Overrides

        public override sealed int GetHashCode()
        {
            return (int)Modifier ^ Key;
        }

        #endregion
    }
}
