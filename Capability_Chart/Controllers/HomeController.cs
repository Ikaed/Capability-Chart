using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Capability_Chart.Models;
using Microsoft.EntityFrameworkCore;
using Highsoft.Web.Mvc.Charts;

namespace Capability_Chart.Controllers
{
    public class HomeController : Controller
    {
        //Super duper hard coded feature
        private List<Skills> featured_skills = new List<Skills>();
        //Do not remove, breaks program
        private List<int> no_show_list = new List<int>() { 4 };

        private readonly ILogger<HomeController> _logger;
        private capability_chartContext _context;

        public HomeController(ILogger<HomeController> logger, capability_chartContext context)
        {
            _logger = logger;
            _context = context;
        }
        //Spiderchart with the featured skills and team names
        public IActionResult Index(int id)
        {
            //List<string> Skills = new List<string>(); //Test code
            List<LineSeriesData> Ratings = new List<LineSeriesData>(); //Test code
            List<int> EmployeeID = new List<int>();
            List<String> skill_highchart = new List<string>();
            List<LineSeriesData> max_skill_rating = new List<LineSeriesData>();

            var teams = _context.Teams.ToList();
            var employees = _context.Employee.Where(e => e.AssignedTeam == id).ToList();
            var skills = _context.Skills.ToList();
            var assigned_skills = _context.AssignedSkill.ToList();
            int skillCount = 0;
            double total_skill_value = 0;
            //Dictionary<string , double> featured_skills_dict = new Dictionary<string, double>();
            foreach (Employee employee in employees)
            {
                EmployeeID.Add(employee.Id);

            }
            featured_skills = _context.Skills.ToList<Skills>();
            //Fix this
            //Get ALL empid in assigned_skill table ,validate the empid with employee in selected team
            //teamid to get all employees
            //Loop skills table and filter all team members with id, meanwhile get calculated scores
            foreach(Skills skill in skills)
            {
                total_skill_value = 0;
                var assigned_skill = _context.AssignedSkill.Where(e => e.SkillId == skill.Id).ToList();
                int max = 0;
                foreach(AssignedSkill assignedSkill in assigned_skill)
                {
                    foreach(int empid in EmployeeID)
                    {
                        if(empid == assignedSkill.EmpId)
                        {
                            total_skill_value += (double)assignedSkill.AssignedScore;
                            skillCount++;
                            if ((int)assignedSkill.AssignedScore > max)
                                max = (int)assignedSkill.AssignedScore;
                        }
                        
                    }
                }
                if (skillCount == 0)
                    total_skill_value = 0;
                else
                    total_skill_value /= EmployeeID.Count;
                max = max > 5 ? max = 5 : max;
                total_skill_value = total_skill_value > 5 ? total_skill_value = 5 : total_skill_value;
                skill_highchart.Add(skill.Name);
                Ratings.Add(new LineSeriesData() { Y = total_skill_value });
                max_skill_rating.Add(new LineSeriesData() { Y = max });

            }
            ViewData["Max_Skill_Rating"] = max_skill_rating;
            ViewData["Skills"] = skill_highchart;
            ViewData["Ratings"] = Ratings;
            ViewData["Teams"] = teams;
            ViewData["TeamID"] = id;
            
            
            return View();

        }


        public IActionResult TeamAdminPage(int id)
        {
            var teams = _context.Teams.ToList();
            var employees = _context.Employee.ToList();
            var unassignedEmployees = new List<Employee>();

            for (int i = 0; i < employees.Count(); i++)
            {
                if(employees[i].AssignedTeam == null)
                {
                    unassignedEmployees.Add(employees[i]);
                }
            }

            
            var test2 = _context.Employee.Where(e => e.AssignedTeam == id).ToList();


            ViewData["Test"] = test2; //For team members table
            ViewData["TeamID"] = id; 

            ViewData["UnassignedEmployees"] = unassignedEmployees; //For unassigned employee table
            ViewData["Teams"] = teams; //For all teams dropdown
            ViewData["Employee"] = employees;  //For all employees table
            return View();
        }

        public ActionResult addEmployee(String firstname, String lastname)
        {
            _context.Employee.Add(new Employee{ Firstname = firstname, Lastname = lastname});
            _context.SaveChanges();
            return RedirectToAction("TeamAdminPage");
        }

        public ActionResult deleteEmployee(int id)
        {
            var emp = _context.Employee.Where(e => e.Id == id).First();
            _context.Employee.Remove(emp);
            _context.SaveChanges();
            return RedirectToAction("TeamAdminPage");
        }

