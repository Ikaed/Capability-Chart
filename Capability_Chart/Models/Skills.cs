using System;
using System.Collections.Generic;

namespace Capability_Chart.Models
{
    public partial class Skills
    {
        public Skills()
        {
            AssignedSkill = new HashSet<AssignedSkill>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AssignedSkill> AssignedSkill { get; set; }
    }
}
