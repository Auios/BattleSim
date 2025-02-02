using Raylib_cs;
using System.Numerics;

namespace BattleSim;

public static class Game {
    public static List<Team> teams = [];
    public static List<Troop> troops = [];
    public static List<Troop> deadTroops = [];
    public static List<Projectile> projectiles = [];

    public static Team CreateTeam(string name, Color color, Rectangle spawnArea) {
        Team team = new(name, color, spawnArea);
        teams.Add(team);
        return team;
    }

    public static Troop SpawnTroop(Team team) {
        float x = team.spawnArea.X + Random.Shared.NextSingle() * team.spawnArea.Width;
        float y = team.spawnArea.Y + Random.Shared.NextSingle() * team.spawnArea.Height;

        Troop troop = new(new Vector2(x, y), team);
        troops.Add(troop);
        return troop;
    }

    public static Projectile SpawnProjectile(Vector2 position, Vector2 direction, Color color) {
        Projectile projectile = new(position, direction, color);
        projectiles.Add(projectile);
        return projectile;
    }

    public static void CleanupProjectiles() {
        projectiles.RemoveAll(p => p.ShouldRemove);
    }

    public static void CleanupTroops() {
        List<Troop> deadTroopsToMove = troops.Where(t => t.state == Troop.State.Dead).ToList();
        foreach (Troop troop in deadTroopsToMove) {
            troops.Remove(troop);
            // deadTroops.Add(troop);
        }
    }
}
