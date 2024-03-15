using System;
using GameSetup;

namespace CCSS
{
    public class Card
    {
        public readonly CardTemplate CardTemplate;
        public readonly Character[] Characters;

        public Character Role(string roleName)
        {
            var roleNumber = Array.IndexOf(CardTemplate.Roles, roleName);
            return Characters[roleNumber];
        }
    }
}