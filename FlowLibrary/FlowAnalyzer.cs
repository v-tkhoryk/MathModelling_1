namespace FlowLibrary
{
    using System;
    using System.Linq;
    using System.Text;

    public class FlowAnalyzer
    {
        public const int SIZE = 1000;
        private const int N = 18;
        public readonly double lambda = 10 * (N + 1) / (N + 4);
        private double[] randomArray = new double[SIZE];
        private double[] betweenCallsIntervals = new double[SIZE];
        private double[] timeOfCallArray = new double[SIZE];

        public  double[] incomingCallsProbabaility;
        private double[] incomingCallsProbabaility_ModeledLambda;
        public  double[] incomingCallsModelProbabaility;
        private int maxCallsPerInterval;
        public Interval[] intervalsArray;
        public double modelledLambda;
        /// <summary>
        /// Array of amounts of intervals with N calls
        /// </summary>
        private int[] divisionOfCallsAmongIntervals;

        public FlowAnalyzer()
        {

        }

        public FlowAnalyzer(double _lambda)
        {
            lambda = _lambda;
            Calculate();
        }

        private long Factorial(int x)
        {
            return x == 0 ? 1 : x * Factorial(--x);
        }

        public double[] CalculateProbability(double lambda)
        {
            var probabilityOfCalls = new double[maxCallsPerInterval + 1];
            for (int i = 0; i <= maxCallsPerInterval; i++)
                probabilityOfCalls[i] = Math.Pow(lambda, i) / Factorial(i) * Math.Exp(-lambda);
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
                    i == 0 
                    ? betweenCallsIntervals[i] 
                    : betweenCallsIntervals[i] + timeOfCallArray[i - 1];
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
            divisionOfCallsAmongIntervals = new int[maxCallsPerInterval + 1];

            foreach (var interval in intervalsArray)
            {
                divisionOfCallsAmongIntervals[interval.Calls]++;
            }


            double averageZ = intervalsArray.Last().EndPoint / 1000.0;
            modelledLambda = 1 / averageZ;

            incomingCallsProbabaility = new double[maxCallsPerInterval + 1];
            incomingCallsModelProbabaility = new double[maxCallsPerInterval + 1];
            incomingCallsProbabaility_ModeledLambda = new double[maxCallsPerInterval + 1];

            Array.Copy(sourceArray: CalculateProbability(lambda),
                       destinationArray: incomingCallsProbabaility,
                       length: maxCallsPerInterval + 1);
            Array.Copy(sourceArray: CalculateProbability(modelledLambda),
                       destinationArray: incomingCallsProbabaility_ModeledLambda,
                       length: maxCallsPerInterval + 1);

            for (int i = 0; i < incomingCallsModelProbabaility.Length; i++)
            {
                incomingCallsModelProbabaility[i] = (double)divisionOfCallsAmongIntervals[i] / amountOfIntervals;
            }
        }

        public StringBuilder PrintPrimaryCalculation()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("\tPRIMARY CALCULATION");
            output.AppendLine("   i    |       r(i)      |        z(i)      |      t(i)      | ");
            for (int i = 0; i < SIZE; i++)
            {
                output.AppendLine(
                      $"{i + 1,6} |{ randomArray[i],9:N4}   | {betweenCallsIntervals[i],9:N4}   | {timeOfCallArray[i],9:N4} | ");
            }

            return output;
        }

        public StringBuilder PrintCallsPerInterval()
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("\tCALLS PER INTERVAL ");
            foreach (var interval in intervalsArray)
            {
                output.AppendLine($"[{interval.StartPoint,4} : {interval.EndPoint,4}] ={interval.Calls,6}");
            }

            return output;
        }

        public StringBuilder PrintDivisionOfCalls(string title, bool isFirstRaw = true)
        {
            var output = new StringBuilder();
            
            if (isFirstRaw)
            {
                output.AppendLine();
                output.Append($"Interval|");
                for (var i = 1; i <= intervalsArray.Length; i++)
                {
                    output.Append($"{i,3}|");
                }
            }
            output.AppendLine();
            output.Append($"{title,8}|");
            foreach (var item in intervalsArray)
                output.Append($"{item.Calls,3}|");
            return output;
        }

        public StringBuilder PrintDistributionOfIntervals()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("\tINTERVALS WITH N CALLS");
            output.Append("   N Calls    |");
            for (var i = 0; i < divisionOfCallsAmongIntervals.Length; i++)
            {
                output.Append($"{i,6}    |");
            }
            output.AppendLine();
            output.Append("Inters amount |");
            foreach (var item in divisionOfCallsAmongIntervals)
                output.Append($"{item,6}    |");

            return output;
        }

        public StringBuilder PrintProbabilities()
        {
            var output = new StringBuilder("\n\n");
            output.AppendLine("\tPROBABILITY OF RECEIVING N CALLS");

            output.Append("N of calls \t|");
            for (int i = 0; i < incomingCallsModelProbabaility.Length; i++)
                output.Append($" {i,8} |");
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
                output.Append($" {item,5:N6} |");
                sum += item;
            }
            output.Append($" {sum,5:N3} |\n");
            return output;
        }
    }
}
