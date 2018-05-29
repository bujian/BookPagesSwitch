using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SHGBIT.WPFBase.Helper
{
    public class U32APIHelper
    {
        public static object Lockobject = new object();

        public const long GCL_STYLE = -26;
        public const int GWL_STYLE = -16;
        public const long GWL_EXSTYLE = -20;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_POPUP = 369164288;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_HIDEWINDOW = 0x0010;
        public const int WS_MINIMIZE = 0x20000000;

        public const int SW_HIDE = 0;            //隐藏
        public const int SW_SHOWNORMAL = 1;      //显示，标准状态
        public const int SW_SHOWMINIMIZED = 2;   //显示，最小化
        public const int SW_SHOWMAXIMIZED = 3;   //显示，最大化
        public const int SW_SHOWNOACTIVATE = 4;  //显示，不激活
        public const int SW_SHOW = 5;
        public const int SW_RESTORE = 9;         //显示，回复原状
        public const int SW_SHOWDEFAULT = 10;    //显示，默认

        public const int WS_BORDER = 0x800000;
        public const int WS_CAPTION = 0xC00000;

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("User32.dll")]
        public static extern long SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }
    }
}
