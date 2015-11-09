namespace Ninject.Features.Sample
{
    public interface IWorkflow
    {
        string Process(string path, int maxLineLength);
    }
}