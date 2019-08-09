using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleNpgsqlBug
{
    public class Child
    {
        private Child() { }
        public Guid ChildId { get; private set; }
        public string Name { get; private set; }

        public Guid ParentId { get; private set; }

        public Child(string name)
        {
            ChildId = Guid.NewGuid();
            Name = name;
        }
    }
}
