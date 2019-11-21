using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class CurrentVote
   {
      public int ParticipantId { get; set; }
      public string ParticipantName { get; set; }
      public string ImageIndex { get; set; }

      public CurrentVote()
      {
         ParticipantId = 0;
         ParticipantName = ImageIndex = "";
      }

      public CurrentVote(CurrentVote other)
      {
         ParticipantId = other.ParticipantId;
         ParticipantName = other.ParticipantName;
         ImageIndex = other.ImageIndex;
      }

      public override string ToString()
      {
         return $"ParticipantId: {ParticipantId }, ParticipantName: { ParticipantName ?? ""}, ImageIndex: {ImageIndex ?? ""}";
      }

   }
}