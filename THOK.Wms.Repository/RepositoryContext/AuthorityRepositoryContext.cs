﻿using System.Data.Entity;
using THOK.Common.Ef.Infrastructure;
using THOK.Common.Ef.Interfaces;
using THOK.Wms.Repository.Interfaces;
using System;

namespace THOK.Wms.Repository.RepositoryContext
{
    public class AuthorityRepositoryContext : IAuthorityRepositoryContext, IRepositoryContext
    {
        private const string OBJECT_CONTEXT_KEY = "THOK.Wms.Repository.AuthorizeContext,THOK.Wms.Repository.dll";
        public DbSet<T> GetDbSet<T>() 
            where T : class
        {
            return ContextManager.GetDbContext(OBJECT_CONTEXT_KEY).Set<T>();
        }

        public DbContext DbContext
        {
            get
            {
                return ContextManager.GetDbContext(OBJECT_CONTEXT_KEY);
            }
        }

        public int SaveChanges()
        {

            try
            {
                return this.DbContext.SaveChanges();
            }catch(System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                return -1;
            }
        }

        public void Terminate()
        {
            ContextManager.SetRepositoryContext(null, OBJECT_CONTEXT_KEY);
        }
    }
}
