using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleNpgsqlBug
{
    public class MyRepository
    {
        private readonly MyDbContext _context;

        public MyRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<Parent> Get(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Parents
                 .Include(p => p.Childs) // including or disable including - no matter
                 .SingleOrDefaultAsync(n => n.ParentId == id, cancellationToken);
        }

        public async Task Save(Parent parent)
        {
            var entry = _context.Entry(parent);

            if (entry.State == EntityState.Detached)
                _context.Parents.Add(parent);

            await _context.SaveChangesAsync();
        }
    }
}
