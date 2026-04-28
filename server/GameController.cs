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

    public GameController()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "data/monsters.json");
        string json = System.IO.File.ReadAllText(path);
        config = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    [HttpGet("config")]
    public IActionResult ReturnConfig()
    {
        return Ok(config);
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