using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace BeerRating.BeerRatingLogic
{
   /*
   public class VoteController
   {
      // static holder for instance, need to use lambda to construct since constructor private
      private static readonly Lazy<VoteController> _instance = new Lazy<VoteController>(() => new VoteController());
      // accessor for instance
      public static VoteController Instance { get { return _instance.Value; } }

      //private readonly IHubContext _hubContext;
      private Dictionary<int, Run> _runs;

      // private to prevent direct instantiation.
      private VoteController()
      {
         // Save our hub context so we can easily use it 
         // to send to its connected clients
         //_hubContext = GlobalHost.ConnectionManager.GetHubContext<VoteBoardHub>();

         _runs = new Dictionary<int, Run>();

         Run run = new Run { RunId = 2};

      }

      public Run GetRun(int id)
      {
         if (_runs.ContainsKey(id))
         {
            return new Run(_runs[id]);
         }
         return null;
      }

      public Run GetNewRun(string run_name)
      {
         int id = int.MinValue;
         foreach (var item in _runs)
         {
            if (item.Key > id)
            {
               id = item.Key;
            }
         }
         if (id == int.MinValue)
         {
            id = 1;
         }
         else
         {
            id++;
         }
         Run run = new Run { RunId = id };
         run.RunName = run_name ?? run.RunName;
         _runs.Add(id, run);
         return new Run(run);
      }

      public bool AddUser(int run_id, string username, out string error)
      {
         error = "";
         if (!_runs.ContainsKey(run_id))
         {
            error = "Run ID = " + run_id.ToString() + " eksisterer ikke, kan ikke registrere bruker!";
            return false;
         }
         return _runs[run_id].TryAddUser(username, out error);
      }

   }

   public class Run
   {
      Dictionary<VoteUser, DateTime> _users;
      public int RunId { get; set; }
      public string RunName { get; set; }
      public bool Completed { get; set; }
      public DateTime Started { get; set; }
      public DateTime Updated { get; set; }

      public Run()
      {
         _users = new Dictionary<VoteUser, DateTime>();
         RunName = "Not Set";
         Completed = false;
         Started = DateTime.Now;
         Updated = DateTime.Now;
      }

      public Run(Run other) : this()
      {
         if (other != null)
         {
            RunId = other.RunId;
            RunName = other.RunName;
            Completed = other.Completed;
            Started = other.Started;
            Updated = other.Updated;
            foreach (var item in other._users)
            {
               _users.Add(new VoteUser(item.Key), item.Value);
            }
         }
      }

      public bool TryAddUser(string user, out string error)
      {
         error = "";
         if (string.IsNullOrEmpty(user))
         {
            error = "Supplied username was empty";
            return false;
         }
         VoteUser usr = new VoteUser { Username = user };
         if (!_users.ContainsKey(usr))
         {
            _users.Add(usr, DateTime.Now);
         }
         return true;
      }

   }

   public class VoteUser : IComparable
   {
      public string Username { get; set; }

      public VoteUser()
      {
         Username = "";
      }

      public VoteUser(VoteUser other) : this()
      {
         Username = other.Username;
      }

      public int CompareTo(object obj)
      {
         return Username.CompareTo(obj);
      }
   }
   */
}