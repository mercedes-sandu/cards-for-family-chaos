namespace Preconditions
{
    public class HasMetDTO
    {
        public string Type { get; set; } = "HasMet";
        public string RoleOne { get; set; }
        public string RoleTwo { get; set; }
    }

    public class LessThanDTO
    {
        public string Type { get; set; } = "LessThan";
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public class GreaterThanDTO
    {
        public string Type { get; set; } = "GreaterThan";
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public class EqualToDTO
    {
        public string Type { get; set; } = "EqualTo";
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public class LikesDTO
    {
        public string Type { get; set; } = "Likes";
        public string RoleOne { get; set; }
        public string RoleTwo { get; set; }
        public float MinThreshold { get; set; }
    }
    
    public class DislikesDTO
    {
        public string Type { get; set; } = "Dislikes";
        public string RoleOne { get; set; }
        public string RoleTwo { get; set; }
        public float MaxThreshold { get; set; }
    }
}