using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   public class VoteRound
   {
      // int - UserId, int - the specific user's vote/score
      Dictionary<int, int> _votes;

      public Brand CurrentBrand { get; set; }
      public bool Completed { get; set; }

      public VoteRound()
      {
         _votes = new Dictionary<int, int>();
      }

      public VoteRound(VoteRound other) : this()
      {
         if (other != null)
         {
            foreach (var item in other._votes)
            {
               _votes.Add(item.Key, item.Value);
            }
            CurrentBrand = other.CurrentBrand;
            Completed = other.Completed;
         }
      }

      public void AddVote(int user_id, int score)
      {
         if (_votes.ContainsKey(user_id))
         {
            if (score == 0)
            {
               // Special case - it's allowed to remove an existing vote by submitting a zero
               _votes.Remove(user_id);
            }
            else
            {
               _votes[user_id] = score;
            }
         }
         else if (score > 0)
         {
            _votes.Add(user_id, score);
         }
      }

      public override string ToString()
      {
         return $"CurrentBrand: { CurrentBrand.ToString() }, Number of votes: { _votes.Count }";
      }
   }
}