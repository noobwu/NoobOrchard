namespace Orchard.Threading.Tasks
{
    public interface IBackgroundTask : IDependency {
        void Sweep();
    }
}
