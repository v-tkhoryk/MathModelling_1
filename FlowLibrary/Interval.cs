namespace FlowLibrary
{
    public class Interval
    {
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Calls { get; set; }

        public void SetPoints(int a, int b)
        {
            StartPoint = a;
            EndPoint = b;
        }
    }
}
