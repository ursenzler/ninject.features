namespace Ninject.Features.Sample
{
    using Ninject.Features.Sample.LineWrapping;
    using Ninject.Features.Sample.TimeStamping;
    using Ninject.Features.Sample.UmlautReplacing;

    public class Workflow : IWorkflow
    {
        private readonly UmlautReplacingFeature.IUmlautReplacingFeatureFactory umlautReplacingFeatureFactory;

        private readonly LineWrappingFeature.ILineWrappingFeatureFactory lineWrappingFeatureFactory;

        private readonly TimeStampingFeature.ITimeStampingFeatureFactory timeStampingFeatureFactory;

        public Workflow(
            UmlautReplacingFeature.IUmlautReplacingFeatureFactory umlautReplacingFeatureFactory,
            LineWrappingFeature.ILineWrappingFeatureFactory lineWrappingFeatureFactory,
            TimeStampingFeature.ITimeStampingFeatureFactory timeStampingFeatureFactory)
        {
            this.umlautReplacingFeatureFactory = umlautReplacingFeatureFactory;
            this.lineWrappingFeatureFactory = lineWrappingFeatureFactory;
            this.timeStampingFeatureFactory = timeStampingFeatureFactory;
        }

        public string Process(string content, int maxLineLength)
        {
            content = this.ReplaceUmlauts(content);

            content = this.WrapLines(content, maxLineLength);

            return this.TimeStampEachLine(content);
        }

        private string ReplaceUmlauts(string content)
        {
            var documentLoader = this.umlautReplacingFeatureFactory.CreateDocumentLoader(UmlautReplacingOptions.All);
            return documentLoader.ReplaceUmlauts(content);
        }

        private string WrapLines(string content, int maxLineLength)
        {
            var documentManipulator = this.lineWrappingFeatureFactory
                .CreateDocumentManipulator(new WrapConfiguration(maxLineLength));
            return documentManipulator.WrapLines(content);
        }

        private string TimeStampEachLine(string content)
        {
            var documentSaver = this.timeStampingFeatureFactory.CreateTimeStamper();
            return documentSaver.TimeStampEachLine(content);
        }
    }
}