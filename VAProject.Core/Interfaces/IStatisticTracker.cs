namespace VAProject.Core.Interfaces
{
    public interface IStatisticTracker
    {
        public Task RecordUsageAsync(string commandName);
    }
}
