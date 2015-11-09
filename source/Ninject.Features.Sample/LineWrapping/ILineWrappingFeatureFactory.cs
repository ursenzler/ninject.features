namespace Ninject.Features.Sample.LineWrapping
{
    public interface ILineWrappingFeatureFactory
    {
        ILineWrapper CreateDocumentManipulator(WrapConfiguration wrapConfiguration);
    }
}