public class Move
{
    public string Id { get; set; } 
    public string Name { get; set; } // ime koje igrac vidi na ekranu
    public string Kind { get; set; } // da li je obican napad, magija ili buff
    public string Scale { get; set; } // da li je fizicki ili magic  udarac
    public float Power { get; set; } // jacina samog poteza (amount)
    public string Target { get; set; } // koga udara (self,enemy)
    public string? Stat { get; set; } // koji atribut menja (dize napad ili spusta odbranu)
    public int? Delta { get; set; } // za koliko tacno menja taj atribut
    public int? Duration { get; set; } // koliko krugova traje taj efekat
    public int? HpCost { get; set; } // koliko hp-a za izvodjenje tog poteza
}