using BattleSim;
using Raylib_cs;
using System.Numerics;

public class Projectile {
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed = 200;
    public float DistanceTraveled = 0;
    public float MaxDistance = 200;
    public Color Color;
    public bool ShouldRemove = false;

    public Projectile(Vector2 position, Vector2 direction, Color color) {
        Position = position;
        Direction = direction;
        Color = color;
    }

    public void Update(double dt) {
        Position += Direction * Speed * (float)dt;
        DistanceTraveled += Speed * (float)dt;

        // Check for collisions with troops
        foreach (Team team in Game.teams) {
            foreach (Troop troop in team.GetTroops()) {
                if (troop.state != Troop.State.Dead && Vector2.Distance(Position, troop.position) < troop.size) {
                    // Kill the troop
                    troop.state = Troop.State.Dead;
                    ShouldRemove = true;
                    return;
                }
            }
        }

        // Mark for removal if max distance reached
        ShouldRemove = DistanceTraveled >= MaxDistance;
    }

    public void Render() {
        Raylib.DrawCircleV(Position, 1, Color);
    }
}