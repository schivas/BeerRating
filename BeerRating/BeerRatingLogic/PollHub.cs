using BeerRating.BeerRatingLogic.DAL.Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace BeerRating.BeerRatingLogic
{
   [HubName("pollCtrl")]
   public class PollHub : Hub
   {

      #region Dashboard Methods - i.e the logic used by the dashboard keeping track of users and votes

      public async Task GetBlindTest(string id)
      {
         BlindTest bt = null;
         string error = "";
         // Regelen blir at hvis vi mottar noe annet enn et nummer så startes en ny blindtest
         if (!int.TryParse(id, out int blind_test_id))
         {
            // Start en ny blindtest
            ResultHolder<int> ret = await PollController.Instance.GetNewBlindTest(id);
            if (string.IsNullOrEmpty(ret.Error))
            {
               ResultHolder<BlindTest> r = await PollController.Instance.GetBlindTest(ret.Result);
               if (string.IsNullOrEmpty(r.Error))
               {
                  bt = r.Result;
               }
               else
               {
                  error = r.Error;
               }
            }
            else
            {
               error = ret.Error;
            }
         }
         else
         {
            // Prøv å finn en eksisterende blindtest
            ResultHolder <BlindTest> r = await PollController.Instance.GetBlindTest(blind_test_id);
            if (string.IsNullOrEmpty(r.Error))
            {
               bt = r.Result;
            }
            else
            {
               error = r.Error;
            }
         }
         await Clients.Caller.pollGetBlindTestResponse(new { Result = string.IsNullOrEmpty(error) ? "Success" : "Failure", Error = error, Instance = bt });
      }

      public async Task ApplyVotes(string blind_test_id, string brand_name, string abv, string override_mode)
      {
         string error = "";
         abv = (abv ?? "").Replace(",", ".");
         int rounds = 0;
         int b_id = 0;
         if (int.TryParse(blind_test_id, out int id) && float.TryParse(abv, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out float f_abv) && int.TryParse(override_mode, out int o_mode))
         {
            b_id = id;
            ResultHolder<int> ret = await PollController.Instance.ApplyVotes(id, brand_name, f_abv, o_mode);
            error = ret.Error;
            if (string.IsNullOrEmpty(ret.Error))
            {
               rounds = await PollController.Instance.GetNumberOfRounds(id);
               await Clients.Others.votesAreApplied(new { Result = "Success", Error = "", BlindTestId = b_id, Rounds = rounds });
            }
         }
         else
         {
            error = "Kunne ikke registrere denne runden - en eller flere av parametrene lot seg ikke konvertere!";
         }
         await Clients.Caller.pollApplyVotesResponse(new { Result = string.IsNullOrEmpty(error) ? "Success" : "Failure", Error = error, BlindTestId = b_id, Rounds = rounds });
      }

      public async Task RequestResult(string blind_test_id)
      {
         if (int.TryParse(blind_test_id, out int bt_id))
         {
            ResultHolder<string> ret = await PollController.Instance.GetResult(bt_id);
            await Clients.Caller.requestResultResponse(new { ret.Result, ret.Error });
         }
         else
         {
            await Clients.Caller.requestResultResponse(new { Result = "", Error = "Sendt inn blindtest-ID var ikke numerisk" });
         }
      }

      public async Task GetDummyData()
      {
         ResultHolder<string> ret = await PollController.Instance.GetScoreBoard(2);
         await Clients.Caller.requestDummData(new { ret.Result, ret.Error });
      }

      public async Task RequestScoreboard(string blind_test_id)
      {
         if (int.TryParse(blind_test_id, out int bt_id))
         {
            ResultHolder<string> ret = await PollController.Instance.GetScoreBoard(bt_id);
            await Clients.Caller.requestScoreBoardResponse(new { ret.Result, ret.Error });
         }
         else
         {
            await Clients.Caller.requestScoreBoardResponse(new { Result = "", Error = "Sendt inn blindtest-ID var ikke numerisk" });
         }
      }

      #endregion

      #region Client Methods - i.e the logic used by each user

      /// <summary>
      /// Used by participant to add itself to the blind test
      /// </summary>
      /// <param name="blind_test_id">A numeric value that identifies the blind test to be added to</param>
      /// <param name="connectionId_client">The Id of the client from the front end</param>
      /// <param name="participant_name">The user name</param>
      /// <returns></returns>
      public async Task AddUser(string blind_test_id, string connectionId_client, string participant_name)
      {
         if (int.TryParse(blind_test_id, out int test_id))
         {
            // First try to get the blind test
            ResultHolder<BlindTest> r = await PollController.Instance.GetBlindTest(test_id);
            if (!string.IsNullOrEmpty(r.Error))
            {
               await Clients.Caller.registerUserResult(RegisterUserResult.Msg(false, "Mottok en blindtest-ID (" + blind_test_id + ") som ikke eksisterer i databasen."));
               return;
            }

            // Then try to parse the user name as a number
            if (int.TryParse(participant_name, out int user_id))
            {
               #region Comment
               // Ok - the submitted participant_name is numerical
               // By definition, this is normally not allowed - a participant_name or nick, shall be a string (at least in our app)
               // The only time this is allowed to slip through, is when a refresh (F5) has occurred.
               // In this case the user must add itself once more. But the user may already exist in the database, hence we have a problem.
               // In such situations, the user can enter its given user ID (a number). So why not just allow a name to be re-enterred?
               // This is to raise the threshold a bit for other users to impersonate a different user
               #endregion
               Participant user = await PollController.Instance.GetUserById(test_id, user_id, Context.ConnectionId);
               if (user != null)
               {
                  int last_vote = await PollController.Instance.GetParticipantCurrentVote(test_id, user_id);
                  await Clients.Caller.registerUserResult(RegisterUserResult.Msg(true, null, r.Result.Id, r.Result.Name, user.Id, user.Name, last_vote, user.ImageIndex));
               }
               else
               {
                  await Clients.Caller.registerUserResult(RegisterUserResult.Msg(false, "Mottok en bruker-ID som ikke eksisterer i databasen"));
               }
            }
            else
            {
               // The user name was not a number - this is the normal case
               int last_vote = 0;
               ResultHolder<int> rr = await PollController.Instance.AddParticipant(test_id, participant_name, Context.ConnectionId, connectionId_client);
               string image_index = null;
               if (string.IsNullOrEmpty(rr.Error))
               {
                  last_vote = await PollController.Instance.GetParticipantCurrentVote(test_id, rr.Result);
                  Participant user = await PollController.Instance.GetUserById(test_id, rr.Result, Context.ConnectionId);
                  if (user != null)
                  {
                     image_index = user.ImageIndex;
                  }
               }
               await Clients.Caller.registerUserResult(RegisterUserResult.Msg(string.IsNullOrEmpty(rr.Error), rr.Error, r.Result.Id, r.Result.Name, rr.Result, participant_name, last_vote, image_index));
            }
         }
         else
         {
            await Clients.Caller.registerUserResult(RegisterUserResult.Msg(false, "Mottok en ikke numerisk blindtest-ID (" + blind_test_id + ")"));
         }
      }

      public async Task AddVote(string blind_test_id, string user_id, string vote)
      {
         if (int.TryParse(blind_test_id, out int bt_id) && int.TryParse(user_id, out int u_id) && int.TryParse(vote, out int vt))
         {
            Participant participant = await PollController.Instance.GetUserById(bt_id, u_id, Context.ConnectionId);
            if (participant != null)
            {
               RegVoteReply reply = await PollController.Instance.AddVote(bt_id, u_id, vt);
               if (reply == null)
               {
                  await Clients.Caller.voteAddVoteResult(new { BlindTestId = bt_id, ParticipantId = u_id, Result = "Klarte ikke å aksessere databasen!" });
               }
               else
               {
                  // Ok - vi har fått tilbake en status fra databasen
                  // Skal vi informere andre om stemmen?
                  if (reply.InformOthers)
                  {
                     // This call is implemented in the dashboard in order to update the the users summary in current round
                     // We will not send to vote since it's secret. The vote is stored in the back-end until the round is completed
                     await Clients.Others.voteAdded(new { reply.BlindTestId, reply.ParticipantId, ParticipantName = participant.Name, reply.ImageIndex, reply.UserVoteDeleted });
                  }
                  // Give the caller a feedback on the vote
                  await Clients.Caller.voteAddVoteResult(new { BlindTestId = bt_id, ParticipantId = u_id, reply.Result });
               }
            }
         }
      }

      #endregion

      #region Connection stuff

      public override Task OnConnected()
      {
         //System.Diagnostics.Trace.WriteLine("OnConnected - ConnectionId: " + Context.ConnectionId + ", Context.User: " + Context.User.ToString());
         return base.OnConnected();
      }

      public override Task OnReconnected()
      {
         //System.Diagnostics.Trace.WriteLine("OnReconnected - ConnectionId: " + Context.ConnectionId);
         return base.OnReconnected();
      }

      public override Task OnDisconnected(bool stopCalled)
      {
         //System.Diagnostics.Trace.WriteLine("OnDisconnected - ConnectionId: " + Context.ConnectionId);
         //Task<ResultHolder<string>> r = PollController.Instance.RemoveConnection(Context.ConnectionId);
         //if (!string.IsNullOrEmpty(r.Result.Result))
         //{
         //   System.Diagnostics.Trace.WriteLine("OnDisconnected - disconnected users: " + r.Result.Result);
         //   Clients.All.disconnectedUser(r.Result.Result);
         //}
         return base.OnDisconnected(stopCalled);
      }

      #endregion

   }

   public static class RegisterUserResult
   {
      public static object Msg(bool op_ok, string err_msg, int? blind_test_id, string blind_test_name, int? participant_id, string participant_name, int last_vote = -1, string imageIndex = null)
      {
         return new
         {
            Result = op_ok ? "Success" : "Failure",
            Error = op_ok ? "" : (err_msg ?? ""),
            BlindTestId = blind_test_id,
            BlindTestName = (blind_test_name ?? ""),
            ParticipantId = participant_id,
            ParticipantName = (participant_name ?? ""),
            LastVote = last_vote,
            ImageIndex = (imageIndex ?? "01")
         };
      }
      public static object Msg(bool op_ok, string err_msg)
      {
         return Msg(op_ok, err_msg, null, null, null, null);
      }
   }
}