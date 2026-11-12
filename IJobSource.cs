public interface IJobSource
{
    Task<List<AdzunaJob>> FetchJobsAsync();
}
