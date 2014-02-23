using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestingSystem.Utils.Data;

namespace TestingSystem.DataAccess
{
    public class TesttingStats
    {
        public int TestID { get; set; }
        public string TestTitle { get; set; }
        public List<Pair<double, int>> AnsweringStat;
    }
}
