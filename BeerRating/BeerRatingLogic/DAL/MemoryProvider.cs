using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BeerRating.BeerRatingLogic.DAL.Entities;

namespace BeerRating.BeerRatingLogic.DAL
{
   public class MemoryProvider : IDataProvider
   {
      Dictionary<int, BlindTest> _tests;

      public MemoryProvider()
      {
         _tests = new Dictionary<int, BlindTest>();
      }


      //public async Task<bool> AddUser(int blind_test_id, string username, string connectionId, string connectionId_client, out int participant_id, out string error)
      //{
      //   participant_id = -1;
      //   error = "";
      //   username = username.Trim();
      //   if (!_tests.ContainsKey(blind_test_id))
      //   {
      //      error = "Blindtest (Id = " + blind_test_id.ToString() + ") eksisterer ikke - kan ikke registrere bruker!";
      //      return Task.FromResult(false);
      //   }
      //   return _tests[blind_test_id].AddPerson(username, connectionId, connectionId_client, out participant_id, out error);
      //}


      // 
      public async Task<ResultHolder<int>> AddParticipant(int blind_test_id, string username, string connectionId, string connectionId_client)
      {
         ResultHolder<int> r = new ResultHolder<int>();
         username = username.Trim();
         if (!_tests.ContainsKey(blind_test_id))
         {
            //error = "Blindtest (Id = " + blind_test_id.ToString() + ") eksisterer ikke - kan ikke registrere bruker!";
            //return Task.FromResult(false);
            r.Error = "Blindtest (Id = " + blind_test_id.ToString() + ") eksisterer ikke - kan ikke registrere bruker!";
         }
         else
         {
            int participant_id;
            string error;
            //r.Result = _tests[blind_test_id].AddParticipant(username, connectionId, connectionId_client, out participant_id, out error);
            //r.Other = participant_id;
            _tests[blind_test_id].AddParticipant(username, connectionId, connectionId_client, out participant_id, out error);
            r.Result = participant_id;
            r.Error = error;
         }
         //return _tests[blind_test_id].AddPerson(username, connectionId, connectionId_client, out participant_id, out error);
         return r;
      }

      public void AddVote(int blind_test_id, int participant_id, int vote)
      {
         if (_tests.ContainsKey(blind_test_id))
         {
            _tests[blind_test_id].AddVote(participant_id, vote);
         }
      }

      public Task<string> AddVote2(int blind_test_id, int participant_id, int vote)
      {
         throw new NotImplementedException();
      }

      public Task<RegVoteReply> AddVote3(int blind_test_id, int participant_id, int vote)
      {
         throw new NotImplementedException();
      }

      public Task<ResultHolder<int>> ApplyVotes(int blind_test_id, string brand_name, float abv, int override_mode)
      {
         throw new NotImplementedException();
      }

      public Task<ResultHolder<string>> GetAllBlindTests()
      {
         throw new NotImplementedException();
      }

      //public BlindTest GetBlindTest(int blind_test_id, out string error)
      //{
      //   error = "";
      //   if (_tests.ContainsKey(blind_test_id))
      //   {
      //      return new BlindTest(_tests[blind_test_id]);
      //   }
      //   error = "Fant ikke blindtest (Id = " + blind_test_id.ToString() + ")";
      //   return null;
      //}

      public async Task<ResultHolder<BlindTest>> GetBlindTest(int blind_test_id)
      {
         //error = "";
         //if (_tests.ContainsKey(blind_test_id))
         //{
         //   return new BlindTest(_tests[blind_test_id]);
         //}
         //error = "Fant ikke blindtest (Id = " + blind_test_id.ToString() + ")";
         ResultHolder<BlindTest> ret = new ResultHolder<BlindTest>();
         if (_tests.ContainsKey(blind_test_id))
         {
            ret.Result = new BlindTest(_tests[blind_test_id]);
            ret.Error = "";
         }
         else
         {
            ret.Error = "Fant ikke blindtest (Id = " + blind_test_id.ToString() + ")";
         }
         return ret;
      }

      //public Task<BlindTest> GetNewBlindTest(string test_name, out string error)
      //{
      //   error = "";
      //   int id = _tests.Count > 0 ? (_tests.Keys.Max() + 1) : 1;
      //   BlindTest test = new BlindTest { Id = id };
      //   test.Name = test_name ?? test.Name;
      //   _tests.Add(id, test);
      //   return Task.FromResult(test);
      //}

      public async Task<ResultHolder<int>> GetNewBlindTest(string test_name)
      {
         int id = _tests.Count > 0 ? (_tests.Keys.Max() + 1) : 1;
         BlindTest test = new BlindTest { Id = id };
         test.Name = test_name ?? test.Name;
         _tests.Add(id, test);
         //IntResult r = new IntResult { Code = id, Error = "" };
         ResultHolder<int> r = new ResultHolder<int> { Result = id, Error = "" };
         return r;
      }

      public async Task<int> GetNumberOfRounds(int blind_test_id)
      {
         if (_tests.ContainsKey(blind_test_id))
         {
            return _tests[blind_test_id].Rounds;
         }
         return 0;
      }

      public Task<int> GetParticipantCurrentVote(int blind_test_id, int participant_id)
      {
         throw new NotImplementedException();
      }

      public Task<ResultHolder<string>> GetResult(int blind_test_id)
      {
         throw new NotImplementedException();
      }

      public Task<ResultHolder<string>> GetScoreBoard(int blind_test_id)
      {
         throw new NotImplementedException();
      }

      //public Participant GetUserById(int blind_test_id, int participant_id, string connectionId)
      //{
      //   if (_tests.ContainsKey(blind_test_id))
      //   {
      //      return _tests[blind_test_id].GetParticipantById(participant_id, connectionId);
      //   }
      //   return null;
      //}
      public async Task<Participant> GetUserById(int blind_test_id, int participant_id, string connectionId)
      {
         if (_tests.ContainsKey(blind_test_id))
         {
            return _tests[blind_test_id].GetParticipantById(participant_id, connectionId);
         }
         return null;
      }

      //public bool IsConnectionAlive(int blind_test_id, string connectionId, out int participant_id, out string username)
      //{
      //   participant_id = -1;
      //   username = "";
      //   if (_tests.ContainsKey(blind_test_id))
      //   {
      //      return _tests[blind_test_id].IsConnectionAlive(connectionId, out participant_id, out username);
      //   }
      //   return false;
      //}

      public async Task<ResultHolder<int>> IsConnectionAlive(int blind_test_id, string connectionId)
      {
         ResultHolder<int> r = new ResultHolder<int>();
         int participant_id = -1;
         string username = "";
         if (_tests.ContainsKey(blind_test_id))
         {
            if (_tests[blind_test_id].IsConnectionAlive(connectionId, out participant_id, out username))
            {
               // Bruker ikke username lenger - er kun med fordi jeg ikke har endret signaturen til BlindTest::IsConnectionAlive
               r.Result = participant_id;
            }
         }
         return r;
      }

      public async Task<ResultHolder<string>> RemoveConnection(string connectionId)
      {
         ResultHolder<string> r = new ResultHolder<string> { Result = "" };
         string owners = "";
         List<string> list = new List<string>();
         foreach (var item in _tests)
         {
            if (item.Value.RemoveConnection(connectionId, out string usr))
            {
               list.Add(usr);
            }
         }
         if (list.Count > 0)
         {
            owners = string.Join(", ", list);
         }
         r.Result = owners;
         return r;
      }

      public Task<string> UpdateBlindTestName(int blind_test_id, string new_name)
      {
         throw new NotImplementedException();
      }
   }
}