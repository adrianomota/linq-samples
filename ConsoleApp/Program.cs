using Cars;
using ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Data");

            var cars = ProcessCar($"{path}\\fuel.csv");

            var manufacturers = ProcessManufactureres($"{path}\\manufacturers.csv");

            QueryXml();
            Console.Read();

            // await Task.FromResult(0);
        }

        #region QueryXMl

        private static void QueryXml()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Data");

            var document = XDocument.Load($"{path}\\fuel.xml");

            var query =
                from element in document.Descendants("Car")
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;

            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        #endregion QueryXMl

        #region Create Xml File

        private static void CreateXml(IEnumerable<Car> cars)
        {
            var document = new XDocument();
            //var carsEl = new XElement("Cars");
            //foreach (var item in cars)
            //{
            //    var car = new XElement("Car", new XAttribute("Name", item.Name),
            //                                  new XAttribute("Combine", item.Combined),
            //                                  new XAttribute("Manufacturer", item.Manufacturer));
            //    carsEl.Add(car);
            //}

            var carsEl = new XElement("Cars",
                    from record in cars
                    select new XElement("Car",
                                               new XAttribute("Name", record.Name),
                                               new XAttribute("Combine", record.Combined),
                                               new XAttribute("Manufacturer", record.Manufacturer))
                );

            document.Add(carsEl);
            document.Save("fuel.xml");
        }

        #endregion Create Xml File

        #region Efficient Aggregation With Extension Methods

        private static void EffientAggregation(IEnumerable<Car> cars)
        {
            //faço três loops, um para cada agregação, e calculo as agregações
            //var query =
            // from car in cars
            // group car by car.Manufacturer into carGroup
            // select new
            // {
            //     Name = carGroup.Key,
            //     Min = carGroup.Min(m => m.Combined),
            //     Max = carGroup.Max(m => m.Combined),
            //     Avg = carGroup.Average(m => m.Combined),
            // } into result
            // orderby result.Max descending
            // select result;

            //foreach (var result in query)
            //{
            //    Console.WriteLine($"{result.Name}");
            //    Console.WriteLine($"\t Min: {result.Min}");
            //    Console.WriteLine($"\t Max: {result.Max}");
            //    Console.WriteLine($"\t Avg: {result.Avg}");
            //}

            //faço um único loop e calculo as agregações
            var query2 =
                cars.GroupBy(c => c.Manufacturer)
                    .Select(g =>
                    {
                        var results = g.Aggregate(new CarStatistics(),
                                        (acc, c) => acc.Accumulate(c),
                                        acc => acc.Compute());

                        return new
                        {
                            Name = g.Key,
                            Min = results.Min,
                            Max = results.Max,
                            Avg = results.Average
                        };
                    })
                    .OrderByDescending(r => r.Max);

            foreach (var result in query2)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Min: {result.Min}");
                Console.WriteLine($"\t Max: {result.Max}");
                Console.WriteLine($"\t Avg: {result.Avg}");
            }
        }

        #endregion Efficient Aggregation With Extension Methods

        #region Aggregating Data

        private static void AggregatingData(IEnumerable<Car> cars)
        {
            var query =
             from car in cars
             group car by car.Manufacturer into carGroup
             select new
             {
                 Name = carGroup.Key,
                 Min = carGroup.Min(m => m.Combined),
                 Max = carGroup.Max(m => m.Combined),
                 Avg = carGroup.Average(m => m.Combined),
             } into result
             orderby result.Max descending
             select result;

            foreach (var result in query)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Min: {result.Min}");
                Console.WriteLine($"\t Max: {result.Max}");
                Console.WriteLine($"\t Avg: {result.Avg}");
            }
        }

        #endregion Aggregating Data

        #region Group By

        private static void GroupByCoutry(IEnumerable<Car> cars, IEnumerable<Manufacturer> manufacturers)
        {
            var query =
               from manufacturer in manufacturers
               join car in cars on manufacturer.Name equals car.Manufacturer
                   into carGroup
               select new
               {
                   Manufacturer = manufacturer,
                   Cars = carGroup
               } into result
               group result by result.Manufacturer.Headquarters;

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Key}");

                foreach (var car in group.SelectMany(s => s.Cars)
                                         .OrderByDescending(o => o.Combined))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void GroupingCarsForHierarchicalData(IEnumerable<Car> cars, IEnumerable<Manufacturer> manufacturers)
        {
            var query =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                select new
                {
                    Manufacturer = manufacturer,
                    Cars = carGroup
                };

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");

                foreach (var car in group.Cars.OrderByDescending(d => d.Combined))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            var query2 = manufacturers
                .GroupJoin(cars, m => m.Name, c => c.Manufacturer,
                (m, g) =>
                     new
                     {
                         Manufacturer = m,
                         Cars = g
                     })
                 .OrderBy(m => m.Manufacturer.Name);

            foreach (var group in query2)
            {
                // Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");

                foreach (var car in group.Cars.OrderByDescending(d => d.Combined))
                {
                    // Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void GroupCars(IEnumerable<Car> cars)
        {
            var query =
                from car in cars
                group car by car.Manufacturer.ToUpper() into manufacturer
                orderby manufacturer.Key
                select manufacturer;

            foreach (var group in query)
            {
                Console.Write(group.Key);

                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        #endregion Group By

        #region Execute Join Cars and Manufacturers

        private static void JoinCarsAndManufacturers(IEnumerable<Car> cars, IEnumerable<Manufacturer> manufacturers)
        {
            //with select
            var query = from car in cars
                        join manufacturer in manufacturers on car.Manufacturer equals manufacturer.Name
                        orderby car.Combined descending, car.Name ascending
                        select new
                        {
                            manufacturer.Headquarters,
                            car.Name,
                            car.Combined,
                            manufacturer.Year
                        };

            query.ToList().ForEach(p =>
            {
                //Console.WriteLine($"{p.Name}-{p.Headquarters}-{p.Year}");
            });

            //with method syntax
            var query2 = cars.Join(manufacturers,
                                   c => c.Manufacturer,
                                   m => m.Name, (c, m) => new
                                   {
                                       m.Headquarters,
                                       c.Name,
                                       c.Combined
                                   })
                        .OrderByDescending(c => c.Combined)
                        .ThenBy(c => c.Name);

            query2.ToList()
                  .ForEach(x =>
                  {
                      // Console.WriteLine($"{x.Name}-{x.Headquarters}-{x.Combined}");
                  });

            //with composiye key
            var query3 = from car in cars
                         join manufacturer in manufacturers
                         on new { car.Manufacturer, car.Year }
                                 equals
                                 new { Manufacturer = manufacturer.Name, manufacturer.Year }
                         orderby car.Combined descending, car.Name ascending
                         select new
                         {
                             manufacturer.Headquarters,
                             car.Name,
                             car.Combined,
                             manufacturer.Year
                         };

            query3.ToList().ForEach(p =>
            {
                //Console.WriteLine($"{p.Name}-{p.Headquarters}-{p.Year}");
            });

            //with method syntax composite key
            var query4 = cars.Join(manufacturers,
                                   c => new { c.Manufacturer, c.Year },
                                   m => new { Manufacturer = m.Name, m.Year },
                                   (c, m) => new
                                   {
                                       m.Headquarters,
                                       c.Name,
                                       c.Combined
                                   })
                        .OrderByDescending(c => c.Combined)
                        .ThenBy(c => c.Name);

            query4.ToList()
                  .ForEach(x =>
                  {
                      Console.WriteLine($"{x.Name}-{x.Headquarters}-{x.Combined}");
                  });
        }

        #endregion Execute Join Cars and Manufacturers

        #region Process Cars

        private static IEnumerable<Car> ProcessCar(string path)
        {
            var query = File.ReadLines(path, System.Text.Encoding.UTF8)
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

        #endregion Process Cars

        #region ProcessManufacturers

        private static IEnumerable<Manufacturer> ProcessManufactureres(string path)
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

        #endregion ProcessManufacturers

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

        #endregion Filter Where With LINQ Get Movies

        #region Get Developers With Length Name Equal Five

        private static void GetDevelopersNames(IEnumerable<Employee> empls)
        {
            //var employees = from q in empls
            //where q.Name.Length >= 5
            //orderby q.Name
            //select q;

            var employees = empls.Where(e => e.Name.Length >= 5)
                                 .OrderBy(e => e.Name)
                                 .Select(e => e);

            foreach (var emp in employees)
            {
                Console.WriteLine(emp.Name);
            }
        }

        #endregion Get Developers With Length Name Equal Five

        #region Filter With Action

        private static void GetWithAction(IEnumerable<Employee> employees)
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

        #endregion Filter With Action

        #region Filter With Func

        private static void GetWithFunc(IEnumerable<Employee> employees)
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

        #endregion Filter With Func

        #region Filter with Lambda

        private static void GetWithLambda(IEnumerable<Employee> employees)
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

        #endregion Aux Methods
    }
}