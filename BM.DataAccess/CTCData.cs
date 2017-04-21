using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace BM.DataAccess
{
    public class CTCData
    {
        private string photoTest = "<ImageXMLSerialization xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                               "    <Width>354</Width>" +
                               "    <Height>267</Height>" +
                               "    <BMPBytes>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</BMPBytes>" +
                               "</ImageXMLSerialization>";
        
        private readonly string _connectionString;
        public CTCData(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Member GetMemberByMemberId(string memberNumber)
        {
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                "SELECT First, Last, Photo FROM tblMember_v62 WHERE Acct = @Acct AND Active = 1", connection);
            command.Parameters.Add("@Acct", SqlDbType.VarChar).Value = memberNumber;
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
                        FirstName = reader["First"].ToString(),
                        LastName = reader["Last"].ToString(),
                        MemberNumber = memberNumber
                    };
                    var photo = reader["Photo"].ToString();
                    member.Photo = new CTCImage();
                    if (!string.IsNullOrEmpty(photo) && !string.IsNullOrWhiteSpace(photo))
                    {
                        try
                        {                            
                            member.Photo.XMLSerialization = DeserializeImage(photo);
                            member.Photo.ImageType = ImageTypeEnum.XMLSerialization;
                        }
                        catch
                        {
                            //Deserialization failed due to bad data
                            member.Photo.ImageType = ImageTypeEnum.Path;
                            member.Photo.FileName = photo;
                        }
                    }
                    else
                    {
                        member.Photo.ImageType = ImageTypeEnum.None;
                    }

                    return member;
                }
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        private static ImageXMLSerialization DeserializeImage(string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImageXMLSerialization));

            var reader = new StringReader(xmlString);
            var imageXMLSerialization = (ImageXMLSerialization)serializer.Deserialize(reader);
            reader.Close();
            return imageXMLSerialization;
        }
    }



    public class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MemberNumber { get; set; }
        public CTCImage Photo { get; set; }
    }

    public class CTCImage
    {
        public CTCImage()
        {
            ImageType = ImageTypeEnum.None;
        }

        public ImageTypeEnum ImageType { get; set; }
        public string FileName { get; set; }
        public ImageXMLSerialization XMLSerialization { get; set; }
    }

    public enum ImageTypeEnum
    {
        None,
        XMLSerialization,
        Path
    }

    [Serializable]
    public class ImageXMLSerialization
    {
        [System.Xml.Serialization.XmlElement("Width")]
        public int Width { get; set; }
        [System.Xml.Serialization.XmlElement("Height")]
        public int Height { get; set; }
        [System.Xml.Serialization.XmlElement("BMPBytes")]
        public byte[] BMPBytes { get; set; }
    }
}
