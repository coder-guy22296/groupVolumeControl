using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
namespace VolumeControlUtility
{
    /// <summary>
    /// A resolver that will serialize all properties, and ignore custom TypeConverter attributes.
    /// </summary>
    public class SerializableContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            foreach (var p in properties)
                p.Ignored = false;

            return properties;
        }

        protected override Newtonsoft.Json.Serialization.JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (contract is Newtonsoft.Json.Serialization.JsonStringContract)
                return CreateObjectContract(objectType);
            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member,
                                        MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            //Console.WriteLine("dont worry, im right here");

            if (property.DeclaringType == typeof(System.Int32) &&
                property.PropertyName == "ExitCode")
            {
                property.ShouldSerialize = instanceOfProblematic => false;
            }/*
            property.ShouldSerialize = instance =>
            {
                try
                {
                    PropertyInfo prop = (PropertyInfo)member;
                    if (prop.CanRead)
                    {
                        prop.GetValue(instance, null);
                        return true;
                    }
                }
                catch
                {
                }
                return false;
            };*/
            return property;
        }
    }
}
