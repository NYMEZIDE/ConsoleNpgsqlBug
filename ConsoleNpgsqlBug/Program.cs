using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleNpgsqlBug
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionstring = "Server=localhost;Port=5432;Database=testBug;User Id=fog;Password=GN25giyWrx6fgG;";

            var parentId = Guid.NewGuid();

            using (MyDbContext context = new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().UseNpgsql(connectionstring).Options))
            {
                context.Database.EnsureCreated();
                context.Database.Migrate();

                MyRepository repository = new MyRepository(context);

                var parent = await repository.Get(parentId, default(CancellationToken));

                if (parent == null)
                    parent = new Parent(parentId, "parent name");

                parent.Add(new Child("child 1"));

                await repository.Save(parent);
            }

            using (MyDbContext context2 = new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().UseNpgsql(connectionstring).Options))
            {
                MyRepository repository = new MyRepository(context2);

                var parent = await repository.Get(parentId, default(CancellationToken));

                if (parent == null)
                    throw new Exception("parent not found");

                parent.Add(new Child("child 2"));

                await repository.Save(parent); // "Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted 
                //since entities were loaded. See http://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions."
            }
        }
    }
}
