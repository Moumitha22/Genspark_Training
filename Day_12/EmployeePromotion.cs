using System;
using System.Collections.Generic;

namespace Day_12
{
    class EmployeePromotion
    {
        private List<string> promotionList = new List<string>();

        public void EnterEmployeeNames()
        {
            ConsoleHelper.PrintGreen("\nPlease enter the employee names in the order of their eligibility for promotion\n");
            ConsoleHelper.PrintGreen("Please enter blank to stop\n");

            string input;
            while (true)
            {
                ConsoleHelper.PrintGreen("Enter employee name: ");
                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }
                promotionList.Add(input);
            }
            ConsoleHelper.PrintBlue("\nPromotion List of Employees:");
            foreach (var name in promotionList)
            {
                ConsoleHelper.PrintBlue(name);
            }
        }
       
        public void FindEmployeePosition()
        {
            if (promotionList.Count == 0)
            {
                ConsoleHelper.PrintYellow("\nNo employees in the promotion list.");
                return;
            }

            string employeeName = UserInputHelper.GetValidString("\nPlease enter the name of the employee to check promotion position: ");

            int position = promotionList.IndexOf(employeeName);

            if (position == -1)
            {
                ConsoleHelper.PrintRed($"Employee '{employeeName}' not found in the promotion list.");
            }
            else
            {
                ConsoleHelper.PrintBlue($"'{employeeName}' is in position {position + 1} for promotion.");
            }
        }

        public void ShrinkListMemory()
        {
            ConsoleHelper.PrintBlue($"\nThe current size of the collection is {promotionList.Capacity}");
            promotionList.TrimExcess();
            ConsoleHelper.PrintBlue($"The size after removing the extra space is {promotionList.Capacity}\n");
        }

        public void DisplayPromotedEmployees()
        {
            List<string> sortedList = new(promotionList);
            sortedList.Sort();

            ConsoleHelper.PrintBlue("\nPromoted employee list:\n");
            
            foreach (var name in sortedList)
            {
                ConsoleHelper.PrintBlue($"{name}");
            }
        }

       
    }
}
