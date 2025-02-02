using Raylib_cs;

namespace BattleSim;

public class Team(string name, Color color, Rectangle spawnArea) {
    public string name = name;
    public Color color = color;
    public Rectangle spawnArea = spawnArea;

    private double spawnTimer = 0;
    private int maxTroopCount = 10;


    public List<Troop> GetTroops() {
        return Game.troops.Where(t => t.team == this).ToList();
    }

    public void Update(double dt) {
        spawnTimer += dt;
        if (spawnTimer >= 1.0 && Game.troops.Count < maxTroopCount) {
            spawnTimer = 0;
            Troop troop = Game.SpawnTroop(this);
        }
    }

    public void Render() {
        Color spawnAreaColor = color;
        spawnAreaColor.A /= 4;
        Raylib.DrawRectangleRec(spawnArea, spawnAreaColor);
    }
}
