using System;

namespace Day_12
{
    class Employee : IComparable<Employee>
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }

        public Employee() { }

        public Employee(int id, int age, string name, double salary)
        {
            Id = id;
            Age = age;
            Name = name;
            Salary = salary;
        }

        public void TakeEmployeeDetailsFromUser()
        {
            Id = UserInputHelper.GetValidPositiveInt("Enter Employee ID: ");
            Name = UserInputHelper.GetValidString("Enter Employee Name: ");
            Age = UserInputHelper.GetValidPositiveInt("Enter Employee Age: ");
            Salary = UserInputHelper.GetValidDouble("Enter Employee Salary: ");
        }


        public int CompareTo(Employee other)
        {
            return other.Salary.CompareTo(this.Salary);
        }

        public override string ToString()
        {
            return $"ID: {Id} | Name: {Name} | Age: {Age} | Salary: {Salary}";
        }
    }
}
