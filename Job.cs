using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MVCcrypto.Data;
using MVCcrypto.Models.Models;
using Npgsql;

namespace MVCcrypto;

public class Job 
{
    class Config
    {
        public string? ApiToken { get; set; }
        public string? Host { get; set; }
        public int Refresh { get; set; }
        public string? Format { get; set; }
        public string? Currency { get; set; }
        public string[]? Tokens { get; set; }
    }

    public static readonly HttpClient Client = new HttpClient();

    public async Task Start()
    {
        Config config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"))!;
        var response = HttpCall(config);
        var body = await response;

        Console.WriteLine(body == "{\"gecko_says\":\"(V3) To the Moon!\"}" ? "[API]API Connected!" : "[API]API DOWN!");

        Dictionary<string, double> tokensPrice = new Dictionary<string, double>();
        foreach (var token in config.Tokens!)
        {
            tokensPrice.Add(token, 0.0);
        }

        var cs =
            "User ID=cece;Password=;Server=localhost;Port=5432;Database=asp_crypto; Integrated Security=true;Pooling=true;";
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        while (true)
        {
            await using var con = new NpgsqlConnection(cs);
            con.Open();
            for (int i = 0; i < config.Tokens!.Length; i++)
            {
                response = HttpCall(config,
                    "simple/price?ids=" + config.Tokens[i] + "&vs_currencies=" + config.Currency);
                body = await response;
                Dictionary<string, Dictionary<string, double>> json =
                    JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(body)!;
                if (tokensPrice[config.Tokens[i]] != json[config.Tokens[i]]["usd"])
                {
                    tokensPrice[config.Tokens[i]] = json[config.Tokens[i]]["usd"];
                    Console.WriteLine("[API]" + config.Tokens[i] + " price: " + tokensPrice[config.Tokens[i]]);
                    //System.IO.File.AppendAllLines(config.Tokens[i] + config.Format, new string[] { config.Tokens[i] + " is at " + tokensPrice[config.Tokens[i]].ToString() + config.Currency + " at " + DateTime.Now.ToString("HH:mm:ss") + " the " + DateTime.Now.ToString("dd/MM/yyyy") });
                    try
                    {
                        string sql = $"INSERT INTO \"Currencies\" (\"Name\", \"CurrentPrice\") VALUES ('{config.Tokens[i]}', {tokensPrice[config.Tokens[i]].ToString(nfi)});";
                        Console.WriteLine(sql);
                        await using var cmd = new NpgsqlCommand(sql, con);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            con.Close();
            await Task.Delay(config.Refresh * 1000);
        }
    }

    private static async Task<string> HttpCall(Config config, string path = "ping")
    {
        Console.WriteLine("[API]Calling " + config.Host + path);
        var stringTask = Client.GetStringAsync(config.Host + path);
        return await stringTask;
    }
}