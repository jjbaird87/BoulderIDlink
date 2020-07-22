using System;
using System.Data;
using System.Data.SqlClient;

namespace BM.DataAccess
{
    public class CtcData
    {
        private readonly string _connectionString;
        public CtcData(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Member GetMemberByMemberId(string memberNumber)
        {
            //Normalize the value
            var split = memberNumber.Split('-');
            memberNumber = split[0].Length < 4 ? split[0].PadLeft(4, '0') : split[0];
            memberNumber = memberNumber.TrimStart('R');
            if (split.Length > 1)
            {
                memberNumber = memberNumber.PadRight(6, '0');
                memberNumber += $"{split[1]}";
            }
            else
            {
                memberNumber = memberNumber.PadRight(7, '0');
            }

            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                "SELECT FirstName, LastName, Photo FROM Members " +
                "WHERE (ID=@ID OR WiegandUserValue1=@ID) AND Active = 'Active'", connection);
            command.Parameters.Add("@ID", SqlDbType.VarChar).Value = memberNumber;
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (!reader.HasRows)
                    return null;
                while (reader.Read())
                {
                    var member = new Member
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        MemberNumber = memberNumber,
                        Photo = reader["Photo"] == DBNull.Value ? null : (byte[])reader["Photo"]
                    };


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
        public string Action { get; set; }
    }
}
