using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class Score
   {
      public int BlindTestId { get; set; }
      public string Line { get; set; }
      public string MinValue { get; set; }
      public string MaxValue { get; set; }

      public static string Parse(List<Score> scores)
      {
         if ((scores == null) || (scores.Count == 0))
         {
            return "";
         }
         //string data = string.Join(",", scores);
         string data = "";
         string sep = "";
         //int cnt = 0;
         foreach (var item in scores)
         {
            data += sep + item.Line;
            //sep = System.Environment.NewLine + ";";
            sep = ",";
         }
         //return "[{\"data\": [" + data + "],\"yAxesMin\":" + scores[0].MinValue + "}]";
         return "{\"data\": [" + data + "],\"MinValue\":" + scores[0].MinValue + ",\"MaxValue\":" + scores[0].MaxValue + "}";
      }
   }
}