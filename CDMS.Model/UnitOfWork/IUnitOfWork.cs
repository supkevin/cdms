using System;
using System.Data.Entity;

namespace CDMS.Model.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }

        int SaveChange();
    }
}
