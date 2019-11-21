using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{

   /// <summary>
   /// Async methods cannot have ref, in or out parameters. Instead we can use this class which is a Tuple
   /// </summary>
   //public class IntResult
   //{
   //   public int Code { get; set; }
   //   public string Error { get; set; }

   //   public IntResult()
   //   {
   //      Code = 0;
   //      Error = "";
   //   }
   //}

   public class ResultHolder<T>
   {
      public T Result { get; set; }
      public string Error { get; set; }

      public ResultHolder()
      {
         Error = "";
      }
   }

   public class ResultHolder2<T, E>
   {
      public T Result { get; set; }
      public E Other { get; set; }
      public string Error { get; set; }

      public ResultHolder2()
      {
         Error = "";
      }
   }


}