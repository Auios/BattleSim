using Raylib_cs;
using System.Numerics;

namespace BattleSim;

public class DebugPoint {
    public Vector2 position;
    public float rotation;
}

public static class PathMaker {
    public static List<DebugPoint> points = [];

    public static void Init() {
        DebugPoint point = new();
        point.position = new(0, 0);
        point.rotation = (float)Math.Atan2(200 - point.position.Y, 200 - point.position.X);
        points.Add(point);
    }

    public static void Render() {
        // bool lightDarkColor = true;
        // for(int i = 0; i < points.Count - 1; i++) {
        //     Vector2 point = points[i];
        //     if(i == 0) {
        //         Raylib.DrawLineEx(start, point, 1, lightDarkColor ? Color.White : Color.Gray);
        //     }
        //     else {
        //         Raylib.DrawLineEx(point, points[i + 1], 1, lightDarkColor ? Color.White : Color.Gray);
        //     }
        //     lightDarkColor = !lightDarkColor;
        // }
    }

    public static void MakeSegment() {

    }
}
