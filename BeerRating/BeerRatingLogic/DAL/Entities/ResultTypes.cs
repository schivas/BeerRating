namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class ResultHolder<T>
   {
      public T Result { get; set; }
      public string Error { get; set; }

      public ResultHolder()
      {
         Error = "";
      }
   }
}