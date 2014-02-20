using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data;


namespace TestingSystem.DataAccess
{
    public static class SqlDataReaderExtentions
    {
        /// <summary>
        /// Unfolds SqlDataReader as IEnumerable.
        /// </summary>
        /// <param name="reader">A SqlDataReader to unfold.</param>
        /// <returns>A sequecnce of IDataRecord from SqlDataReader.</returns>
        public static IEnumerable<IDataRecord> AsEnumerable(this SqlDataReader reader)
        {
            foreach(IDataRecord record in reader)
            {
                yield return record;
            }
        }
    }
}
