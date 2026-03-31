using Raylib_cs;

namespace BattleSim;
public static class Utils {
  public static Color CalculateSecondaryColor(Color primaryColor) {
    // Calculate luminance
    float luminance = (primaryColor.R * 0.299f + primaryColor.G * 0.587f + primaryColor.B * 0.114f) / 255f;

    // If color is dark, use lighter accent, else use darker accent
    if (luminance < 0.5f) {
      return new Color(
          (byte)Math.Clamp(primaryColor.R + 100, 0, 255),
          (byte)Math.Clamp(primaryColor.G + 100, 0, 255),
          (byte)Math.Clamp(primaryColor.B + 100, 0, 255),
          (byte)255
      );
    }
    return new Color(
        (byte)Math.Clamp(primaryColor.R - 100, 0, 255),
        (byte)Math.Clamp(primaryColor.G - 100, 0, 255),
        (byte)Math.Clamp(primaryColor.B - 100, 0, 255),
        (byte)255
    );
  }
}
