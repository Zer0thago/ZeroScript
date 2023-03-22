using System;
using System.Runtime.InteropServices;

            namespace MyNamespace
            {
                public class MyClass
                {
[DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        public static void HideConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }
                    public static void Main()
                    {
                        int x = 2;;
if (x == x) {
System.Windows.Forms.MessageBox.Show("hello");
} else {
System.Windows.Forms.MessageBox.Show("hello2");
}
                    }
                }
            }