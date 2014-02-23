using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestingSystem.Utils.Data;

namespace TestingSystem.Controls
{
    public class TestSubmitEventArgs
    {
        public TestSubmitEventArgs(List<Pair<int, bool>> testResults)
        {
            if (testResults == null)
            {
                throw new ArgumentNullException("testResults", "Missing object reference.");
            }
            TestResults = testResults;
        }
        public readonly List<Pair<int, bool>> TestResults;
    }
}
