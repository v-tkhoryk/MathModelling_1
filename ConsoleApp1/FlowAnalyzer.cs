using System;
using System.Linq;

namespace ConsoleApp1
{
    public class FlowAnalyzer
    {
        const int SIZE = 1000;
        const int N = 14;
        const double lambda = 10 * (N + 1) / (N + 4);

        double[] randomArray = new double[SIZE];
        double[] betweenCallsIntervals = new double[SIZE];
        double[] timeOfCallArray = new double[SIZE];

        Interval[] intervalsArray;
        int[] divisionOfCalls;


        public void Calculate()
        {
            int amountOfIntervals;
            int maxCallsPerInterval;

            var rand = new Random();

            #region Initialization of r[] and z[]

            for (int i = 0; i < SIZE; i++)
            {
                randomArray[i] = rand.NextDouble();
                betweenCallsIntervals[i] = -1 / lambda * Math.Log(randomArray[i]);
                timeOfCallArray[i] += 
                    i == 0 ? betweenCallsIntervals[i] : betweenCallsIntervals[i] + timeOfCallArray[i - 1];
            }

            #endregion
            /*
            timeOfCallArray[0] = 0 + betweenCallsIntervals[0];

            for (int i = 1; i < SIZE; i++)
            {
                timeOfCallArray[i] = timeOfCallArray[i - 1] + betweenCallsIntervals[i];
            }
            */

            amountOfIntervals = (int)timeOfCallArray[SIZE - 1] + 1;

            intervalsArray = new Interval[amountOfIntervals];

            for (int i = 0; i < amountOfIntervals; i++)
            {
                intervalsArray[i] = new Interval();
                intervalsArray[i].SetPoints(i, i + 1);
            }

            foreach (var item in timeOfCallArray)
            {
                intervalsArray[(int)item].Calls++;
            }

            maxCallsPerInterval = intervalsArray.Max(x => x.Calls);
            divisionOfCalls = new int[maxCallsPerInterval + 1];

            foreach (var interval in intervalsArray)
            {
                divisionOfCalls[interval.Calls]++;
            }
        }
        public void Print()
        {
            #region Output of results
            Console.WriteLine("==================== PRIMARY CALCULATION ====================");
            Console.WriteLine("     r(i)     |     z(i)     |     t(i)     | ");
            for (int i = 0; i < SIZE; i++)
                Console.WriteLine(
                    $"{ randomArray[i],9:N5} \t {betweenCallsIntervals[i],9:N5} \t {timeOfCallArray[i],9:N5}");

            Console.WriteLine("==================== CALLS PER INTERVAL ====================");
            foreach (var interval in intervalsArray)
            {
                Console.WriteLine($"[{interval.StartPoint} : {interval.EndPoint}]  \t= {interval.Calls}");
            }

            Console.WriteLine();
            Console.WriteLine("============ DIVISION OF CALLS AMONG INTERVALS ============");
            for (var i = 0; i < divisionOfCalls.Length; i++)
            {
                Console.WriteLine($"{i} Calls \t: {divisionOfCalls[i]}");
            }
            #endregion
        }
    }
}

