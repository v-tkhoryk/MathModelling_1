using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace MainWindow
{
    public partial class IntervalPlot : Form
    {
        public IntervalPlot()
        {
            InitializeComponent();
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Flow1 vs Flow2 vs Summary Flow calls distribution";
            myPane.XAxis.Title.Text = "Intervals";
            myPane.YAxis.Title.Text = "Calls";

            PointPairList flow1PairList = new PointPairList();
            PointPairList flow2PairList = new PointPairList();
            PointPairList summaryPairList = new PointPairList();

            foreach (var item in FlowModel.flow1.intervalsArray)
            {
                flow1PairList.Add(item.EndPoint, item.Calls);
            }
            foreach (var item in FlowModel.flow2.intervalsArray)
            {
                flow2PairList.Add(item.EndPoint, item.Calls);
            }
            for (int i = 1; i <= FlowModel.summary.Length; i++)
            {
                summaryPairList.Add(i, FlowModel.summary[i - 1].Calls);
            }

            LineItem realProbCurve = myPane.AddCurve("First Flow",
                  flow1PairList, Color.Red, SymbolType.Diamond);

            LineItem modelledProbCurve = myPane.AddCurve("Second Flow",
                  flow2PairList, Color.Blue, SymbolType.Circle);

            LineItem summaryCurve = myPane.AddCurve("Summary Flow",
                  summaryPairList, Color.Green, SymbolType.Square);


            zedGraphControl1.AxisChange();

            myPane.XAxis.Scale.MajorStep = 1;
            myPane.XAxis.Scale.Max = summaryPairList.Count;
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            SetSize();
        }

        private void SetSize()
        {
            zedGraphControl1.Location = new Point(0, 0);
            zedGraphControl1.IsShowPointValues = true;
            zedGraphControl1.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 50);
        }
    }
}
