using System.Text;
using YamlDotNet.Serialization;

namespace svp2lab_Converter
{
    public static class YamlLoadUtil
    {
        // 参考：https://qiita.com/YoshijiGates/items/fc33bbb9cc6d377806e1

        private static readonly IDeserializer _deserializer;
        private static readonly EncodingProvider _provider;
        private static readonly Encoding _encoding;

        static YamlLoadUtil()
        {
            _provider = CodePagesEncodingProvider.Instance;
            _encoding = Encoding.GetEncoding("utf-8");
            _deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        }

        public static T LoadFile<T>(string filename) where T : class
        {
            if (!File.Exists(filename))
            {
                return null;
            }
            try
            {
                var input = new StreamReader(filename, _encoding);
                return _deserializer.Deserialize<T>(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static T Load<T>(string input) where T : class
        {
            try
            {
                return _deserializer.Deserialize<T>(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
