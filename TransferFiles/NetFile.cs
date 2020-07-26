using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace NetFile
{
    [Serializable]
    public class NetFile
    {
        private byte[] _data;

        public NetFile() { }

        public NetFile(byte[] data)
        {
            NetFile file = FromArray(data);
            FileName = file.FileName;
            Data = file.Data;
        }

        public string FileName { get; set; }

        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                Checksum = GetMD5Hash(_data);
            }
        }

        public string Checksum { get; set; }

        public static string GetMD5Hash(byte[] source)
        {
            StringBuilder hash = new StringBuilder();
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(source);
                for (int index = 0; index < data.Length; index++)
                {
                    hash.Append(data[index].ToString("x2"));
                }

                return hash.ToString();
            }
        }

        public byte[] ToArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);

                return stream.ToArray();
            }
        }

        public static NetFile FromArray(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                stream.Position = 0;
                return (NetFile)formatter.Deserialize(stream);
            }
        }
    }
}
