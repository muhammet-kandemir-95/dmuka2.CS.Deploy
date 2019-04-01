using System;

namespace Test.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~Welcome Test.ConsoleApp~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Line 0001");
            Console.WriteLine("Line 0002");
            Console.WriteLine("Line 0003");
            Console.WriteLine("Line 0004");
            Console.WriteLine("Line 0005");
            Console.WriteLine("~~~~~~~~~~~~~~~~~Working on Background Test.ConsoleApp~~~~~~~~~~~~~~~~~");

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine(DateTime.Now.ToString("yyyyMMddhhmmss.fff log"));
            }
        }
    }
}
