using System;
using System.Collections.Generic;
using System.Linq;


namespace BeerRating.BeerRatingLogic.DAL.Entities
{
   //interface IBlindTest
   //{
   //   int Id { get; set; }
   //   string Name { get; set; }
   //   bool IsCompleted { get; set; }
   //   DateTime Created { get; set; }
   //   DateTime Updated { get; set; }
   //   int Rounds { get; set; }
   //   bool IsRoundActive { get; set; }
   //   List<Participant> ActiveVotes { get; set; }
   //}

   public class BlindTest
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public bool IsCompleted { get; set; }
      public bool AllowAdHocParticipants { get; set; }
      public DateTime Created { get; set; }
      public DateTime Updated { get; set; }
      public int Rounds { get; set; }
      public bool IsRoundActive { get; set; }

      public List<CurrentVote> CurrentVotes { get; set; }

      private Dictionary<int, Participant> _participants;

      // int - the round number, VoteRound - the votes for all the persons
      Dictionary<int, VoteRound> _completed_rounds;

      VoteRound _current_round;

      public BlindTest()
      {
         Name = "";
         Created = Updated = DateTime.Now;
         Rounds = 0;
         IsRoundActive = false;
         CurrentVotes = new List<CurrentVote>();
         _participants = new Dictionary<int, Participant>();
         _completed_rounds = new Dictionary<int, VoteRound>();
         _current_round = new VoteRound();
      }

      public BlindTest(BlindTest other): this()
      {
         if (other != null)
         {
            Id = other.Id;
            Name = other.Name;
            IsCompleted = other.IsCompleted;
            Created = other.Created;
            Updated = other.Updated;

            foreach (var item in other.CurrentVotes)
            {
               CurrentVotes.Add(new CurrentVote(item));
            }

            foreach (var item in other._participants)
            {
               _participants.Add(item.Key, new Participant(item.Value));
            }

            foreach (var item in other._completed_rounds)
            {
               _completed_rounds.Add(item.Key, new VoteRound(item.Value));
            }

            _current_round = new VoteRound(other._current_round);
         }
      }

      //public int Rounds { get { return _completed_rounds.Count; } }
      
      public bool AddParticipant(string username, string connectionId, string connectionId_client, out int participant_id, out string error)
      {
         participant_id = -1;
         error = "";
         username = username.Trim();
         if (AddParticipantByConnections(connectionId, connectionId_client, out participant_id, out string existing_name))
         {
            System.Diagnostics.Trace.WriteLine("HEY HEY HEY - this connection already identify as user");
            username = existing_name; // The stored name takes precedence
            return true;
         }

         if (string.IsNullOrEmpty(username))
         {
            error = "Brukernavn kan ikke være blankt.";
            return false;
         }

         foreach (var user in _participants)
         {
            if (user.Value.Name.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
               error = "Brukernavnet er alt tatt!";
               return false;
            }
         }
         participant_id = _participants.Count > 0 ? (_participants.Keys.Max() + 1) : 1;
         _participants.Add(participant_id, new Participant { Id = participant_id, Name = username, ConnectionId = connectionId });
         return true;
      }

      public bool AddVote(int participant_id, int vote)
      {
         if (_participants.ContainsKey(participant_id))
         {
            _current_round.AddVote(participant_id, vote);
         }
         return false;
      }

      public bool AddParticipantByConnections(string connectionId, string connectionId_client, out int participant_id, out string username)
      {
         participant_id = -1;
         username = "";
         if (connectionId.Equals(connectionId_client))
         {
            // This is rare - it indicates a user that is already connected -
            // this can indicate that a the user has navigated to a different page, then returned (e.g with back button in browser).
            // SignalR can in some situations keep the connections alive. This means that the user already is in the database.
            // We can then let the user in (this app is not very strikt when it comes to security - which is by design.
            // A plane refresh (F5) will force a disconnect, hence that situation will not get us here..
            if (IsConnectionAlive(connectionId, out participant_id, out username))
            {
               return true;
            }
         }
         return false;
      }

      public Participant GetParticipantById(int participant_id, string connectionId)
      {
         foreach (var item in _participants)
         {
            if (item.Value.Id == participant_id)
            {
               item.Value.ConnectionId = connectionId;
               return new Participant(item.Value);
            }
         }
         return null;
      }

      public bool IsConnectionAlive(string connectionId, out int participant_id, out string username)
      {
         participant_id = -1;
         username = "";
         foreach (var item in _participants)
         {
            if (item.Value.ConnectionId.Equals(connectionId))
            {
               participant_id = item.Value.Id;
               username = item.Value.Name;
               return true;
            }
         }
         return false;
      }

      public bool RemoveConnection(string connectionId, out string owner)
      {
         owner = "";
         foreach (var item in _participants)
         {
            if (item.Value.ConnectionId.Equals(connectionId))
            {
               item.Value.ConnectionId = "";
               owner = item.Value.Name;
               return true;
            }
         }
         return false;
      }

      public override string ToString()
      {
         return $"Id: { Id }, Name: { Name }, Completed: { IsCompleted }, Started: { Created }, Updated: { Updated }, Participants: { _participants.Count }, Completed Rounds: { _completed_rounds.Count }, Current Round: { _current_round.CurrentBrand }";
      }
   }
}