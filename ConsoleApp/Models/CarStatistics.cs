using Cars;
using System;

namespace ConsoleApp.Models
{
    internal class CarStatistics
    {
        public CarStatistics()
        {
            Min = int.MinValue;
            Max = int.MaxValue;
        }

        public int Min { get; set; }
        public int Max { get; set; }
        public double Average { get; set; }

        public int Count { get; set; }
        public int Total { get; set; }

        internal CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combined;
            Min = Math.Min(Min, car.Combined);
            Max = Math.Max(Max, car.Combined);
            return this;
        }

        internal CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }
    }
}