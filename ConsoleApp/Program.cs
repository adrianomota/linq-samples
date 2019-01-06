using ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IEnumerable<Employee> devs = GetDevelopersEmployees();

            IEnumerable<Employee> sales = GetSalesEmployees();

            Console.Read();

            await Task.FromResult(0);
        }

        #region Filter with Lambda

        public void GetWithLambda(IEnumerable<Employee> devs)
        {
            foreach (Employee employee in devs.Where(
                e => e.Name.StartsWith("A")))
            {
                Console.WriteLine(e.Name);
            }
        }

        #endregion Filter with Lambda

        #region While With GetENumerator

        private static void GetWithEnumerator(IEnumerable<Employee> devs)
        {
            IEnumerator<Employee> enumerator = devs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine($"{enumerator.Current.ToString()}");
            }
        }

        #endregion While With GetENumerator

        #region Filter With Anonymous Type

        public void GetWithAnonymousTypes(IEnumerable<Employee> devs)
        {
            foreach (Employee dev in devs.Where(NameStartWithA))
            {
                Console.WriteLine(dev.Name);
            }
        }

        #endregion Filter With Anonymous Type

        #region Filter With Delegate

        public void GetWithDelegate(IEnumerable<Employee> devs)
        {
            foreach (Employee dev in devs.Where(
                delegate (Employee arg)
                {
                    return arg.Name.StartsWith("A");
                }))
            {
                Console.WriteLine(dev.Name);
            }
        }

        #endregion Filter With Delegate

        private static bool NameStartWithA(Employee arg)
        {
            return arg.Name.StartsWith("A");
        }

        private static IEnumerable<Employee> GetDevelopersEmployees()
        {
            IEnumerable<Employee> emps = new Employee[]
            {
                new Employee("Adriano"),
                new Employee("Miguel")
            };

            return emps;
        }

        private static IEnumerable<Employee> GetSalesEmployees()
        {
            IEnumerable<Employee> emps = new List<Employee>()
            {
                new Employee("Gleyce")
            };

            return emps;
        }
    }
}