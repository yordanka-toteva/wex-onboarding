using System.Xml.Serialization;
using System.IO;
using wex_onboarding.XMLToObject;

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
