using Raylib_cs;

namespace BattleSim;

public class Team(string name, Color color, Rectangle spawnArea) {
  public string name = name;
  public Color color = color;
  public Rectangle spawnArea = spawnArea;

  private DateTime lastSpawnTime = DateTime.Now;
  private double spawnTimerSeconds = 0.5;
  private int maxTroopCount = 100;


  public List<Troop> GetTroops() {
    return Game.troops.Where(t => t.team == this).ToList();
  }

  public void Update(double dt) {
    if (DateTime.Now - lastSpawnTime > TimeSpan.FromSeconds(spawnTimerSeconds) && Game.troops.Count < maxTroopCount) {
      lastSpawnTime = DateTime.Now;
      Troop troop = Game.SpawnTroop(this);
    }
  }

  public void Render() {
    Color spawnAreaColor = color;
    spawnAreaColor.A /= 4;
    Raylib.DrawRectangleRec(spawnArea, spawnAreaColor);
  }
}
