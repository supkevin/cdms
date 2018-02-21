using System.Data.Entity;


namespace CDMS.Model.DbContextFactory
{
    public interface IDbContextFactory
    {
        DbContext GetDbContext();
    }
}
