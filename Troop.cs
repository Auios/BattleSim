using Raylib_cs;
using System.Formats.Asn1;
using System.Numerics;

namespace BattleSim;

public class Troop {
    public Vector2 position;
    public Vector2 destination;
    public float rotation = 0;
    public Color color;
    public Color secondaryColor;
    public Team team;
    private double fireMinCooldown = 1;
    private double fireMaxCooldown = 2;
    private double fireCooldown = 0;
    public double timeSinceDeath = 0;

    public float size = 5;
    public float speed = 20;
    public float rotationSpeed = 8;

    private Troop? targetEnemy = null;
    public float attackRange = 50;

    public enum State {
        Idle,
        Move,
        Seek,
        Attack,
        Dead
    }
    public State state = State.Idle;


    public Troop(Vector2 position, Team team) {
        this.position = position;
        this.team = team;
        this.color = team.color;
        secondaryColor = Utils.CalculateSecondaryColor(color);
        rotation = Random.Shared.NextSingle() * 2 * MathF.PI;
    }

    public void Update(double dt) {
        if(state == State.Dead) {
            timeSinceDeath += dt;
            return;
        }
        
        if(state == State.Idle) {
            List<Troop> allTroops = Game.teams.SelectMany(t => t.GetTroops()).ToList();
            List<Troop> targets = ScanForEnemies(allTroops);
            if(targets.Count != 0) {
                // Pick the closest enemy
                targetEnemy = targets.OrderBy(t => Vector2.Distance(position, t.position)).First();
                state = State.Attack;
            }
            else {
                // Set destination based on team
                Vector2 teamDestination = team.name == "Red Team" 
                    ? new Vector2(0, 0) 
                    : new Vector2(200, 200);
                
                // Get direction towards team destination
                Vector2 direction = Vector2.Normalize(teamDestination - position);
                
                // Add some randomness to the movement
                float randomAngle = (Random.Shared.NextSingle() - 0.5f) * (MathF.PI / 4f);
                float distance = 10f;
                
                // Calculate new destination
                destination = position + new Vector2(
                    MathF.Cos(randomAngle) * distance,
                    MathF.Sin(randomAngle) * distance
                ) + direction * 20f;

                state = State.Move;
            }
        }
        else if(state == State.Move) {

            // Calculate direction to destination
            Vector2 directionToDestination = destination - position;
            float distanceToDestination = directionToDestination.Length();

            // If close to destination, stop moving
            if(distanceToDestination < 2) {
                state = State.Idle;
            }
            else {
                // Calculate target rotation towards destination
                float targetRotation = (float)Math.Atan2(directionToDestination.Y, directionToDestination.X);

                // Gradually rotate towards target rotation using rotationSpeed
                float angleDifference = targetRotation - rotation;
                if(angleDifference > Math.PI) angleDifference -= (float)(2 * Math.PI);
                if(angleDifference < -Math.PI) angleDifference += (float)(2 * Math.PI);

                rotation += Math.Clamp(angleDifference, -rotationSpeed * (float)dt, rotationSpeed * (float)dt);

                // Determine movement speed based on rotation progress
                float rotationProgress = (float)(Math.Abs(angleDifference) / Math.PI); // Normalized rotation progress
                float currentSpeed = speed * (1 - 0.5f * rotationProgress); // Reduce speed while rotating
            }

            position += new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * speed * (float)dt;
        }
        else if(state == State.Seek) {
        }
        else if(state == State.Attack) {
            // Update fire cooldown
            if(fireCooldown > 0) {
                fireCooldown -= dt;
            }

            if(targetEnemy == null || targetEnemy.state == State.Dead || Vector2.Distance(position, targetEnemy.position) > attackRange ) {
                state = State.Idle;
                targetEnemy = null;
            }
            else {
                // Calculate target rotation towards enemy
                Vector2 directionToEnemy = Vector2.Normalize(targetEnemy.position - position);
                float targetRotation = (float)Math.Atan2(directionToEnemy.Y, directionToEnemy.X);

                // Check if we are facing the enemy (within a small angle threshold)
                float angleDifference = targetRotation - rotation;
                if(angleDifference > Math.PI) angleDifference -= (float)(2 * Math.PI);
                if(angleDifference < -Math.PI) angleDifference += (float)(2 * Math.PI);

                // If we're close to facing the enemy and cooldown is ready
                if(Math.Abs(angleDifference) < 0.1f && fireCooldown <= 0) {
                    Shoot();
                    // Set new random cooldown between 0.5 and 1.0 seconds
                    fireCooldown = Random.Shared.NextDouble() * fireMinCooldown + (fireMaxCooldown - fireMinCooldown);
                }
                else {
                    // Gradually rotate towards target rotation using rotationSpeed
                    rotation += Math.Clamp(angleDifference, -rotationSpeed * (float)dt, rotationSpeed * (float)dt);
                }
            }
        }
    }

    public void Render() {
        Color renderColor = state == State.Dead 
            ? new Color(
                (byte)(color.R * 0.3f + Color.Gray.R * 0.7f),
                (byte)(color.G * 0.3f + Color.Gray.G * 0.7f),
                (byte)(color.B * 0.3f + Color.Gray.B * 0.7f),
                color.A
            )
            : color;
        
        Color renderSecondaryColor = state == State.Dead 
            ? new Color(
                (byte)(secondaryColor.R * 0.3f + Color.Gray.R * 0.7f),
                (byte)(secondaryColor.G * 0.3f + Color.Gray.G * 0.7f),
                (byte)(secondaryColor.B * 0.3f + Color.Gray.B * 0.7f),
                secondaryColor.A
            )
            : secondaryColor;

        // Draw the troop with secondary color as outline
        Raylib.DrawCircleV(position, size, renderSecondaryColor);
        Raylib.DrawCircleV(position, size * 0.8f, renderColor);

        Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * size;
        Raylib.DrawLineEx(position, position + direction, 1, Color.White);

        const float range = MathF.PI / 2.5f;
        Vector2 directionLeft = new Vector2((float)Math.Cos(rotation - range), (float)Math.Sin(rotation - range)) * 30;
        Raylib.DrawLineEx(position, position + directionLeft, 1, Color.Blue);
        Vector2 directionRight = new Vector2((float)Math.Cos(rotation + range), (float)Math.Sin(rotation + range)) * 30;
        Raylib.DrawLineEx(position, position + directionRight, 1, Color.Red);

    }

    public List<Troop> ScanForEnemies(List<Troop> allTroops) {
        // Find all enemy troops within attack range, excluding dead troops
        List<Troop> nearbyEnemies = allTroops.Where(t => 
            t != this && 
            t.team != team && 
            t.state != State.Dead && 
            Vector2.Distance(position, t.position) <= attackRange
        ).ToList();
        return nearbyEnemies;
    }

    public void Shoot() {
        if (targetEnemy != null) {
            Vector2 directionToEnemy = Vector2.Normalize(targetEnemy.position - position);
            Vector2 spawnOffset = directionToEnemy * size; // Spawn projectile ahead of the troop
            Game.SpawnProjectile(position + spawnOffset, directionToEnemy, color);
        }
    }
}
