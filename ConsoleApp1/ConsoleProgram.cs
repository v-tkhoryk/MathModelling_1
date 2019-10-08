using System;

namespace ConsoleApp1
{
    class ConsoleProgram
    {
        static void Main(string[] args)
        {

            var flow = new FlowAnalyzer();
            flow.Calculate();
            Console.ReadLine();
        }

    }
}

