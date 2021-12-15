using BeerRating.BeerRatingLogic.DAL;
using BeerRating.BeerRatingLogic.DAL.Entities;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace BeerRating.BeerRatingLogic
{
   public class PollController : IDataProvider
   {
      // static holder for instance, need to use lambda to construct since the constructor is private
      private static readonly Lazy<PollController> _instance = new Lazy<PollController>(() => new PollController(new SqlServerProvider()));

      public static PollController Instance { get { return _instance.Value; } }
      private readonly IDataProvider _provider;

      private PollController(IDataProvider provider)
      {
         _provider = provider;
      }

      public async Task<ResultHolder<BlindTest>> GetBlindTest(int blind_test_id)
      {
         return await _provider.GetBlindTest(blind_test_id);
      }

      public async Task<ResultHolder<int>> GetNewBlindTest(string test_name)
      {
         return await _provider.GetNewBlindTest(test_name);
      }

      public async Task<ResultHolder<int>> AddParticipant(int blind_test_id, string username, string connectionId, string connectionId_client)
      {
         return await _provider.AddParticipant(blind_test_id, username, connectionId, connectionId_client);
      }

      public async Task<int> GetNumberOfRounds(int blind_test_id)
      {
         return await _provider.GetNumberOfRounds( blind_test_id);
      }

      public Task<ResultHolder<string>> RemoveConnection(string connectionId)
      {
         return _provider.RemoveConnection(connectionId);
      }

      public async Task<ResultHolder<int>> IsConnectionAlive(int blind_test_id, string connectionId)
      {
         return await _provider.IsConnectionAlive(blind_test_id, connectionId);
      }

      public async Task<Participant> GetUserById(int blind_test_id, int participant_id, string connectionId)
      {
         return await _provider.GetUserById(blind_test_id, participant_id, connectionId);
      }

      public async Task<ResultHolder<int>> ApplyVotes(int blind_test_id, string brand_name, float abv, int override_mode)
      {
         return await _provider.ApplyVotes(blind_test_id, brand_name, abv, override_mode);
      }

      public async Task<int> GetParticipantCurrentVote(int blind_test_id, int participant_id)
      {
         return await _provider.GetParticipantCurrentVote(blind_test_id, participant_id);
      }

      public async Task<ResultHolder<string>> GetResult(int blind_test_id)
      {
         return await _provider.GetResult(blind_test_id);
      }

      public async Task<ResultHolder<string>> GetAllBlindTests()
      {
         return await _provider.GetAllBlindTests();
      }

      public async Task<string> UpdateBlindTestName(int blind_test_id, string new_name)
      {
         return await _provider.UpdateBlindTestName(blind_test_id, new_name);
      }

      public async Task<RegVoteReply> AddVote(int blind_test_id, int participant_id, int vote)
      {
         return await _provider.AddVote(blind_test_id, participant_id, vote);
      }

      public async Task<ResultHolder<string>> GetScoreBoard(int blind_test_id)
      {
         return await _provider.GetScoreBoard(blind_test_id);
      }
   }
}