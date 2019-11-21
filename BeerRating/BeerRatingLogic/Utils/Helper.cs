using System.Configuration;

namespace BeerRating.BeerRatingLogic.Utils
{
   public static class Helper
   {
      public static string CnnVal(string name)
      {
         // https://www.connectionstrings.com/sql-server/
         return ConfigurationManager.ConnectionStrings[name].ConnectionString;
      }
   }
}