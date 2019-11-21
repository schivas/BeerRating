using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BeerRating.BeerRatingLogic
{
   [HubName("beerRating")]
   public class BeerRatingHub : Hub
   {
      public async Task Send(string name, string message)
      {
         await Clients.All.addNewMessageToPage(name, message);
         System.Diagnostics.Trace.WriteLine("Caller: " + name + ", ConnectionId: " + Context.ConnectionId + ", Message: " + message);
      }

      public async Task TestJson(string name, string message)
      {
         List<TestData> testList = new List<TestData>();

         testList.Add(new TestData { p1 = "AAA", p2 = "BBB", p3 = DateTime.Now });
         testList.Add(new TestData { p1 = "CCC", p2 = "DDD", p3 = DateTime.Now });

         await Clients.All.sendMessage(testList);
      }

      public async Task TestBeerJson(string name, string message)
      {
         List<BeerData> testList = new List<BeerData>();

         //testList.Add(new TestData { p1 = "AAA", p2 = "BBB", p3 = DateTime.Now });
         //testList.Add(new TestData { p1 = "CCC", p2 = "DDD", p3 = DateTime.Now });

         testList.Add(new BeerData { Name = "Borg Juleøl", Score = 5.38 });
         testList.Add(new BeerData { Name = "Aass Juleøl", Score = 5.00 });
         testList.Add(new BeerData { Name = "Ekangersmuget Julebajer", Score = 5.00 });
         testList.Add(new BeerData { Name = "Kinn Tomasmesse", Score = 5.00 });
         testList.Add(new BeerData { Name = "Voss bryggeri Jolebrygg", Score = 4.75 });
         testList.Add(new BeerData { Name = "Tuborg Juleøl", Score = 4.63 });
         testList.Add(new BeerData { Name = "Ringnes Juleøl", Score = 4.50 });
         testList.Add(new BeerData { Name = "7-Fjell 7 sorter", Score = 3.88 });
         testList.Add(new BeerData { Name = "Kinn Juleblot", Score = 3.88 });
         testList.Add(new BeerData { Name = "ELØ Glitrende Juleøl", Score = 3.75 });
         testList.Add(new BeerData { Name = "Mack God Jul", Score = 3.63 });
         testList.Add(new BeerData { Name = "Hansa Juleøl", Score = 3.50 });
         await Clients.All.sendMessage(testList);
      }

      public async Task TestBeerJsonAdvanced(string name, string message)
      {
         
         List<BeerData> testList = new List<BeerData>();
         testList.Add(new BeerData { Name = "Borg Juleøl", Score = 5.38 });
         testList.Add(new BeerData { Name = "Aass Juleøl", Score = 5.00 });
         testList.Add(new BeerData { Name = "Ekangersmuget Julebajer", Score = 5.00 });
         testList.Add(new BeerData { Name = "Kinn Tomasmesse", Score = 5.00 });
         testList.Add(new BeerData { Name = "Voss bryggeri Jolebrygg", Score = 4.75 });
         testList.Add(new BeerData { Name = "Tuborg Juleøl", Score = 4.63 });
         testList.Add(new BeerData { Name = "Ringnes Juleøl", Score = 4.50 });
         testList.Add(new BeerData { Name = "7-Fjell 7 sorter", Score = 3.88 });
         testList.Add(new BeerData { Name = "Kinn Juleblot", Score = 3.88 });
         testList.Add(new BeerData { Name = "ELØ Glitrende Juleøl", Score = 3.75 });
         testList.Add(new BeerData { Name = "Mack God Jul", Score = 3.63 });
         testList.Add(new BeerData { Name = "Hansa Juleøl", Score = 3.50 });

         BeerDataAdvanced obj = new BeerDataAdvanced {data = testList, yAxesMin = 3 };
         await Clients.All.sendMessage(obj);
      }
   }


   public class TestData
   {
      public string p1 { get; set; }
      public string p2 { get; set; }
      public DateTime p3 { get; set; }
   }

   public class BeerData
   {
      public string Name { get; set; }
      public double Score { get; set; }
   }

   public class BeerDataAdvanced
   {
      public List<BeerData> data { get; set; }
      public double yAxesMin { get; set; }
   }


}