using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WHL.DAL;
using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;

namespace WHL.Services
{
    /// <summary>
    /// User Service：we put all user biz logic handing here~
    /// Author: Pango
    /// </summary>
    public class EmployeeService : BaseService{

        /// <summary>
        /// get a test employee for all test
        /// </summary>
        /// <returns></returns>
        public Employee GetTestEmployee() {

            Employee admin = db.EmployeesDB.First(s => s.Name.Contains("Admin"));
            Employee pango = db.EmployeesDB.First(s=>s.Name.Contains("Pango"));
            Employee lion = db.EmployeesDB.First(s => s.Name.Contains("Lion"));

            return admin;

        }
    }
}