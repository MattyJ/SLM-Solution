using Fujitsu.SLM.Data;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Extensions
{
    public static class UsersExtensions
    {
        public static Dictionary<string, string> GetUsersDictionary(this Dictionary<string, string> value)
        {
            using (var targetUserDb = new IdentityDataContext("Name=SLMTargetUserDataContext"))
            {
                return targetUserDb.Users.Where(x => x.Id != null).ToDictionary(field => field.Email, field => field.Id);
            }
        }
    }
}
