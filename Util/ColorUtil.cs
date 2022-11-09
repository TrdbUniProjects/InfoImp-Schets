using System;
using System.IO;
using Avalonia.Media;

namespace Schets.Util;

/// <summary>
/// Color expressed as HSV
/// </summary>
public struct Hsv {
    /// <summary>
    /// The HSV components
    /// </summary>
    public double H, S, V;

    public override string ToString() {
        return $"H: {this.H} S: {this.S} V: {this.V}";
    }
}

/// <summary>
/// Color expressed as RGB
/// </summary>
public struct Rgb {
    /// <summary>
    /// The RGB components
    /// </summary>
    public int R, G, B;

    public override string ToString() {
        return $"R: {this.R} G: {this.G} B: {this.B}";
    }
}

/// <summary>
/// Utilities for dealing with colors
/// </summary>
public static class ColorUtil {

    /// <summary>
    /// Convert an RGB color to the HSV spectrum
    /// </summary>
    /// <param name="rgb">The RGB to convert</param>
    /// <returns>The HSV output</returns>
    public static Hsv FromRgb(Rgb rgb) {
        // Derived from: https://www.rapidtables.com/convert/color/rgb-to-hsv.html
        
        double r = rgb.R / 255d;
        double g = rgb.G / 255d;
        double b = rgb.B / 255d;

        double cmax = Math.Max(r, Math.Max(g, b));
        double cmin = Math.Min(r, Math.Min(g, b));
        double deltaC = cmax - cmin;

        double h = -1d;

        if (cmax == cmin) {
            h = 0;
        } else if (cmax == r) {
            h = (60 * ((g - b) / deltaC) + 360) % 360;
        } else if (cmax == g) {
            h = (60 * ((b - r) / deltaC) + 120) % 360;
        } else if (cmax == b) {
            h = (60 * ((r - g) / deltaC) + 240) % 360;
        }

        double s;
        if (cmax == 0) {
            s = 0;
        } else {
            s = (deltaC / cmax) * 100;
        }

        double v = cmax * 100;

        return new Hsv() {
            H = h,
            S = s,
            V = v,
        };
    }

    /// <summary>
    /// Convert a HSV color to the RGB spectrum
    /// </summary>
    /// <param name="hsv">The HSV color to convert</param>
    /// <returns>The converted RGB</returns>
    /// <exception cref="InvalidDataException">If the HSV is invalid</exception>
    public static Rgb FromHsv(Hsv hsv) {
        // Derived from https://www.rapidtables.com/convert/color/hsv-to-rgb.html
        double c = hsv.V / 100 * (hsv.S / 100);
        double x = c * (1 - Math.Abs(hsv.H / 60 % 2 - 1));
        double m = hsv.V / 100 - c;

        double r, g, b;
        double h = hsv.H;
        switch (h) {
            case >= 0 and < 60:
                r = c;
                g = x;
                b = 0;
                break;
            case >= 60 and < 120:
                r = x;
                g = c;
                b = 0;
                break;
            case >= 120 and < 180:
                r = 0;
                g = c;
                b = x;
                break;
            case >= 180 and < 240:
                r = 0;
                g = x;
                b = c;
                break;
            case >= 240 and < 300:
                r = x;
                g = 0;
                b = c;
                break;
            case >= 300 and < 360:
                r = c;
                g = 0;
                b = x;
                break;
            default:
                throw new InvalidDataException("Invalid HSV");
        }

        return new Rgb {
            R = (int)Math.Round((r + m) * 255d),
            G = (int)Math.Round((g + m) * 255d),
            B = (int)Math.Round((b + m) * 255d),
        };
    } 
    
    /// <summary>
    /// Darken or lighten a color.
    /// </summary>
    /// <param name="r">The color</param>
    /// <param name="factor">
    /// The factor. factor &gt; 1 means the new color is lighter.
    /// If factor &lt; 1, that means the new color is darker
    /// </param>
    /// <returns>The new color</returns>
    public static Color AdjustHue(Color r, double factor = 0.7d) {
        Rgb originalColor = new() {
            R = r.R,
            G = r.G,
            B = r.B,
        };
        
        // Convert to HSV
        Hsv hsv = FromRgb(originalColor);

        // Darken the color        
        hsv.V *= factor;
        
        // Convert back to RGB
        Rgb newColor = FromHsv(hsv);

        return new Color(255, (byte)newColor.R, (byte)newColor.G, (byte)newColor.B);
    }
    
}