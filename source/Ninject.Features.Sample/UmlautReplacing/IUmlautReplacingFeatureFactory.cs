namespace Ninject.Features.Sample.UmlautReplacing
{
    public interface IUmlautReplacingFeatureFactory
    {
        IUmlautsReplacer CreateDocumentLoader(UmlautReplacingOptions options);
    }
}