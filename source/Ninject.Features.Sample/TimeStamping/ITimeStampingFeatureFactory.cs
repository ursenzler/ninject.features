namespace Ninject.Features.Sample.TimeStamping
{
    public interface ITimeStampingFeatureFactory
    {
        ITimeStamper CreateTimeStamper();
    }
}