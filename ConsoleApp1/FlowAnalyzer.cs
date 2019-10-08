using System;
using System.Linq;
using System.Windows.Forms;
using MainForm;

namespace ConsoleApp1
{

    public class FlowAnalyzer
    {
        const int SIZE = 1000;
        const int N = 18;
        const double lambda = 10 * (N + 1) / (N + 4);

        double[] randomArray = new double[SIZE];
        double[] betweenCallsIntervals = new double[SIZE];
        double[] timeOfCallArray = new double[SIZE];

        double[] incomingCallsProbabaility;
        double[] incomingCallsProbabaility_ModeledLambda;
        double[] incomingCallsModelProbabaility;

        int maxCallsPerInterval;


        Interval[] intervalsArray;
        int[] divisionOfCalls;

        int Factorial(int x)
        {
            return x == 0 ? 1 : x * Factorial(--x);
        }

        public double[] CalculateProbability(double lambda)
        {
            var probabilityOfCalls = new double[maxCallsPerInterval + 1];
            for (int i = 0; i <= maxCallsPerInterval; i++)
                probabilityOfCalls[i] = (Math.Pow(lambda, i) / Factorial(i)) * Math.Exp(-lambda);
            return probabilityOfCalls;
        }

        public void Calculate()
        {
            int amountOfIntervals;

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
                        
            amountOfIntervals = (int)timeOfCallArray.Last() + 1;

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

            double averageZ = timeOfCallArray.Last() / 1000;
            double threadValue = 1 / averageZ;

            incomingCallsProbabaility = new double[maxCallsPerInterval + 1];
            incomingCallsModelProbabaility = new double[maxCallsPerInterval + 1];
            incomingCallsProbabaility_ModeledLambda = new double[maxCallsPerInterval + 1];

            Array.Copy(CalculateProbability(lambda), incomingCallsProbabaility, maxCallsPerInterval + 1) ;
            Array.Copy(CalculateProbability(threadValue), incomingCallsProbabaility_ModeledLambda, maxCallsPerInterval + 1);

            for (int i = 0; i < incomingCallsModelProbabaility.Length; i++)
            {
                incomingCallsModelProbabaility[i] = (double)divisionOfCalls[i] / amountOfIntervals;
                Console.WriteLine($"P({i}) \t= {incomingCallsModelProbabaility[i] , 5:N5}");
            }
        }
        public void PrintResults()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            #region Output of results
            /*
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
            */
            Console.WriteLine();
            Console.WriteLine("============ DIVISION OF CALLS AMONG INTERVALS ============");

            for (var i = 0; i < divisionOfCalls.Length; i++)
                Console.WriteLine($"{i} Calls \t: {divisionOfCalls[i]}");


            PrintProbabilities();
            #endregion
        }

        public void PrintProbabilities()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.WriteLine();
            Console.WriteLine("\t\t\t\t\t===================== PROBABILITY OF RECEIVING N CALLS =====================");

            Console.Write("N of calls \t|");
            for (int i = 0; i < incomingCallsModelProbabaility.Length; i++)
                Console.Write($" {i,5} |");
            Console.Write($"  SUM  |");
            Console.WriteLine();
            Console.WriteLine();


            PrintOneProb("Pi(t) \t\t|", incomingCallsProbabaility);
            PrintOneProb("Pi(t) (!lambda) |", incomingCallsProbabaility_ModeledLambda);
            PrintOneProb("!Pi(t) \t\t|", incomingCallsModelProbabaility);
        }

        public void PrintOneProb(string title, double[] probabilityArray)
        {
            double sum = 0;
            var test = new Form1();
            test.richTextBox1.Text += "title";
            Console.Write(title);
            foreach (var item in probabilityArray)
            {
                Console.Write($" {item,5:N3} |");
                sum += item;
            }
            Console.Write($" {sum,5:N3} |");
            Console.WriteLine();
        }
    }
}

