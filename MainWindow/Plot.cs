using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using FlowLibrary;

namespace MainWindow
{
    public partial class Plot : Form
    {
        public Plot()
        {
            InitializeComponent();
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Probability vs Modelled probability of incoming calls per minute";
            myPane.XAxis.Title.Text = "Calls";
            myPane.YAxis.Title.Text = "Probability";

            PointPairList probsPairList = new PointPairList();
            PointPairList modelProbsPairList = new PointPairList();
            for (int i = 0; i < FlowModel.generalFlow.incomingCallsProbabaility.Length; i++)
            {
                probsPairList.Add(i, FlowModel.generalFlow.incomingCallsProbabaility[i]);
                modelProbsPairList.Add(i, FlowModel.generalFlow.incomingCallsModelProbabaility[i]);
            }

            LineItem realProbCurve = myPane.AddCurve("Probability of incoming calls",
                  probsPairList, Color.Red, SymbolType.Diamond);

            LineItem modelledProbCurve = myPane.AddCurve("Modelled probability",
                  modelProbsPairList, Color.Blue, SymbolType.Circle);

            zedGraphControl1.AxisChange();

            myPane.XAxis.Scale.MajorStep = 1;
            myPane.XAxis.Scale.Max = probsPairList.Count;
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
