using System;
using System.Collections.Generic;

namespace Capability_Chart.Models
{
    public partial class AssignedSkill
    {
        public int Id { get; set; }
        public int? SkillId { get; set; }
        public int? EmpId { get; set; }
        public byte? AssignedScore { get; set; }

        public Employee Emp { get; set; }
        public Skills Skill { get; set; }
    }
}
