using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class Result
   {
      public int BlindTestId { get; set; }
      public int ParticipantId { get; set; }
      public int RoundNo { get; set; }
      public string BrandName { get; set; }
      public double ABV { get; set; }
      public double Rangering { get; set; }
      public double Snitt { get; set; }
      public string Name { get; set; }
      public double? Vote { get; set; }
      public int Overridden { get; set; }
      public double SnittPerson { get; set; }

      public Result()
      {
         BrandName = Name = "";
      }

      public static string Parse(List<Result> results)
      {
         if ((results == null) || (results.Count == 0))
         {
            return "";
         }
         List<Result> l = new List<Result>();
         List<string> lines = new List<string>();
         string line = "";
         int round = 0;
         int count = 0;
         void a(int x)
         {
            lines.Add((line + "}").Replace("¤", "\""));
            line = "";
            round = x;
         }
         Dictionary<int, KeyValuePair<string, double>> person_avg = new Dictionary<int, KeyValuePair<string, double>>();
         foreach (var item in results)
         {
            if (!person_avg.ContainsKey(item.ParticipantId))
            {
               person_avg.Add(item.ParticipantId, new KeyValuePair<string, double>(item.Name, item.SnittPerson));
            }
            count++;
            if (round == 0)
            {
               round = item.RoundNo;
            }
            if (round != item.RoundNo)
            {
               a(item.RoundNo);
            }

            if (string.IsNullOrEmpty(line))
            {
               line = $"{{¤Rank¤: { item.Rangering },¤Merke¤: ¤{ item.BrandName }¤, ¤ABV¤: ¤{ item.ABV.ToString("#0.0", System.Globalization.CultureInfo.GetCultureInfo(1033)) }¤,¤Snitt¤: ¤{ item.Snitt.ToString("#0.00", System.Globalization.CultureInfo.GetCultureInfo(1033)) }¤";
            }
            double verdi = item.Vote == null ? 0 : (double)item.Vote;
            string stemme = (verdi > 0) ? verdi.ToString("#0", System.Globalization.CultureInfo.GetCultureInfo(1033)) : "kubbet";
            line += $",¤{ item.Name }¤: ¤{ stemme }¤";

            if (count == results.Count())
            {
               a(item.RoundNo);
            }
         }

         line = $"{{¤Rank¤: null,¤Merke¤: ¤¤, ¤ABV¤: null,¤Snitt¤: ¤Snitt:¤";

         foreach (var p in person_avg)
         {
            string stemme = p.Value.Value.ToString("#0.00", System.Globalization.CultureInfo.GetCultureInfo(1033));
            line += $",¤{ p.Value.Key }¤: ¤{ stemme }¤";
         }
         a(0);
         // Her setter vi sammen de ulike linjene til et array
         string total = "[";
         count = 0;
         foreach (var entry in lines)
         {
            count++;
            total += entry;
            if (count < lines.Count)
            {
               total += ",";
            }
         }
         return total + "]";
      }
   }
}