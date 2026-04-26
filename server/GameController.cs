using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Text.Json;
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
        Move move = monster.Moveset[Random.Shared.Next(monster.Moveset.Count)];
        return Ok(move);
    }
}