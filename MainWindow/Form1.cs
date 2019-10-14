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
    public partial class Form1 : Form
    {
        FlowAnalyzer generalFlow;
        FlowAnalyzer flow1;
        FlowAnalyzer flow2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            generalFlow = new FlowAnalyzer();
            generalFlow.Calculate();
            richTextBox1.Text = generalFlow.PrintProbabilities().ToString();
            button2.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var graph = new Graph();
            graph.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            flow1 = new FlowAnalyzer(
                10 * (FlowAnalyzer.SIZE + 1) / (FlowAnalyzer.SIZE + 4));
            flow2 = new FlowAnalyzer(
                15 * (FlowAnalyzer.SIZE + 1) / (FlowAnalyzer.SIZE + 4));


            richTextBox2.Text = flow1.PrintDivisionOfCalls("Flow 1").ToString();
            richTextBox2.AppendText(flow2.PrintDivisionOfCalls("Flow 2", false).ToString());

            int len1 = flow1.intervalsArray.Length;
            int len2 = flow2.intervalsArray.Length;

            Interval[] summary = new Interval[
                Math.Max(len1, len2)];

            for (int i = 0; i < summary.Length; i++)
            {
                summary[i] = new Interval();
            }
            
            for (int i = 0; i < Math.Min(len1, len2); i++)
            {
                summary[i].Calls =
                    (flow1.intervalsArray[i].Calls) + (flow2.intervalsArray[i].Calls);
            }
            if (flow1.intervalsArray.Length >= flow2.intervalsArray.Length)
            {
                Array.Copy(flow1.intervalsArray, len2,summary, len2,len1-len2);
            }
            else
            {
                Array.Copy(flow2.intervalsArray, len1, summary, len1, len2 - len1);
            }

            
            richTextBox2.Text += "\n";
            richTextBox2.Text += string.Format("Summary |");
            foreach (var item in summary)
                richTextBox2.Text += $"{item.Calls,3}|";
        }
    }
}
