using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSystem.DataAccess
{
    public class UserTestingResults
    {
        public int Passed;
        public int Count;
        public double GetPercents()
        {
            if (Count == 0) 
                return Count;
            return Passed / (double)Count * 100;
        }
    }
}
