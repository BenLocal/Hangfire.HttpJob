using System;
using System.Collections.Generic;
using System.Text;

namespace AppPlugin1
{
    public class ClassA : IClassA
    {
        public string SayHello()
        {
            Console.WriteLine("Hello");

            return "SayHello Result";
        }
    }
}
