using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class Participant
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string ImageIndex { get; set; }
      public string ConnectionId { get; set; }
      public DateTime Created { get; set; }

      // int is round number
      private Dictionary<int, Vote> _scores;

      public Participant()
      {
         Name = ImageIndex = ConnectionId = "";
         Created = DateTime.Now;
         _scores = new Dictionary<int, Vote>();
      }

      public Participant(Participant other) : this()
      {
         if (other != null)
         {
            Id = other.Id;
            Name = other.Name;
            ImageIndex = other.ImageIndex;
            ConnectionId = other.ConnectionId;
            Created = other.Created;
            foreach (var item in other._scores)
            {
               _scores.Add(item.Key, new Vote(item.Value));
            }
         }
      }

      public override string ToString()
      {
         return $"Navn: { Name }, ImageIndex: { ImageIndex ?? "" }, ConnectionId: { ConnectionId }, Created: { Created.ToString() }, Score Count: { _scores.Count }";
      }
   }
}