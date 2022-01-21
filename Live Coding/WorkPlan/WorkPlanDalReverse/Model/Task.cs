using System;
using System.Collections.Generic;

namespace WorkPlanDalReverse.Model
{
    public partial class Task
    {
        public Task()
        {
            Workers = new HashSet<Person>();
        }

        public int Id { get; set; }
        public string Description { get; set; } = null!;

        public virtual ICollection<Person> Workers { get; set; }
    }
}
