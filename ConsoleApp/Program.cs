using Cars;
using ConsoleApp.Common;
using ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
    
            var cars = ProcessCar("./fuel.csv");

            foreach (var car in cars)
            {
                Console.WriteLine(car.Name);
            }

            Console.Read();

            await Task.FromResult(0);
        }

        #region Process Cars
        static IList<Car> ProcessCar(string path)
        {
            var query = File.ReadLines(path)
                        .Select(l =>
                        {
                            var columns = l.Split(",");

                            return new Car()
                            {
                                Year = int.Parse(columns[0]),
                                Manufacturer = columns[1],
                                Name = columns[2],
                                Displacement = double.Parse(columns[3]),
                                Cylinders = int.Parse(columns[4]),
                                City = int.Parse(columns[5]),
                                Highway = int.Parse(columns[6]),
                                Combined = int.Parse(columns[7])
                            };
                        });

            return query.ToList();
        }

        #endregion

        #region ProcessManufacturers
        static IList<Manufacturer> ProcessManufactureres(string path)
        {
            var query = File.ReadAllLines(path)
                            .Where(l => l.Length > 1)
                            .Select(l =>
                            {
                                var columns = l.Split(",");
                                return new Manufacturer()
                                {
                                    Name = columns[0],
                                    Headquarters = columns[1],
                                    Year = int.Parse(columns[2])
                                };
                            });
            return query.ToList();
        }

        #endregion

        #region Filter Where With LINQ Get Movies
        public static void GetMovies()
        {
            var movies = new List<Movie>()
            {
                new Movie() { Title = "The Dark Knight", Rating= 8.9f, Year=2000},
                new Movie() { Title = "Jason Bournet", Rating= 10.0f, Year=2018},
                new Movie() { Title = "Prometheus", Rating= 9.0f, Year=2017},
                new Movie() { Title = "TLegacy Bourne", Rating= 8.2f, Year=2010},
                new Movie() { Title = "Heat to Heat", Rating= 8.5f, Year=1998},

            };

            var query = movies.Where(m => m.Year > 2000);

            foreach (var movie in query)
            {
                Console.WriteLine($"{movie.Title} - {movie.Year}");
            }
        }

        #endregion

        #region Get Developers With Length Name Equal Five
        static void GetDevelopersNames(IEnumerable<Employee> empls)
        {
            //var employees = from q in empls
            //where q.Name.Length >= 5
            //orderby q.Name
            //select q;

            var employees = empls.Where(e => e.Name.Length >= 5)
                                 .OrderBy(e => e.Name)
                                 .Select(e => e);

                        foreach(var emp in employees)
            {
                Console.WriteLine(emp.Name);
            }
        }

        #endregion

        #region Filter With Action
        static void GetWithAction(IEnumerable<Employee> employees)
        {
            //Actions always return void
            Action<int> write = x => Console.WriteLine(x);

            Func<int, int> square = x => x * x;

            Func<int, int, int> add = (int x, int y) =>
            {
                int temp = x + y;
                return temp;
            };

            var ret = square(add(2, 2));

            write(ret);
        }
        #endregion

        #region Filter With Func
        static void GetWithFunc(IEnumerable<Employee> employees)
        {
            Func<int, int> square = x => x * x;

            Func<int, int, int> add = (int x, int y) =>
            {
                int temp = x + y;
                return temp;
            };

            var ret = square(add(2, 2));

            Console.WriteLine($"Valor: {ret}");
        }
        #endregion

        #region Filter with Lambda

        static void GetWithLambda(IEnumerable<Employee> employees)
        {
            foreach (Employee employee in employees.Where(e => e.Name.StartsWith("A", StringComparison.InvariantCulture)))
            {
                Console.WriteLine(employee.Name);
            }
        }

        #endregion Filter with Lambda

        #region While With GetENumerator

        private static void GetWithEnumerator(IEnumerable<Employee> employees)
        {
            IEnumerator<Employee> enumerator = employees.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine($"{enumerator.Current.ToString()}");
            }
        }

        #endregion While With GetENumerator

        #region Filter With Anonymous Type

        public void GetWithAnonymousTypes(IEnumerable<Employee> employees)
        {
            foreach (Employee dev in employees.Where(NameStartWithA))
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

        #region Aux Methods
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

        #endregion
    }
}