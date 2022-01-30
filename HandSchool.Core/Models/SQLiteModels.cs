using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;

namespace HandSchool.Models
{
    [Table("user_accounts")]
    public class UserAccount
    {
        [PrimaryKey] public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotNull] 
        public bool AutoLogin { get; set; }

        public override string ToString()
        {
            return $"ServerName: {ServerName}, UserName: {UserName}";
        }
    }
    

    [Table("server_jsons")]
    public class ServerJson
    {
        [PrimaryKey]
        public string JsonName
        {
            get => _jsonName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("JsonName cannot be blank");
                }

                _jsonName = value;
            }
        }

        private string _jsonName = "";

        public string Json { get; set; }
        
        /// <summary>
        /// 将Json的指定路径对象转化为指定类型的对象。
        /// </summary>
        /// <param name="paths">需要转化成对象所在的路径</param>
        /// <typeparam name="T">输出类型</typeparam>
        /// <returns>转换的结果。如果转换失败，对于值类型，会抛出异常；对于引用类型，会返回null。</returns>
        /// <exception cref="ArgumentException">指定类型为值类型时，当Json为空、path不存在时抛出</exception>
        /// <exception cref="JsonSerializationException">指定类型为值类型时，Json格式错误时抛出</exception>
        public T ToObject<T>(params string[] paths)
        {
            //判断输入类型是值类型还是引用类型
            var valueType = typeof(T).IsValueType;
            
            //Json不能为空
            if (Json is null)
            {
                if (!valueType) return default;
                throw new ArgumentException("Json is null");
            }
            
            //先将Json转化成JToken，然后延输入路径寻找
            var rootJToken = JsonConvert.DeserializeObject<JToken>(Json);
            var jt = rootJToken;
            foreach (var path in paths)
            {
                if (path is null)
                {
                    throw new ArgumentException("Paths cannot be null");
                }

                jt = jt?[path];
            }

            //JToken为空，转换失败
            if (jt is null)
            {
                if (!valueType)
                {
                    return default;
                }

                throw new ArgumentException("One or more paths do not exist in this json");
            }

            try
            {
                return jt.ToObject<T>();
            }
            catch (ArgumentException)
            {
                if (!valueType) return default;
                throw;
            }
            catch (JsonSerializationException)
            {
                if (!valueType) return default;
                throw;
            }
        }
    }

    [Table("config")]
    public class Config
    {
        [PrimaryKey]
        [Column("config_name")]
        public string ConfigName { get; set; }

        [Column("value")]
        public string Value { get; set; }
    }
}