using BeerRating.BeerRatingLogic.DAL.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Help = BeerRating.BeerRatingLogic.Utils;

namespace BeerRating.BeerRatingLogic.DAL
{
   public class SqlServerProvider : IDataProvider
   {
      public async Task<ResultHolder<int>> AddParticipant(int blind_test_id, string username, string connectionId, string connectionId_client)
      {
         ResultHolder<BlindTest> bt = await GetBlindTest(blind_test_id);
         ResultHolder<int> r = new ResultHolder<int>();
         if (string.IsNullOrEmpty(bt.Error))
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<ResultHolder<int>>("dbo.spAddParticipant"
                  , new { BlindTestId = blind_test_id, UserName = username, ConnectionId = connectionId, ConnectionIdClient = connectionId_client }
                  , null, null, CommandType.StoredProcedure);
               if (ret.Count() > 0)
               {
                  r = ret.FirstOrDefault();
               }
               else
               {
                  r.Error = "Feilet i å få resultatet fra SQL-proc som registrerer deltakere!";
               }
            }
         }
         else
         {
            r.Error = "Blindtest (Id = " + blind_test_id.ToString() + ") eksisterer ikke - kan ikke registrere bruker!";
         }
         return r;
      }

      public async Task<RegVoteReply> AddVote(int blind_test_id, int participant_id, int vote)
      {
         RegVoteReply reply = null;
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<RegVoteReply>("dbo.spAddVote"
                  , new { BlindTestId = blind_test_id, ParticipantId = participant_id, Vote = vote }
                  , null, null, CommandType.StoredProcedure);
               if (ret.Count() > 0)
               {
                  reply = ret.FirstOrDefault();
               }
            }
         }
         catch { }
         return reply;
      }

      public async Task<ResultHolder<int>> ApplyVotes(int blind_test_id, string brand_name, float abv, int override_mode)
      {
         ResultHolder<int> r = new ResultHolder<int>();
         using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
         {
            var ret = await connection.QueryAsync<ResultHolder<int>>("dbo.spApplyVotes"
                  , new { BlindTestId = blind_test_id, BrandName = brand_name, ABV = abv, OverrideMode = override_mode }
                  , null, null, CommandType.StoredProcedure);
            if (ret.Count() > 0)
            {
               r = ret.FirstOrDefault();
            }
            else
            {
               r.Error = "Feilet i å få resultatet fra SQL-proc som registrerer avstemmingsrunder!";
            }
         }
         return r;
      }

      public Task<ResultHolder<string>> GetAllBlindTests()
      {
         throw new NotImplementedException();
      }

      public async Task<ResultHolder<BlindTest>> GetBlindTest(int blind_test_id)
      {
         ResultHolder<BlindTest> r = new ResultHolder<BlindTest>();
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<BlindTest>(
                  @"SELECT a.*, b.Rounds, IsRoundActive = CONVERT(bit, IIF(c.Antall <> 0, 1, 0))
                  FROM dbo.BlindTest AS a
                  CROSS APPLY (SELECT Rounds = COUNT(distinct RoundNo) from [dbo].[Round] WHERE BlindTestId=@bt_Id) AS b
                  CROSS APPLY (SELECT Antall = COUNT(*) from [dbo].[CurrentRound] WHERE BlindTestId=@bt_Id) AS c
                  Where a.Id=@bt_Id", new { bt_Id = blind_test_id });
               if (ret.Count() > 0)
               {
                  r.Result = ret.FirstOrDefault();
               }
               else
               {
                  r.Error = "Fant ikke blindtest (Id = " + blind_test_id.ToString() + ")";
               }

               var cv = await connection.QueryAsync<CurrentVote>(
                  @"SELECT a.ParticipantId, ParticipantName = b.[Name], b.ImageIndex
                    FROM dbo.CurrentRound as a
                    INNER JOIN dbo.Participant as b on a.ParticipantId = b.Id
                    CROSS APPLY(SELECT img_id = a.ParticipantId % 10) AS c
                    WHERE a.BlindTestId=@bt_Id", new { bt_Id = blind_test_id });
               if (cv.Count() > 0)
               {
                  foreach (var item in cv)
                  {
                     r.Result.CurrentVotes.Add(item);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            r.Error = ex.Message;
         }
         return r;
      }

      public async Task<ResultHolder<int>> GetNewBlindTest(string test_name)
      {
         ResultHolder<int> r = new ResultHolder<int>();
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<int>("dbo.spGetNewBlindTest", new { BlindTestName = test_name }, null, null, CommandType.StoredProcedure);
               r.Result = ret.FirstOrDefault();
            }
         }
         catch (Exception ex)
         {
            r.Error = ex.Message;
         }
         return r;
      }

      public async Task<int> GetNumberOfRounds(int blind_test_id)
      {
         int ret = 0;
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
                ret = await connection.ExecuteScalarAsync<int>(
                  @"SELECT RoundNo = ISNULL(b.RoundNo, 0) FROM (SELECT Glue = 1) AS a
                    CROSS JOIN (SELECT RoundNo = MAX(RoundNo) FROM [dbo].[Round] WHERE BlindTestId=@Id) as b", new { Id = blind_test_id });
            }
         }
         catch { ret = 0 ; }
         return ret;
      }

      public async Task<int> GetParticipantCurrentVote(int blind_test_id, int participant_id)
      {
         int ret = 0;
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               ret = await connection.ExecuteScalarAsync<int>(
                 @"SELECT Vote = CONVERT(int, ROUND(Vote, 0)) FROM [dbo].[CurrentRound]
                  WHERE [BlindTestId]=@bt_id AND [ParticipantId]=@p_id", new { bt_id = blind_test_id, p_id = participant_id });
            }
         }
         catch { ret = 0; }
         return ret;
      }

      public async Task<ResultHolder<string>> GetResult(int blind_test_id)
      {
         ResultHolder<string> r = new ResultHolder<string>();
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<Result>("dbo.spResult"
               , new { BlindTestId = blind_test_id }
               , null, null, CommandType.StoredProcedure);
               if (ret.Count() > 0)
               {
                  r.Result = Result.Parse((List<Result>)ret);
               }
               else
               {
                  throw new Exception("Fant ikke noe resultat for denne blindtesten");
               }
            }
         }
         catch (Exception ex)
         {
            r.Error = ex.Message;
         }
         return r;
      }

      public async Task<ResultHolder<string>> GetScoreBoard(int blind_test_id)
      {
         ResultHolder<string> r = new ResultHolder<string>();
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<Score>("dbo.spScoreBoard"
               , new { BlindTestId = blind_test_id }
               , null, null, CommandType.StoredProcedure);
               if (ret.Count() > 0)
               {
                  r.Result = Score.Parse((List<Score>)ret);
               }
               else
               {
                  throw new Exception("Fant ikke noe resultat for denne blindtesten");
               }
            }
         }
         catch (Exception ex)
         {
            r.Error = ex.Message;
         }
         return r;
      }

      public async Task<Participant> GetUserById(int blind_test_id, int participant_id, string connectionId)
      {
         Participant p = null;
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<Participant>("select * from dbo.Participant where BlindTestId = @BlindTestId and Id = @Id", new { BlindTestId = blind_test_id, Id = participant_id });
               if (ret.Count() > 0)
               {
                  p = ret.FirstOrDefault();

                  if (p != null)
                  {
                     if (!(connectionId ?? "").Equals(p.ConnectionId ?? "", StringComparison.OrdinalIgnoreCase))
                     {
                        await connection.QueryAsync<int>("dbo.spUpdateConnection"
                           , new { BlindTestId = blind_test_id, ParticipantId = participant_id, ConnectionId = connectionId }
                           , null, null, CommandType.StoredProcedure);
                     }
                  }
               }
            }
         }
         catch { }
         return p;
      }

      public async Task<ResultHolder<int>> IsConnectionAlive(int blind_test_id, string connectionId)
      {
         ResultHolder<int> r = new ResultHolder<int>();
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<int>("dbo.spIsConnectionAlive", new { BlindTestId = blind_test_id, ConnectionId = connectionId }, null, null, CommandType.StoredProcedure);
               if (ret.Count() > 0)
               {
                  r.Result = ret.FirstOrDefault();
               }
               else
               {
                  r.Error = "Feilet i å få resultatet fra SQL-proc som registrerer deltakere!";
               }
            }
         }
         catch (Exception ex)
         {
            r.Error = ex.Message;
         }
         return r;
      }

      public async Task<ResultHolder<string>> RemoveConnection(string connectionId)
      {
         ResultHolder<string> r = new ResultHolder<string>();
         try
         {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<string>("dbo.spRemoveConnection", new { ConnectionId = connectionId }, null, null, CommandType.StoredProcedure);
               r.Result = ret.FirstOrDefault();
            }
         }
         catch (Exception ex)
         {
            r.Error = ex.Message;
         }
         return r;
      }

      public async Task<string> UpdateBlindTestName(int blind_test_id, string new_name)
      {
         string res = "";
         using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Help.Helper.CnnVal("syse_db")))
            {
               var ret = await connection.QueryAsync<string>("dbo.spUpdateBlindTestName"
                  , new { BlindTestId = blind_test_id, BlindTestName = new_name }
                  , null, null, CommandType.StoredProcedure);
            if (ret.Count() > 0)
            {
               res = ret.FirstOrDefault();
            }
         }
         return res;
      }
   }
}