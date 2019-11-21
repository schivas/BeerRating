using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BeerRating.BeerRatingLogic
{
   [HubName("voteBoard")]
   public class VoteBoardHub : Hub
   {
      public async Task GetRun(string id)
      {
         Run run = null;
         // Regelen blir at hvis vi mottar noe annet enn et nummer, så startes et nytt run
         if (!int.TryParse(id, out int n))
         {
            run = VoteController.Instance.GetNewRun(id);
         }
         else
         {
            run = VoteController.Instance.GetRun(n);
         }
         //await Clients.All.vb_GetRunRequest(run);
         await Clients.Caller.vb_GetRunRequest(run);
      }

      public async Task AddUser(string run_id, string username)
      {
         if (int.TryParse(run_id, out int n))
         {
            string error;
            VoteController.Instance.AddUser(n, username, out error);
            if (string.IsNullOrEmpty(error))
            {
               await Clients.Caller.registerUserResult(new { Result = "Success", Error = "" });
            }
            else
            {
               await Clients.Caller.registerUserResult(new { Result = "Failure", Error = error });
            }
         }
         else
         {
            await Clients.Caller.registerUserResult(new { Result = "Failure", Error = "Mottok en ikke numerisk Run ID (" + run_id + ")" });
         }
      }

      public async Task AddVote(string name, string run_id, string vote)
      {
         if (int.TryParse(run_id, out int m) && int.TryParse(vote, out int n))
         {
            await Clients.Others.vb_AddedVote(name, run_id, vote);
         }
         else
         {
            System.Diagnostics.Trace.WriteLine("AddVote Failed - Caller: " + (name ?? "") + ", run_id: " + (run_id ?? "") + ", ConnectionId: " + Context.ConnectionId + ", vote: " + (vote ?? ""));
         }
         //await Clients.All.addNewMessageToPage(name, message);
         //await Clients.Caller.voteResult(res);
         
      }
   }
}