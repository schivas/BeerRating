using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class Brand
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public double ABV { get; set; }
      public DateTime Created { get; set; }

      public Brand()
      {
         Name = "";
         ABV = 0;
         Created = DateTime.Now;
      }

      public Brand(Brand other) : this()
      {
         if (other != null)
         {
            Id = other.Id;
            Name = other.Name;
            ABV = other.ABV;
            Created = other.Created;
         }
      }

      public override string ToString()
      {
         return $"Id: { Id }, Name: { Name ?? "" }, ABV: { ABV }";
      }
   }
}