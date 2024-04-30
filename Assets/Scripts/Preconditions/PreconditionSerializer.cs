using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Preconditions
{
    public class PreconditionSerializer
    {
        // public static string Serialize(Precondition precondition)
        // {
        //     switch (precondition)
        //     {
        //         case HasMet hasMet:
        //             return JsonConvert.SerializeObject(new HasMetDTO
        //             {
        //                 RoleOne = hasMet.RoleOne,
        //                 RoleTwo = hasMet.RoleTwo
        //             });
        //             break;
        //         case LessThan lessThan:
        //             break;
        //     }
        //
        //     return "";
        // }

        public static Precondition Deserialize(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            string type = jsonObject["Type"]?.ToObject<string>();

            return type switch
            {
                "HasMet" => new HasMet(jsonObject["RoleOne"]?.ToObject<string>(),
                    jsonObject["RoleTwo"]?.ToObject<string>()),
                "LessThan" => new LessThan(jsonObject["Left"]?.ToObject<float>(),
                    jsonObject["Right"]?.ToObject<float>()),
                "GreaterThan" => new GreaterThan(jsonObject["Left"]?.ToObject<float>(),
                    jsonObject["Right"]?.ToObject<float>()),
                "EqualTo" => new EqualTo(jsonObject["Left"]?.ToObject<float>(), jsonObject["Right"]?.ToObject<float>()),
                "Likes" => new Likes(jsonObject["RoleOne"]?.ToObject<string>(),
                    jsonObject["RoleTwo"]?.ToObject<string>(), jsonObject["MinThreshold"]?.ToObject<float>()),
                "Dislikes" => new Dislikes(jsonObject["RoleOne"]?.ToObject<string>(),
                    jsonObject["RoleTwo"]?.ToObject<string>(), jsonObject["MaxThreshold"]?.ToObject<float>()),
                _ => throw new ArgumentException($"Unsupported precondition type: {type}.")
            };
        }
    }
}