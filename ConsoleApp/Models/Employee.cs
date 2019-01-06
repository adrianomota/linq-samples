using System;

namespace ConsoleApp.Models
{
    internal class Employee
    {
        private Employee()
        {
            Id = Guid.NewGuid();
        }

        public Employee(string name) : this()
        {
            Name = name;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            return $"Id: {Id} Name: {Name}";
        }
    }
}