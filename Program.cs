using Raylib_cs;
using System.Numerics;

namespace BattleSim;

public static class Program {
  public static bool runApp = true;
  public static Camera2D camera;
  static Vector2? dragStartPosition = null;

  static void Main() {
    Console.WriteLine(GetNativeScreenSize());
    Raylib.InitWindow(1800, 1200, "Battle Sim");

    // Initialize the camera with default values
    camera = new Camera2D {
      Target = new Vector2(0, 0),  // Initial camera target
      Offset = new Vector2(1800 / 2f, 1200 / 2f),  // Center of the screen
      Zoom = 1.0f  // Default zoom level
    };

    Init();  // Make sure to call Init() to set up teams

    double lastTime = Raylib.GetTime();

    while (runApp) {
      double currentTime = Raylib.GetTime();
      double dt = currentTime - lastTime;
      lastTime = currentTime;

      Input(dt);
      Update(dt);
      Render();
      Thread.Sleep(1);
    }

    Raylib.CloseWindow();
  }

  public static void Init() {
    int size = 50;
    Game.CreateTeam("Blue Team", Color.Blue, new Rectangle(0, 0, size, size));
    Game.CreateTeam("Red Team", Color.Red, new Rectangle(200 - size, 200 - size, size, size));
  }

  public static void Shutdown() {
    runApp = false;
  }

  public static void Input(double dt) {
    if (Raylib.IsKeyPressed(KeyboardKey.Escape)) Shutdown();

    float mouseWheelMove = Raylib.GetMouseWheelMove();

    // Middle mouse button drag to pan
    if (Raylib.IsMouseButtonPressed(MouseButton.Middle)) {
      dragStartPosition = Raylib.GetMousePosition();
    }
    if (Raylib.IsMouseButtonReleased(MouseButton.Middle)) {
      dragStartPosition = null;
    }
    if (Raylib.IsMouseButtonDown(MouseButton.Middle) && dragStartPosition.HasValue) {
      Vector2 currentMousePos = Raylib.GetMousePosition();
      Vector2 delta = (currentMousePos - dragStartPosition.Value) / camera.Zoom;
      camera.Target -= delta;
      dragStartPosition = currentMousePos;
    }

    // Add WASD camera movement
    float cameraMoveSpeed = 10f / camera.Zoom;
    if (Raylib.IsKeyDown(KeyboardKey.W)) camera.Target.Y -= cameraMoveSpeed * (float)dt * 60;
    if (Raylib.IsKeyDown(KeyboardKey.S)) camera.Target.Y += cameraMoveSpeed * (float)dt * 60;
    if (Raylib.IsKeyDown(KeyboardKey.A)) camera.Target.X -= cameraMoveSpeed * (float)dt * 60;
    if (Raylib.IsKeyDown(KeyboardKey.D)) camera.Target.X += cameraMoveSpeed * (float)dt * 60;

    // Camera zoom control
    if (mouseWheelMove != 0) {
      // Get mouse position in world coordinates before zoom
      Vector2 mouseWorldPos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);

      // Apply zoom with a fixed increment
      float zoomFactor = 1.1f;
      float newZoom = mouseWheelMove > 0 ? camera.Zoom * zoomFactor : camera.Zoom / zoomFactor;

      // Clamp the zoom
      newZoom = Math.Clamp(newZoom, 0.2f, 5f);

      // Calculate the difference between mouse position before and after zoom
      camera.Zoom = newZoom;
      Vector2 newMouseWorldPos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
      Vector2 delta = mouseWorldPos - newMouseWorldPos;

      // Adjust camera target to keep the mouse position stable
      camera.Target += delta;

      // Keep the offset centered
      camera.Offset = new Vector2(
          Raylib.GetScreenWidth() / 2f,
          Raylib.GetScreenHeight() / 2f
      );
    }
  }


  public static void Update(double dt) {
    foreach (Team team in Game.teams) {
      team.Update(dt);
    }

    foreach (Projectile projectile in Game.projectiles) {
      projectile.Update(dt);
    }

    foreach (Troop troop in Game.troops) {
      troop.Update(dt);
    }

    Game.CleanupTroops();
    Game.CleanupProjectiles();
  }

  public static void Render() {
    Raylib.BeginDrawing();
    Raylib.BeginMode2D(camera);

    Raylib.ClearBackground(Color.Black);

    DrawGrid(20, 10);

    foreach (Team team in Game.teams) {
      team.Render();
    }

    foreach (Troop troop in Game.deadTroops) {
      troop.Render();
    }

    foreach (Troop troop in Game.troops) {
      troop.Render();
    }

    foreach (Projectile projectile in Game.projectiles) {
      projectile.Render();
    }

    Raylib.DrawCircleV(new(0, 0), 5, Color.Pink);
    Raylib.DrawCircleV(new(200, 200), 5, Color.Pink);

    Raylib.EndMode2D();

    // Render camera information on screen
    Raylib.DrawText($"Camera - X: {camera.Target.X:F2}", 10, 10, 20, Color.White);
    Raylib.DrawText($"Camera - Y: {camera.Target.Y:F2}", 10, 40, 20, Color.White);
    Raylib.DrawText($"Camera Zoom: {camera.Zoom:F2}", 10, 70, 20, Color.White);

    Raylib.EndDrawing();
  }

  public static Vector2 GetNativeScreenSize() {
    return new Vector2(Raylib.GetMonitorWidth(1), Raylib.GetMonitorHeight(1));
  }

  public static void DrawGrid(int count, float size) {
    for (int x = 0; x <= count; x++) {
      float xPos = x * size;
      Raylib.DrawLine((int)xPos, 0, (int)xPos, (int)(count * size), Color.DarkGray);
    }

    for (int y = 0; y <= count; y++) {
      float yPos = y * size;
      Raylib.DrawLine(0, (int)yPos, (int)(count * size), (int)yPos, Color.DarkGray);
    }
  }
}
