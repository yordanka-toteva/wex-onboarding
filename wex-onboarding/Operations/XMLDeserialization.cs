using System.IO;
using System.Xml.Serialization;

namespace wex_onboarding.Operations
{
    public static class XMLDeserialization
    {
        public static T DeserializeToObject<T>(string filePath) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return (T)xmlSerializer.Deserialize(streamReader);
            }
        }
    }
}
