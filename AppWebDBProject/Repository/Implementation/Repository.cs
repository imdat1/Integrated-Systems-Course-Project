using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> entities;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            entities = context.Set<T>();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            _context.SaveChanges();
        }

        public T Get(Guid? id)
        {
            if (typeof(T).IsAssignableFrom(typeof(Database)))
            {
                return entities
                .Include("Questions")
                .SingleOrDefault(s => s.Id == id);
            }
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public T Get(BaseEntity id)
        {
            if (typeof(T).IsAssignableFrom(typeof(Database)))
            {
                return entities
                .Include("Questions")
                .SingleOrDefaultAsync(s => s.Id == id.Id).Result;
            }
            else return entities.SingleOrDefaultAsync(s => s.Id == id.Id).Result;
        }

        public IEnumerable<T> GetAll()
        {
            if (typeof(T).IsAssignableFrom(typeof(Database)))
            {
                return entities
                .Include("Questions")
                .AsEnumerable();
            }
            return entities.AsEnumerable();
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            _context.SaveChanges();
        }
    }
}
