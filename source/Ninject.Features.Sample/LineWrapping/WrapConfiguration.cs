namespace Ninject.Features.Sample.LineWrapping
{
    public class WrapConfiguration
    {
        public WrapConfiguration(int lineLength)
        {
            this.LineLength = lineLength;
        }

        public int LineLength { get; }
    }
}