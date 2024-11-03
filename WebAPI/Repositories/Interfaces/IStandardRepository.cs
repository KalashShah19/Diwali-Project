namespace WebAPI.Repositories;

interface IStandardRepository
{
    public void AddStandard(int standard);
    public List<int> GetStandards();
    public void RemoveStandard(int standard);
}