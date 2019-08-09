using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleNpgsqlBug
{
    public class Parent
    {
        private Parent() { }

        public Guid ParentId { get; private set; }

        public string Name { get; private set; }

        public ICollection<Child> Childs { get; private set; }
        public Guid ConcurrencyToken { get; private set; }

        public Parent(Guid parentId, string name)
        {
            ParentId = parentId;
            Name = name;
            ConcurrencyToken = Guid.NewGuid();
        }

        public void Add(Child child)
        {
            if (Childs == null)
                Childs = new List<Child>();

            Childs.Add(child);
            ConcurrencyToken = Guid.NewGuid();
        }
    }
}
