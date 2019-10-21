using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowLibrary;

namespace MainWindow
{
    public partial class FlowModel : Form
    {
        public static FlowAnalyzer flow1;
        public static FlowAnalyzer flow2;
        public static Interval[] summary;

        public static FlowAnalyzer generalFlow;
        
        private double lambda1 = 10 * (FlowAnalyzer.SIZE + 1.0) / (FlowAnalyzer.SIZE + 4);
        private double lambda2 = 15 * (FlowAnalyzer.SIZE + 1.0) / (FlowAnalyzer.SIZE + 4);
        private double lambdaSum;

        public FlowModel()
        {
            this.InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            generalFlow = new FlowAnalyzer();
            generalFlow.Calculate();
            richTextBox1.Text = generalFlow.PrintDistributionOfIntervals().ToString();

            richTextBox1.AppendText($"\n\nLambda = {generalFlow.lambda};\n" +
                $"Modelled Lambda = {generalFlow.modelledLambda}");

            richTextBox1.AppendText(generalFlow.PrintProbabilities().ToString());
            richTextBox4.Text = generalFlow.PrintPrimaryCalculation().ToString();
            richTextBox5.Text = generalFlow.PrintCallsPerInterval().ToString();
            button2.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var graph = new Plot();
            graph.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            flow1 = new FlowAnalyzer(lambda1);
            flow2 = new FlowAnalyzer(lambda2);

            richTextBox2.Text = flow1.PrintDivisionOfCalls("Flow 1").ToString();
            richTextBox2.AppendText(flow2.PrintDivisionOfCalls("Flow 2", false).ToString());

            int len1 = flow1.intervalsArray.Length;
            int len2 = flow2.intervalsArray.Length;

            summary = new Interval[
                Math.Max(len1, len2)];

            for (int i = 0; i < summary.Length; i++)
            {
                summary[i] = new Interval();
                summary[i].SetPoints(i, i + 1);
            }

            for (int i = 0; i < Math.Min(len1, len2); i++)
            {
                summary[i].Calls =
                    flow1.intervalsArray[i].Calls + flow2.intervalsArray[i].Calls;
            }
            if (flow1.intervalsArray.Length >= flow2.intervalsArray.Length)
            {
                Array.Copy(flow1.intervalsArray, len2, summary, len2, len1 - len2);
            }
            else
            {
                Array.Copy(flow2.intervalsArray, len1, summary, len1, len2 - len1);
            }

            lambdaSum = (double)(FlowAnalyzer.SIZE) / summary.Last().EndPoint;

            PlotLambdas();

            richTextBox2.Text += "\n";
            richTextBox2.Text += string.Format("Summary |");
            foreach (var item in summary)
            {
                richTextBox2.Text += $"{item.Calls,3}|";
            }
            button4.Enabled = true;
        }

        private void PlotLambdas()
        {
            richTextBox3.Text = $"λ1 = {lambda1:N5}\n" +
                $"λ2 = {lambda2:N5}\n" +
                $"λ1 + λ2 = {(lambda1 + lambda2):N5} \n" +
                $"λ(summary) = {lambdaSum:N5}";
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            var graph = new IntervalPlot();
            graph.Show();
        }
    }
}
