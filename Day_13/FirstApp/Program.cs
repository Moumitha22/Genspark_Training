using FirstApp.Models;
using FirstApp.Interfaces;
using FirstApp.Repositories;
using FirstApp.Services;

namespace FirstApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IRepository<int, Employee> employeeRepository = new EmployeeRepository();
            IEmployeeService employeeService = new EmployeeService(employeeRepository);
            ManageEmployee manageEmployee = new ManageEmployee(employeeService);
            manageEmployee.Start();
        }
    }
}
