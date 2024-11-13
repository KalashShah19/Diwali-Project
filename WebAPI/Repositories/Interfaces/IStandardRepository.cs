namespace WebAPI.Repositories;

interface IStandardRepository
{
    public void AddStandard(int standard);
    public List<string> GetStandards();
    public void RemoveStandard(int standard);
}