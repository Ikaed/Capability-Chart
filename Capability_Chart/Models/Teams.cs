using System;
using System.Collections.Generic;

namespace Capability_Chart.Models
{
    public partial class Teams
    {
        public Teams()
        {
            Employee = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employee { get; set; }
    }
}
