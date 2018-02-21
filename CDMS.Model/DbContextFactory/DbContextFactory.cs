﻿using System;
using System.Data.Entity;
namespace CDMS.Model.DbContextFactory
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly string _ConnectionString = string.Empty;

        public DbContextFactory(string connectionString)
        {
            this._ConnectionString = connectionString;
        }

        private DbContext _dbContext;
        private DbContext dbContext
        {
            get
            {
                if (this._dbContext == null)
                {
                    Type t = typeof(DbContext);
                    this._dbContext =
                        (DbContext)Activator.CreateInstance(t, this._ConnectionString);
                }
                return _dbContext;
            }
        }

        public DbContext GetDbContext()
        {
            return this.dbContext;
        }

    }
}
