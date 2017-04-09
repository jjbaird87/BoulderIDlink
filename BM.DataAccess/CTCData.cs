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
            var command = new SqlCommand("SELECT FirstName, LastName FROM Member WHERE MemberNumber = @MemberNumber", connection);
            command.Parameters.Add("@MemberNumber", SqlDbType.VarChar).Value = memberNumber;
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (!reader.HasRows)
                    return null;
                while (reader.Read())
                {
                    var member = new Member { FirstName = reader["FirstName"].ToString(), LastName = reader["LastName"].ToString(), MemberNumber = memberNumber};
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