        public ActionResult addEmployeeToTeam(int empid, int teamid)
        {
            Employee test = new Employee();
            test = _context.Employee.Where(e => e.Id == empid).First();
            test.AssignedTeam = teamid;
            _context.SaveChanges();
            return RedirectToAction("TeamAdminPage", new { id = teamid });
        }

        public ActionResult removeEmployeeFromTeam(int id)
        {
            var emp = _context.Employee.Where(e => e.Id == id).First();
            var teamid = emp.AssignedTeam;
            emp.AssignedTeam = null;
            _context.SaveChanges();
            return RedirectToAction("TeamAdminPage", new { id = teamid});

        }



        public ActionResult addTeam(String teamname)
        {
            _context.Teams.Add(new Teams { Name = teamname });
            _context.SaveChanges();
            return RedirectToAction("TeamAdminPage");
        }

        public ActionResult deleteTeam(int id)
        {
            var team = _context.Teams.Where(e => e.Id == id).First();
            _context.Teams.Remove(team);
            _context.SaveChanges();
            return RedirectToAction("TeamAdminPage");
        }


        public IActionResult SkillAdminPage()
        { 
            var skills = _context.Skills.ToList();
            var assignedSkills = _context.AssignedSkill.ToList();
            var assignedSkills_IDList = new List<int?>();
            var unusedSkills = new List<Skills>();

            //Get all unused skills, which means all skills that have no entry in the assigned skills table
            for (int i = 0; i < skills.Count(); i++)
            {
                for (int u = 0; u < assignedSkills.Count(); u++)
                {
                    assignedSkills_IDList.Add(assignedSkills[u].SkillId);
                }
                if (!assignedSkills_IDList.Contains(skills[i].Id))
                {
                    unusedSkills.Add(skills[i]);
                }
            }



            ViewData["UnusedSkills"] = unusedSkills; //For unused skills table
            ViewData["Skills"] = skills; //For Skill library
            return View();
        }
       
        public ActionResult DelSkill(int id)
        {
           var skill = _context.Skills.Where(e => e.Id == id).First();
           _context.Skills.Remove(skill);
           _context.SaveChanges();
            return RedirectToAction("SkillAdminPage");
        }
        
        public ActionResult addSkill(String skillname)
        {
            _context.Skills.Add(new Skills {Name=skillname});
            _context.SaveChanges();
            return RedirectToAction("SkillAdminPage");
        }

      
        public ActionResult updateSkill(int skillId, string newname)
        {
            Skills test = new Skills();
            test =  _context.Skills.Where(e => e.Id == skillId).First();
            test.Name = newname;
            _context.SaveChanges();
            return RedirectToAction("SkillAdminPage");
        }

        public IActionResult TeamPage(int id)
        {
            List<LineSeriesData> Ratings = new List<LineSeriesData>(); //Test code
            List<int> EmployeeID = new List<int>();
            List<String> skill_highchart = new List<string>();
            List<LineSeriesData> max_skill_rating = new List<LineSeriesData>();

            var employees = _context.Employee.Where(e => e.AssignedTeam == id).ToList();
            var skills = _context.Skills.ToList();
            var assigned_skills = _context.AssignedSkill.ToList();
            int skillCount = 0;
            double total_skill_value = 0;

            featured_skills = _context.Skills.ToList<Skills>();
           
            foreach (Employee employee in employees)
            {
                EmployeeID.Add(employee.Id);

            }

            foreach (Skills skill in skills)
            {
                var assigned_skill = _context.AssignedSkill.Where(e => e.SkillId == skill.Id).ToList();
                int max = 0;
                total_skill_value = 0;

                foreach (AssignedSkill assignedSkill in assigned_skill)
                {
                    foreach (int empid in EmployeeID)
                    {
                        if (empid == assignedSkill.EmpId)
                        {
                            total_skill_value += (double)assignedSkill.AssignedScore;
                            skillCount++;
                            if ((int)assignedSkill.AssignedScore > max)
                                max = (int)assignedSkill.AssignedScore;
                        }

                    }
                }
                if (skillCount == 0)
                    total_skill_value = 0;
                else
                    total_skill_value /= EmployeeID.Count;
                max = max > 5 ? max = 5 : max;
                
                total_skill_value = total_skill_value > 5 ? total_skill_value = 5 : total_skill_value;
                skill_highchart.Add(skill.Name);
                Ratings.Add(new LineSeriesData() { Y = total_skill_value });
                max_skill_rating.Add(new LineSeriesData() { Y = max });

            }
            //Viewdata for highchart here

            //Gets all employees for selected team
            
            //Gets team name of selected team
            var team = _context.Teams.Where(e => e.Id == id).First();
            ViewData["Max_Skill_Rating"] = max_skill_rating;
            ViewData["Skills"] = skill_highchart;
            ViewData["Ratings"] = Ratings;

            ViewData["Teamname"] = team.Name; //For title of the page
            ViewData["Employees"] = employees; //For all team members table
            return View();
        }

