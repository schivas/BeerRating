using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerRating.BeerRatingLogic.DAL.Entities;

namespace BeerRating.BeerRatingLogic.DAL
{
   interface IDataProvider
   {


      Task<ResultHolder<int>> AddParticipant(int blind_test_id, string username, string connectionId, string connectionId_client);
      Task<RegVoteReply> AddVote(int blind_test_id, int participant_id, int vote);
      Task<ResultHolder<int>> ApplyVotes(int blind_test_id, string brand_name, float abv, int override_mode);

      Task<ResultHolder<string>> GetAllBlindTests();
      Task<string> UpdateBlindTestName(int blind_test_id, string new_name);

      Task<ResultHolder<BlindTest>> GetBlindTest(int blind_test_id);
      Task<ResultHolder<int>> GetNewBlindTest(string test_name);
      Task<int> GetNumberOfRounds(int blind_test_id);
      Task<int> GetParticipantCurrentVote(int blind_test_id, int participant_id);
      Task<ResultHolder<string>> GetResult(int blind_test_id);
      Task<Participant> GetUserById(int blind_test_id, int participant_id, string connectionId);
      Task<ResultHolder<int>> IsConnectionAlive(int blind_test_id, string connectionId);
      Task<ResultHolder<string>> RemoveConnection(string connectionId);

      Task<ResultHolder<string>> GetScoreBoard(int blind_test_id);

   }
}
