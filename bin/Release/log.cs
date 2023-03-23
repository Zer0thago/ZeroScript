using System;
using System.Runtime.InteropServices;

            namespace ZeroCode
            {
                public class ZeroCode
                {
                    public static void Main()
                    {
                        int MadeWithZeroCode = 0;
                        System.Windows.Forms.MessageBox.Show("Hello! This is a big programm!!!");
Console.WriteLine("Was ist dein name?");
string name = Console.ReadLine();
Console.WriteLine("Wie alt bist du?");
string  alter = Console.ReadLine();
Console.WriteLine("Woher kommst du?");
string  wohin = Console.ReadLine();
System.Windows.Forms.MessageBox.Show("Hallo, "+name+"! Du bist "+alter+" Jahre alt und"+ " kommst aus "+wohin);
                    }
                }
            }