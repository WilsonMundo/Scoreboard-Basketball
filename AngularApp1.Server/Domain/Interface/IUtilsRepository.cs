namespace AngularApp1.Server.Domain.Interface
{
    public interface IUtilsRepository
    {
        long getSequence(string tableName);
    }
}
