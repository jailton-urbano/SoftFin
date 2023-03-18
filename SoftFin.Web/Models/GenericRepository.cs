using SoftFin.Web.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        public DbControle context;
        public DbSet<TEntity> dbSet;
        public virtual List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState = null)
        {
            var erros = new List<string>();
            if (ModelState != null)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                erros.AddRange(from a in allErrors select a.ErrorMessage);
            }
            return erros;
        }

        public GenericRepository(DbControle context = null)
        {
            if (context == null)
                context = new DbControle();
            
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }
        public virtual TEntity GetByID(int id)
        {
            return dbSet.Find(id);
        }
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == System.Data.Entity.EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = System.Data.Entity.EntityState.Modified;
        }
        public virtual void UpdateNotAtach(TEntity entityToUpdate)
        {
            ;
            context.Entry(entityToUpdate).State = System.Data.Entity.EntityState.Modified;
        }
    }
}
