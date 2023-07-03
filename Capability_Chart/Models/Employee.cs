using System;
using System.Collections.Generic;

namespace Capability_Chart.Models
{
    public partial class Employee
    {
        public Employee()
        {
            AssignedSkill = new HashSet<AssignedSkill>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int? AssignedTeam { get; set; }

        public Teams AssignedTeamNavigation { get; set; }
        public ICollection<AssignedSkill> AssignedSkill { get; set; }
    }
}
