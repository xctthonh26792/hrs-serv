using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Tenjin.Helpers;

namespace Tenjin.Reflections.Serializations
{
    public class SnakeCaseConvention : ConventionBase, IMemberMapConvention
    {
        private SnakeCaseConvention()
        {
        }

        public static SnakeCaseConvention Create()
        {
            return new SnakeCaseConvention();
        }

        public void Apply(BsonMemberMap member)
        {
            member.SetElementName(member.MemberName.ToRegular());
        }
    }
}
