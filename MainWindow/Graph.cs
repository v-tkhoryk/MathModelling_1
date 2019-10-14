using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using FlowLibrary;

namespace MainWindow
{
    public partial class Graph : Form
    {
        public Graph()
        {
            InitializeComponent();
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Probability vs Modelled probability of incoming calls per minute";
            myPane.XAxis.Title.Text = "Calls";
            myPane.YAxis.Title.Text = "Probability";

            PointPairList teamAPairList = new PointPairList();
            PointPairList teamBPairList = new PointPairList();
            //int[] teamAData = buildTeamAData();
            //int[] teamBData = buildTeamBData();
            for (int i = 0; i < FlowAnalyzer.incomingCallsProbabaility.Length; i++)
            //for (int i = 0; i < 14; i++)
            {
                teamAPairList.Add(i, FlowAnalyzer.incomingCallsProbabaility[i]);
                teamBPairList.Add(i, FlowAnalyzer.incomingCallsModelProbabaility[i]);
            }

            LineItem realProbCurve = myPane.AddCurve("Probability of incoming calls",
                  teamAPairList, Color.Red, SymbolType.Diamond);

            LineItem modelledProbCurve = myPane.AddCurve("Modelled probability",
                  teamBPairList, Color.Blue, SymbolType.Circle);

            zedGraphControl1.AxisChange();

            myPane.XAxis.Scale.MajorStep = 1;
            myPane.XAxis.Scale.Max = teamAPairList.Count;
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            SetSize();
        }

        private void SetSize()
        {
            zedGraphControl1.Location = new Point(0, 0);
            zedGraphControl1.IsShowPointValues = true;
            // Leave a small margin around the outside of the control
            zedGraphControl1.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 50);

        }
    }
}
