**Microsoft.EntityFrameworkCore 3.0.0-preview7.19362.6
Npgsql.EntityFrameworkCore.PostgreSQL 3.0.0-preview7**

I have 2 classes:
```csharp
public class Parent 
{
        private Parent () { }
        public Guid ParentId { get; private set; }
        public ICollection<Child> Childs { get; private set; }
        public Guid ConcurrencyToken { get; private set; }

        public void Add(Child child)
        {
              Childs.Add(child);
              ConcurrencyToken = Guid.NewGuid();
        }
}

public class Child
{
        private Child() { }
        public Guid ChildId { get; private set; }
        public string Name { get; private set; }
        public Guid ParentId { get; private set; }
}
```

and the default implementation of a repository pattern:

```csharp
public async Task<Parent > Get(Guid id, CancellationToken cancellationToken)
{
      return await _context.Parents
           .Include(p => p.Childs) // including or disable including - no matter
           .SingleOrDefaultAsync(n => n.ParentId == id, cancellationToken);
}

public async Task Save(Parent parent )
{
      var entry = _context.Entry(parent);
            
      if (entry.State == EntityState.Detached)
            _context.Parents.Add(parent);

     await _context.SaveChangesAsync();
}
```

If I create a new Parent instance and insert a new Child instance I get two sql inserts to the database(**INSERT one Parent and INSERT one Child**) which is all right, but when I append one more Child to the existing Parent instance with already/or not filled Childs **I get the following error:**
```csharp
Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException: Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. See http://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
   at Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.ConsumeAsync(RelationalDataReader reader, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(DbContext _, ValueTuple`2 parameters, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(DbContext _, ValueTuple`2 parameters, CancellationToken cancellationToken)
   at Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
```

**Created SQL-statements (from logs):**
```sql
"UPDATE \"Parent\" SET \"ConcurrencyToken\" = @p0
WHERE \"ParentId\" = @p1 AND \"ConcurrencyToken\" = @p2;

UPDATE \"Child\" SET \"Name \" = @p3
WHERE \"ChildId \" = @p4;"
```

It tries to update the last appended Child instead of inserting it.