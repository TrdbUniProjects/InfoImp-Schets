using System;
using System.IO;

namespace Schets.Util;

public struct Hsv {
    public double H, S, V;

    public override string ToString() {
        return $"H: {this.H} S: {this.S} V: {this.V}";
    }
}

public struct Rgb {
    public int R, G, B;

    public override string ToString() {
        return $"R: {this.R} G: {this.G} B: {this.B}";
    }
}

public static class ColorUtil {

    // Derived from: https://www.rapidtables.com/convert/color/rgb-to-hsv.html
    public static Hsv FromRgb(Rgb rgb) {
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

    // Derived from https://www.rapidtables.com/convert/color/hsv-to-rgb.html
    public static Rgb FromHsv(Hsv hsv) {
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
    
}