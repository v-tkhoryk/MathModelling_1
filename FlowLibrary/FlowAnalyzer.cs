using System;
using System.Linq;
using System.Text;

namespace FlowLibrary
{
    public class FlowAnalyzer
    {
        public const int SIZE = 1000;
        const int N = 18;
        double lambda = 10 * (N + 1) / (N + 4);

        double[] randomArray = new double[SIZE];
        double[] betweenCallsIntervals = new double[SIZE];
        double[] timeOfCallArray = new double[SIZE];

        public static double[] incomingCallsProbabaility;
        double[] incomingCallsProbabaility_ModeledLambda;
        public static double[] incomingCallsModelProbabaility;

        int maxCallsPerInterval;
        public Interval[] intervalsArray;
        int[] divisionOfCalls;

        public FlowAnalyzer()
        {

        }

        public FlowAnalyzer(double _lambda)
        {
            lambda = _lambda;
            Calculate();
        }

        long Factorial(int x)
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

            //Console.WriteLine(averageZ + " " + threadValue);

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
            //PrintDivisionOfCalls();

            PrintProbabilities();
            #endregion
        }

        public StringBuilder PrintDivisionOfCalls(string title, bool isFirstRaw=true)
        {
            var output = new StringBuilder();
            //output.AppendLine("============ DIVISION OF CALLS AMONG INTERVALS ============");

            /*
            for (var i = 0; i < divisionOfCalls.Length; i++)
                output.AppendLine($"{i} Calls \t: {divisionOfCalls[i]}");
                */

            if (isFirstRaw)
            {
                output.AppendLine();
                output.Append($"Interval|");
                for (var i = 0; i < intervalsArray.Length; i++)
                    output.Append($"{i,3}|");
            }
            output.AppendLine();
            output.Append($"{title, 8}|");
            foreach (var item in intervalsArray)
                output.Append($"{item.Calls, 3}|");
            //output.AppendLine();
            return output;
        }

        public StringBuilder PrintProbabilities()
        {
            var output = new StringBuilder();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            output.AppendLine();
            output.AppendLine("\t\t\t\t\t===================== PROBABILITY OF RECEIVING N CALLS =====================");

            output.Append("N of calls \t|");
            for (int i = 0; i < incomingCallsModelProbabaility.Length; i++)
                //output.Append($"{i,11}|");
                output.Append($" {i, 5} |");
            output.Append($"  SUM  |");
            output.AppendLine();
            output.AppendLine();


            output.Append(PrintOneProb("Pi(t) \t\t|", incomingCallsProbabaility));
            output.Append(PrintOneProb("Pi(t) (!l)\t|", incomingCallsProbabaility_ModeledLambda));
            output.Append(PrintOneProb("!Pi(t) \t|", incomingCallsModelProbabaility));

            return output;
        }

        public StringBuilder PrintOneProb(string title, double[] probabilityArray)
        {
            StringBuilder output = new StringBuilder();
            double sum = 0;
            output.Append(title);
            foreach (var item in probabilityArray)
            {
                output.Append($" {item,5:N3} |");
                sum += item;
            }
            output.Append($" {sum,5:N3} |\n");
            return output;
        }
    }
}

