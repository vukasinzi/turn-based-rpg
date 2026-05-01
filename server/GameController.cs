using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using server.models;

namespace server;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly Config config;
    private readonly HeroDTO hero;

    public GameController()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "data/monsters.json");
        string json = System.IO.File.ReadAllText(path);
        config = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        path = Path.Combine(Directory.GetCurrentDirectory(), "data/player.json");
         json = System.IO.File.ReadAllText(path);
         hero = JsonSerializer.Deserialize<HeroDTO>(json, new JsonSerializerOptions
         {
             PropertyNameCaseInsensitive = true
         });


    }

    [HttpPost("save")]
    public IActionResult SaveHero([FromBody] HeroDTO hero)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "data/player.json");

        var json = JsonSerializer.Serialize(hero, new JsonSerializerOptions { WriteIndented = true });


        System.IO.File.WriteAllText(path, json);

        return Ok();
    }
    [HttpGet("config")]
    public IActionResult ReturnConfig()
    {
        return Ok(config);
    }

    [HttpGet("player")]
    public IActionResult ReturnPlayer()
    {
        return Ok(hero);
    }

    [HttpPost("potez")]
    public IActionResult NextMove([FromBody] Request request)
    {
        
        Monster monster = config.Monsters.FirstOrDefault(x => x.Id == request.MonsterId);
        if (monster == null)
            return NotFound();
        
        var validMoves = monster.Moveset
            .Where(m =>
                request.HeroBuffs.All(b => m.Kind != "debuff" || b.Name != m.Name) &&
                request.MonsterBuffs.All(b =>
                    (m.Kind != "buff" && m.Kind != "buff_cost_hp") || b.Name != m.Name
                )
            )
            .ToList();
        
        if (validMoves.Count == 0)
            validMoves = monster.Moveset; 
    
        Move move = validMoves[Random.Shared.Next(validMoves.Count)];
        return Ok(move);
    }
}