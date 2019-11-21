using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class Vote
   {
      public Brand Brand { get; set; }
      public double Score { get; set; }

      public Vote() { }

      public Vote(Vote other)
      {
         if (other != null)
         {
            Brand = new Brand(other.Brand);
            Score = other.Score;
         }
      }

      public override string ToString()
      {
         return $"Brand: { Brand }, Score: { Score }";
      }
   }
}