/* I am not the author of this code*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

namespace VolumeControlUtility
{
    class SecureJsonSerializer<T>
    where T : class
    {
        private readonly string filePath;
/*
        private readonly ICryptoTransform encryptor;

        private readonly ICryptoTransform decryptor;

        private const string Password = "some password";

        private static readonly byte[] passwordBytes = Encoding.ASCII.GetBytes(Password);
        */
        public SecureJsonSerializer(string filePath)
        {
            this.filePath = filePath;
            //var rmCrypto = GetAlgorithm();
            //this.encryptor = rmCrypto.CreateEncryptor();
            //this.decryptor = rmCrypto.CreateDecryptor();
        }
/*
        private static RijndaelManaged GetAlgorithm()
        {
            var algorithm = new RijndaelManaged();
            int bytesForKey = algorithm.KeySize / 8;
            int bytesForIV = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIV);
            return algorithm;
        }
        */
        public void Save(T obj)
        {
            using (var writer = new StreamWriter(filePath))
            {
                //Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new SerializableContractResolver();
                settings.Error = (serializer, err) => err.ErrorContext.Handled = true;
                writer.Write(JsonConvert.SerializeObject(obj, Formatting.Indented, settings));
            }
        }

        public T Load()
        {
            using (var reader = new StreamReader(filePath))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new SerializableContractResolver();
                //settings.Error = (serializer, err) => err.ErrorContext.Handled = true;
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd(), settings);
            }
        }
    }
}
