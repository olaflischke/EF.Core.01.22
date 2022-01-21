using System;
using System.Collections.Generic;

namespace WorkPlanDalReverse.Model
{
    public partial class Person
    {
        public Person()
        {
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