        public IActionResult PersonPage(int empid)
        {
            //Needs to be done: Viewdata for highchart, Viewdata for all skills the employee has
            List<LineSeriesData> Ratings = new List<LineSeriesData>();
            //gets selected employees name
            var empname = _context.Employee.Where(e => e.Id == empid).First();

            //gets all the skills and their values for selected employee
            var assignedSkills = _context.AssignedSkill.Where(t => t.EmpId == empid).ToList();

            List<string> skillNames = new List<string>();
            List<string> skillValues = new List<string>();

            foreach (AssignedSkill skill in assignedSkills){
                var skills = _context.Skills.Where(s => s.Id == skill.SkillId).First();
                skillNames.Add(skills.Name);
                skillValues.Add(skill.AssignedScore.ToString());
                Ratings.Add(new LineSeriesData() { Y = skill.AssignedScore });
            }
            ViewData["Ratings"] = Ratings;//data for high chart
            ViewData["skillValues"] = skillValues;
            ViewData["skillNames"] = skillNames;
            ViewData["Employeename"] = empname.Firstname + " " + empname.Lastname; //for h1 title of the page
            ViewData["EmployeeID"] = empid; //for edit person button
            ViewData["TeamID"] = empname.AssignedTeam; //for breadcrumbs
            return View();
        }

        public IActionResult PersonEditPage(int empid)
        {
            //Needs to be done: Viewdata for highchart
            List<LineSeriesData> Ratings = new List<LineSeriesData>();
            List<string> skillNames = new List<string>();
            //gets all skills
            var skills = _context.Skills.ToList();

            //gets selected employee
            Employee employee = _context.Employee.Where(e => e.Id == empid).First();

            //gets all the skills and their values for selected employee
            var assignedSkills = _context.AssignedSkill.Where(t => t.EmpId == empid).ToList();


            foreach (AssignedSkill skill in assignedSkills)
            {
                skill.Skill = _context.Skills.Where(s => s.Id == skill.SkillId).First(); //Assign the skill object to the assignedskill object
                skill.Emp = employee;
                skillNames.Add(skill.Skill.Name);
                Ratings.Add(new LineSeriesData() { Y = skill.AssignedScore });
            }
            ViewData["skillNames"] = skillNames;
            ViewData["Ratings"] = Ratings;//data for high chart
            ViewData["AssignedSkills"] = assignedSkills; //For assigned skills table and to filter out assigned skills in skill library
            ViewData["Skills"] = skills; //For Skill library
            ViewData["firstname"] = employee.Firstname; //For h1 title of page and form to change name
            ViewData["lastname"] = employee.Lastname; //For h1 title of page and form to change name
            ViewData["id"] = empid; //For breadcrumbs and for form to change name
            ViewData["TeamID"] = employee.AssignedTeam; //For breadcrumbs


            return View();
        }

        public ActionResult updateEmployeeName(int empid, string firstname, string lastname)
        {
            var employee = _context.Employee.Where(e => e.Id == empid).First();
            employee.Firstname = firstname;
            employee.Lastname = lastname;
            _context.SaveChanges();
            return RedirectToAction("PersonEditPage", new {empid = empid });
        }

        public ActionResult addSkillToPerson(int empid, int skillid)
        {
            AssignedSkill t = new AssignedSkill { EmpId = empid, SkillId = skillid, AssignedScore = 0 };
            _context.AssignedSkill.Add(t);
            _context.SaveChanges();
            return RedirectToAction("PersonEditPage", new { empid = empid });
        }

        public ActionResult removeSkillFromPerson(int empid, int skillid)
        {
            //Gets all assignedskills for employee
            var list = _context.AssignedSkill.Where(e => e.EmpId == empid).ToList();

            //Finds assignedskill that is to be removed
            foreach (AssignedSkill assignedSkill in list)
            {
                if (assignedSkill.SkillId == skillid)
                {
                    _context.AssignedSkill.Remove(assignedSkill);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("PersonEditPage", new { empid = empid });
        }

        public ActionResult assignScoreToAssignedSkill(int empid, int skillid, int score)
        {
           

            var list = _context.AssignedSkill.Where(e => e.EmpId == empid).ToList();
            score = score > 5 ? score = 5 : score;
            score = score < 0 ? score = 0 : score;

            foreach (AssignedSkill assignedSkill in list)
            {
                if (assignedSkill.SkillId == skillid)
                {
                    
                    assignedSkill.AssignedScore = (byte)score;
                    break;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("PersonEditPage", new { empid = empid });
        }
        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
