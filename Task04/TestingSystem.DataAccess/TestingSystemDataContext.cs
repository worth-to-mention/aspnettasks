using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using TestingSystem.Utils.Data;

namespace TestingSystem.DataAccess
{
    /// <summary>
    /// Represents a data context for TestingSystem database.
    /// </summary>
    public class TestingSystemDataContext : IDisposable
    {
        private bool disposed;
        private SqlConnection connection;
        /// <summary>
        /// Constructs an instance of TestingSystemDataContext
        /// with specified database connection string and optional SqlCredential.
        /// Also opens connection. Connection will be closed of dispose.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="credential">Credential for connection.</param>
        public TestingSystemDataContext(string connectionString, SqlCredential credential = null)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString", "You must specify a valid connection string.");
            }
            if (credential == null)
                connection = new SqlConnection(connectionString);
            else
                connection = new SqlConnection(connectionString, credential);
            connection.Open();
        }
        /// <summar>
        /// Gets test with specified id from DB.
        /// </summary>
        /// <param name="testID"></param>
        /// <returns></returns>
        public Test GetTest(int testID)
        {
            SqlCommand cmd = new SqlCommand("[dbo].[GetTestData]", connection);
            cmd.Parameters.Add("@testID", SqlDbType.Int).Value = testID;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

             var query =
                from record in reader.AsEnumerable()
                group record by new
                {
                    TestID = record.GetInt32(0),
                    Title = record.GetString(1)
                } into testsRecord
                select new Test
                {
                    TestID = testsRecord.Key.TestID,
                    Title = testsRecord.Key.Title,
                    Questions =
                        (
                            from record in testsRecord
                            group record by new
                            {
                                QuestionID = record.GetInt32(2),
                                QuestionText = record.GetString(3),
                                Type = record.GetString(4)
                            } into questionRecord
                            select new Question
                            {
                                QuestionID = questionRecord.Key.QuestionID,
                                Text = questionRecord.Key.QuestionText,
                                Type = (QuestionType)Enum.Parse(typeof(QuestionType), questionRecord.Key.Type),
                                Options =
                                    (
                                        from record in questionRecord
                                        select new Option
                                        {
                                            OptionID = record.GetInt32(5),
                                            Text = record.GetString(6),
                                            IsAnswer = record.GetBoolean(7)
                                        }
                                    ).ToList()
                            }
                        ).ToList()
                };
            return query.FirstOrDefault();
            
        }
        
        //TODO: Rename List<Test> TestingSystemDataContext::GetTest(); to List<Test> TestingSystemDataContext::GetTests();
        /// <summary>
        /// Get all tests from DB as list of
        /// Test instances with ID and Title.
        /// It does not query question data.
        /// </summary>
        /// <returns></returns>        
        public List<Test> GetTest()
        {
            SqlCommand cmd = new SqlCommand("[dbo].[GetTests]", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            var query =
                from record in reader.AsEnumerable()
                select new Test
                {
                    TestID = reader.GetInt32(0),
                    Title = reader.GetString(1)
                };
            return query.ToList();
        }
        /// <summary>
        /// Gets testing results for
        /// specified user and test from database.
        /// </summary>
        /// <param name="userID">User ID.</param>
        /// <param name="testID">Test ID</param>
        /// <returns>User testing results for the test in UserTestingResults</returns>
        public UserTestingResults GetUserTestingResults(object userID, int testID)
        {
            SqlCommand cmd = new SqlCommand("[dbo].[GetUserTestingResults]", connection);
            cmd.Parameters.Add("@userID", SqlDbType.UniqueIdentifier).Value = userID;
            cmd.Parameters.Add("@testID", SqlDbType.Int).Value = testID;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            var query =
                from res in reader.AsEnumerable()
                select new UserTestingResults
                {
                    Passed = res.GetInt32(0),
                    Count = res.GetInt32(1)
                };
            return query.FirstOrDefault();
        }
        /// <summary>
        /// Gets testing statistics for all of the tests
        /// in database.
        /// </summary>
        /// <returns>Statistics of the testing as list of TestingStats for each test.</returns>
        public List<TesttingStats> GetTestingStats()
        {
            SqlCommand cmd = new SqlCommand("[dbo].[GetTestingStats]", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            var query =
                from record in reader.AsEnumerable()
                group record by new 
                {
                    TestID = record.GetInt32(0),
                    TestTitle = record.GetString(1)
                } into statRecord
                select new TesttingStats
                {
                    TestID = statRecord.Key.TestID,
                    TestTitle = statRecord.Key.TestTitle,
                    AnsweringStat = (
                        from record in statRecord
                        select new Pair<double, int>
                        {
                            First = record.GetDouble(2),
                            Second = record.GetInt32(3)
                        }
                    ).ToList()
                };
            return query.ToList();
        }
        //TODO Make TestingSystemDataContext::SaveUserResults(); returns void.
        /// <summary>
        /// Save user testing results for specified test.
        /// If database already contains such results,
        /// then they will be replaced.
        /// </summary>
        /// <param name="userID">User ID.</param>
        /// <param name="testID">Test ID.</param>
        /// <param name="results">Testing results as a list of Pair(QuestionID, Passed/Failed) values.</param>
        /// <returns></returns>
        public int SaveUserResults(object userID, int testID, List<Pair<int, bool>> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException("results", "The value of results can not be null.");
            }
            if (results.Count == 0)
            {
                throw new ArgumentException("Result list must contain at least one element.", "results");
            }
            DataTable resultsRow = new DataTable();
            resultsRow.Columns.Add("UserID");
            resultsRow.Columns.Add("TestID");
            resultsRow.Columns.Add("QuestionID");
            resultsRow.Columns.Add("Passed");
            foreach(var result in results)
            {
                resultsRow.Rows.Add(userID, testID, result.First, result.Second);
            }
            SqlCommand cmd = new SqlCommand("[dbo].[SaveUserResults]", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userID", SqlDbType.UniqueIdentifier).Value = userID;
            cmd.Parameters.Add("@testID", SqlDbType.Int).Value = testID;
            cmd.Parameters.Add("@resultRows", SqlDbType.Structured).Value = resultsRow;
            return cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Get user id by user name.
        /// If user with specified user name is not
        /// exists in the database, then user will be created
        /// and id of the currently created user will be
        /// returned.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns>User id.</returns>
        public int GetOrCreateUser(string userName)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "[dbo].[GetOrCreateUser]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
            return (int)cmd.ExecuteScalar();
        }

        public void CreateTest(Test test)
        {

        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                        connection = null;
                    }                    
                    disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
