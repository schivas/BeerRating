using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class RegVoteReply
   {
      public int BlindTestId { get; set; }
      public int ParticipantId { get; set; }
      public string ImageIndex { get; set; }
      public double Vote { get; set; }
      public bool InformClient { get; set; }
      public bool InformOthers { get; set; }
      public bool UserVoteDeleted { get; set; }
      public string Result { get; set; }
   }
}