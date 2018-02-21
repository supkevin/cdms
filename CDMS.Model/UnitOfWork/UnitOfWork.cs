using CDMS.Model.DbContextFactory;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace CDMS.Model.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextFactory DbContextFactory { get; set; }

        private DbContext _context;

        public DbContext Context
        {
            get
            {
                if (this._context != null)
                {
                    return this._context;
                }
                this._context = DbContextFactory.GetDbContext();
                return this._context;
            }
        }

        public UnitOfWork(IDbContextFactory factory)
        {
            this.DbContextFactory = factory;
        }

        public int SaveChange()
        {
            try
            {
                return this.Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                //http://stackoverflow.com/questions/15820505/dbentityvalidationexception-how-can-i-easily-tell-what-caused-the-error

                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("\n", errorMessages);
                throw new DbEntityValidationException(fullErrorMessage, ex.EntityValidationErrors);
            }
            catch (System.Data.DataException ex)
            {                
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                    this._context = null;
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
