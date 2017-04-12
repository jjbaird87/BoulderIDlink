using System.Data;
using System.Data.SqlClient;

namespace BM.DataAccess
{    
    public class CTCData
    {
        private readonly string _connectionString;
        public CTCData(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Member GetMemberByMemberId(string memberNumber)
        {
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT First, Last, Photo FROM Member WHERE Acct = @Acct", connection);
            command.Parameters.Add("@Acct", SqlDbType.VarChar).Value = memberNumber;
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (!reader.HasRows)
                    return null;
                while (reader.Read())
                {
                    var member = new Member { FirstName = reader["First"].ToString(), LastName = reader["Last"].ToString(), MemberNumber = memberNumber};
                    return member;
                }
                return null;
            }
            finally
            {
                connection.Close();
            }            
        }
    }

    public class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MemberNumber { get; set; }
        public byte[] Photo { get; set; }
    }
}
