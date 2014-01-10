using System;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;






namespace Loquat.Service.Business
{
    public abstract class BusinessBase<TDataAccess, TEntity, TKey, TDataContext>
        where TDataAccess : DataAccessBase<TEntity, TKey, TDataContext>, new()
        where TEntity : DataContractBase<TKey>
        where TDataContext : DataContext, new()
    {
        protected static readonly TDataAccess DAO = new TDataAccess();

        public virtual void Create(TEntity entity)
        {
            try
            {
                ValidateRequiredField(entity);
                CheckForCreate(entity);
                DAO.Create(entity);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }

        public virtual void BatchCreate(List<TEntity> entityList)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (TEntity entity in entityList)
                    {
                        ValidateRequiredField(entity);
                        CheckForCreate(entity);
                        DAO.Create(entity);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }

        public virtual void Delete(TKey key)
        {
            try
            {
                DAO.Delete(key);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }

        }
        public virtual void DeleteAll()
        {
            try
            {
                DAO.DeleteAll();
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }

        }
        public virtual void BatchDelete(List<TKey> keyList)
        {
            try
            {
                DAO.BatchDelete(keyList);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }

        }
        public virtual void Update(TEntity entity)
        {
            try
            {
                ValidateRequiredField(entity);
                TEntity oEntity = CheckForUpdate(entity);
                DAO.Update(entity, oEntity);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }

        }

        public virtual void BatchUpdate(List<TEntity> entityList)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (TEntity entity in entityList)
                    {
                        ValidateRequiredField(entity);
                        TEntity oEntity = CheckForUpdate(entity);
                        DAO.Update(entity, oEntity);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }


        public virtual TEntity Get(TKey key)
        {
            try
            {
                return DAO.Get(key);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }

        public virtual List<TEntity> GetAll()
        {
            try
            {
                return DAO.GetAll();
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }
        public virtual QueryResultInfo<TEntity> Query(QueryPropertyInfo<TEntity> queryProperty)
        {
            try
            {
                return DAO.Query(queryProperty);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }

        public virtual List<TEntity> QueryList(TEntity entity)
        {
            try
            {
                return DAO.QueryList(entity);
            }
            catch (Exception ex)
            {
                throw ExceptionFactory.BuildException(ex);
            }
        }


        #region Tool Methods
        protected virtual void ValidateRequiredField(TEntity entity)
        {
        }

        protected virtual void CheckForCreate(TEntity entity)
        {
        }

        protected virtual TEntity CheckForUpdate(TEntity entity)
        {
            return DAO.Get(entity.Key);
        }
        #endregion
    }
}
