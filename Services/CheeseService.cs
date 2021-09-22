using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CheeseAPI.Services
{
    public static class CheeseService
    {
        public static TimeSpan CHEESE_POOL_UPDATE_RATE = TimeSpan.FromHours(12);
        public static readonly uint CHEESE_PAGES;
        public static readonly string FLICKR_TOKEN = Environment.GetEnvironmentVariable("CHEESE_FLICKR_TOKEN");
        public static readonly string API_URL;

        public static DateTime LastCheesePoolUpdate = DateTime.MinValue;
        public static bool CheesePoolUpdating = false;
        public static bool CheesePoolOverwriting = false;
        public static List<string> CheesePool = new();

        static CheeseService()
        {
            API_URL = $"https://www.flickr.com/services/rest/?method=flickr.photos.search&api_key={FLICKR_TOKEN}&tags=cheese&safe_search=2&extras=url_s&per_page=500&page={{0}}&format=json&nojsoncallback=1";
            try
            {
                CHEESE_PAGES = uint.Parse(Environment.GetEnvironmentVariable("CHEESE_PAGES"));
            }
            catch (Exception)
            {
                Console.WriteLine("Using default page count.");
				CHEESE_PAGES = 10;
            }

            UpdateCheesePool();
        }

        public static string Get(int page)
        {
            if (page < 0)
                return null;

            if (!CheesePoolUpdating && DateTime.Now - LastCheesePoolUpdate > CHEESE_POOL_UPDATE_RATE)
                UpdateCheesePool();

            var WaitingSince = DateTime.Now;
            while (CheesePoolOverwriting)
            {
                if (DateTime.Now - WaitingSince > TimeSpan.FromSeconds(1))
                    return null;
            }

            if (CheesePool.Count == 0)
                return null;

            try
            {
                return CheesePool[page % CheesePool.Count];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int GetPagesCount()
        {
            var WaitingSince = DateTime.Now;
            while (CheesePoolOverwriting)
            {
                if (DateTime.Now - WaitingSince > TimeSpan.FromSeconds(1))
                    return -1;
            }

            return CheesePool.Count;
        }

        public static void UpdateCheesePool()
        {
            if (CheesePoolUpdating)
                return;
            CheesePoolUpdating = true;

            if (DateTime.Now - LastCheesePoolUpdate < CHEESE_POOL_UPDATE_RATE)
            {
                Console.WriteLine("Oh well.");
                return;
            }

            LastCheesePoolUpdate = DateTime.Now;
            List<string> NewCheesePool = new();

            try
            {
                Random random = new();
                for (uint page = 1; page <= CHEESE_PAGES; page++)
                {
                    List<Uri> NewerCheesePool = new(), MixCheesePool = new();
                    var uri = string.Format(API_URL, page);
                    var req = WebRequest.Create(uri);
                    var res = req.GetResponse();
                    var dataStream = res.GetResponseStream();
                    var reader = new StreamReader(dataStream);
                    var text = reader.ReadToEnd();
                    var json = JObject.Parse(text);
                    foreach (var photo in json["photos"]["photo"])
                    {
                        var url_s = photo["url_s"];
                        if (url_s == null)
                            continue;

                        var curi = new Uri(url_s.ToString());
                        if (random.Next() % 2 == 0)
                            NewerCheesePool.Add(curi);
                        else
                            MixCheesePool.Add(curi);
                    }
                    if (MixCheesePool.Count > 0)
                        NewerCheesePool.AddRange(MixCheesePool);
                    NewCheesePool.Add(string.Join("\n", NewerCheesePool));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            CheesePoolOverwriting = true;
            CheesePool = NewCheesePool;
            CheesePoolOverwriting = false;

            CheesePoolUpdating = false;
        }
    }
}
