using System;
using UnityEngine;
using System.Linq;

namespace OneSignalPush
{
    public class GUIContext : IDisposable
    {
        readonly Color m_BackgroundColor;
        readonly Color m_Color;
        readonly Color m_ContentColor;
        readonly int m_Depth;
        readonly Matrix4x4 m_Matrix;
        readonly GUISkin m_Skin;
        readonly string m_Tooltip;
        readonly bool m_Enabled;

        public GUIContext()
        {
            m_BackgroundColor = GUI.backgroundColor;
            m_Color = GUI.color;
            m_ContentColor = GUI.contentColor;
            m_Depth = GUI.depth;
            m_Matrix = GUI.matrix;
            m_Skin = GUI.skin;
            m_Tooltip = GUI.tooltip;
            m_Enabled = GUI.enabled;
        }

        public static IDisposable Push()
        {
            return new GUIContext();
        }

        public void Dispose()
        {
            GUI.backgroundColor = m_BackgroundColor;
            GUI.color = m_Color;
            GUI.contentColor = m_ContentColor;
            GUI.depth = m_Depth;
            GUI.matrix = m_Matrix;
            GUI.skin = m_Skin;
            GUI.tooltip = m_Tooltip;
            GUI.enabled = m_Enabled;
        }
    }

    public struct GUIGroup : IDisposable
    {
        public GUIGroup(Rect rect)
        {
            GUI.BeginClip(rect);
        }

        public static GUIGroup Push(Rect rect)
        {
            return new GUIGroup(rect);
        }

        public void Dispose()
        {
            GUI.EndGroup();
        }
    }

    public static class GUIStyleExtensions
    {
        public static GUIStyle SetAlignment(this GUIStyle style, TextAnchor alignment)
        {
            style.alignment = alignment;
            return style;
        }
    }

    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color Darkest(this Color color)
        {
            const float factor = 0.2f;
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }

        public static Color EvenDarker(this Color color)
        {
            const float factor = 0.5f;
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }

        public static Color Darker(this Color color)
        {
            return color.Darker(0.8f);
        }

        public static Color Darker(this Color color, float factor)
        {
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }

        public static Color Brighter(this Color color)
        {
            return color.Brighter(0.2f);
        }

        public static Color Brighter(this Color color, float factor)
        {
            return new Color(color.r + (1 - color.r) * factor, color.g + (1 - color.g) * factor, color.b + (1 - color.b) * factor, color.a);
        }

        public static Color Brightest(this Color color)
        {
            const float factor = 0.4f;
            return new Color(color.r + (1 - color.r) * factor, color.g + (1 - color.g) * factor, color.b + (1 - color.b) * factor, color.a);
        }

        public static Color Desaturate(this Color color, float desaturateAmount = 0.5f, float darkeningAmount = 0.5f)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            s *= desaturateAmount;
            v *= darkeningAmount;
            return Color.HSVToRGB(h, s, v);
        }
    }

    public static class RectExtensions
    {
        public static Rect Below(this Rect source, Rect belowSource)
        {
            return new Rect(source.x, belowSource.y + belowSource.height, source.width, source.height);
        }

        public static Vector2 PointLeftCenter(this Rect source)
        {
            return new Vector2(source.xMin, source.yMax - Mathf.RoundToInt(source.height / 2));
        }

        public static Vector2 PointLeftTop(this Rect source)
        {
            return new Vector2(source.xMin, source.yMin);
        }

        public static Vector2 PointLeftBottom(this Rect source)
        {
            return new Vector2(source.xMin, source.yMax);
        }

        public static Vector2 PointCenterBottom(this Rect source)
        {
            return new Vector2(source.center.x, source.yMax);
        }

        public static Vector2 PointCenterTop(this Rect source)
        {
            return new Vector2(source.center.x, source.yMin);
        }

        public static Rect FactorWithCenterPivot(this Rect source, float factor)
        {
            var center = source.center;
            var newWidth = source.width * factor;
            var newHeight = source.height * factor;

            return new Rect(center.x - newWidth / 2, center.y - newHeight / 2, newWidth, newHeight);
        }

        public static Vector2 PointRightCenter(this Rect source)
        {
            return new Vector2(source.xMax, source.yMax - Mathf.RoundToInt(source.height / 2));
        }

        public static Vector2 PointRightTop(this Rect source)
        {
            return new Vector2(source.xMax, source.yMin);
        }

        public static Vector2 PointRightBottom(this Rect source)
        {
            return new Vector2(source.xMax, source.yMax);
        }

        public static Rect RightOf(this Rect source, Rect target)
        {
            return new Rect(target.x + target.width, source.y, source.width, source.height);
        }

        public static Rect RightOfSelf(this Rect source)
        {
            return source.RightOf(source);
        }

        public static Rect WithSize(this Rect source, float width, float height)
        {
            return new Rect(source.x, source.y, width, height);
        }

        public static Rect WithWidth(this Rect source, float width)
        {
            return new Rect(source.x, source.y, width, source.height);
        }

        public static Rect Round(this Rect source)
        {
            return new Rect(Mathf.RoundToInt(source.x), Mathf.RoundToInt(source.y), Mathf.RoundToInt(source.width), Mathf.RoundToInt(source.height));
        }

        public static Rect WithHeight(this Rect source, float height)
        {
            return new Rect(source.x, source.y, source.width, height);
        }

        public static Rect BelowSelf(this Rect source)
        {
            return source.Below(source);
        }

        public static Rect Pad(this Rect source, float left, float top, float right, float bottom)
        {
            return new Rect(source.x + left, source.y + top, source.width - right, source.height - bottom);
        }

        public static Rect PadCorrectly(this Rect source, float left, float top, float right, float bottom)
        {
            return new Rect(source.x + left, source.y + top, source.width - right - left, source.height - bottom - top);
        }

        public static Rect PadVertically(this Rect source, float pad)
        {
            return source.PadCorrectly(0, pad, 0, pad);
        }

        public static Rect PadHorizontally(this Rect source, float pad)
        {
            return source.PadCorrectly(pad, 0, pad, 0);
        }

        public static Rect Inflate(this Rect source, float left, float top, float right, float bottom)
        {
            return new Rect(source.x - left, source.y - top, source.width + right, source.height + bottom);
        }

        public static Rect PadSides(this Rect source, float padding)
        {
            return new Rect(source.x + padding, source.y + padding, source.width - padding * 2, source.height - padding * 2);
        }

        public static Rect InflateSides(this Rect source, float padding)
        {
            return new Rect(source.x - padding, source.y - padding, source.width + padding * 2, source.height + padding * 2);
        }

        public static Rect AlignTopRight(this Rect source, Rect target)
        {
            return new Rect(target.xMax - source.width, target.y, source.width, source.height);
        }

        public static Rect AlignHorizontallyByCenter(this Rect source, Rect target)
        {
            var y = target.y + (target.height - source.height) / 2;

            return new Rect(source.x, y, source.width, source.height);
        }

        public static Rect RelativeTo(this Rect source, Rect scope)
        {
            return new Rect(source.x - scope.x, source.y - scope.y, source.width, source.height);
        }

        public static Rect SwitchSpace(this Rect source, Rect from, Rect to)
        {
            return source.RelativeTo(from).AbsoluteTo(to);
        }

        public static Rect AbsoluteTo(this Rect source, Rect scope)
        {
            return new Rect(source.x + scope.x, source.y + scope.y, source.width, source.height);
        }

        public static Rect IncludeAsTopCenterOf(this Rect source, Rect target)
        {
            return source.AlignVerticallyByCenter(target).AlignHorizontally(target).Translate(0, -(source.height / 2));
        }

        public static Rect AlignVerticallyByCenter(this Rect source, Rect target)
        {
            var x = target.x + (target.width - source.width) / 2;
            return new Rect(x, source.y, source.width, source.height);
        }

        public static Rect Translate(this Rect source, float x, float y)
        {
            return new Rect(source.x + x, source.y + y, source.width, source.height);
        }

        public static Rect WithOrigin(this Rect source, float x, float y)
        {
            return new Rect(x, y, source.width, source.height);
        }

        public static Rect WithX(this Rect source, float x)
        {
            return source.WithOrigin(x, source.y);
        }

        public static Rect WithMaxX(this Rect source, float x)
        {
            source.xMax = x;
            return source;
        }

        public static Rect WithY(this Rect source, float y)
        {
            return source.WithOrigin(source.x, y);
        }

        public static Rect Align(this Rect source, Rect target)
        {
            return new Rect(target.x, target.y, source.width, source.height);
        }

        public static Rect SquashRight(this Rect source, float squash)
        {
            return new Rect(source.x, source.y, source.width - squash, source.height);
        }

        public static Rect SquashLeft(this Rect source, float squash)
        {
            return new Rect(source.x + squash, source.y, source.width - squash, source.height);
        }

        public static Rect SquashTop(this Rect source, float squash)
        {
            return new Rect(source.x, source.y + squash, source.width, source.height - squash);
        }

        public static Rect SquashBottom(this Rect source, float squash)
        {
            return new Rect(source.x, source.y, source.width, source.height - squash);
        }

        public static Rect AlignAndScale(this Rect source, Rect target)
        {
            return new Rect(target.x, target.y, target.width, target.height);
        }

        public static Rect AlignHorizontally(this Rect source, Rect target)
        {
            return new Rect(source.x, target.y, source.width, source.height);
        }

        public static Rect AlignVertically(this Rect source, Rect target)
        {
            return new Rect(target.x, source.y, source.width, source.height);
        }

        public static Rect CenterInsideOf(this Rect source, Rect target)
        {
            var y = target.y + (target.height - source.height) / 2;
            var x = target.x + (target.width - source.width) / 2;
            return new Rect(x, y, source.width, source.height);
        }

        public static Rect CenterAt(this Rect source, float x, float y)
        {
            return new Rect(Mathf.FloorToInt(x - source.width / 2f), Mathf.FloorToInt(y - source.height / 2f), source.width, source.height);
        }

        public static Rect LeftHalf(this Rect source)
        {
            return new Rect(source.x, source.y, Mathf.RoundToInt(source.width / 2), source.height);
        }

        public static Rect RightHalf(this Rect source)
        {
            var offset = Mathf.RoundToInt(source.width / 2);
            return new Rect(source.x + offset, source.y, offset, source.height);
        }

        public static Rect TopHalf(this Rect source)
        {
            var offset = Mathf.RoundToInt(source.height / 2);
            return new Rect(source.x, source.y, source.width, offset);
        }

        public static Rect BottomHalf(this Rect source)
        {
            var offset = Mathf.RoundToInt(source.height / 2);
            return new Rect(source.x, source.y + offset, source.width, offset);
        }

        public static Rect ClipWidth(this Rect source, float xMax)
        {
            var newWidth = source.xMax <= xMax ? source.width : source.width - (source.xMax - xMax);
            return new Rect(source.x, source.y, newWidth, source.height);
        }

        public static Rect Clip(this Rect source, Rect target)
        {
            var result = source;

            result.xMin = Mathf.Max(result.xMin, target.xMin);
            result.yMin = Mathf.Max(result.yMin, target.yMin);
            result.xMax = Mathf.Min(result.xMax, target.xMax);
            result.yMax = Mathf.Min(result.yMax, target.yMax);

            if (result.yMax < target.yMin || result.yMin > target.yMax)
            {
                result.height = 0;
            }

            if (result.xMax < target.xMin || result.xMin > target.xMax)
            {
                result.width = 0;
            }

            return result;
        }

        public static Rect ClipHorizontalRight(this Rect source, Rect target)
        {
            return new Rect(source.x, source.y, Mathf.Min(0, target.xMax - source.x), source.height);
        }

        public static Rect HorizontalEndToStartOf(this Rect source, Rect target)
        {
            source.xMax = target.xMin;
            return source;
        }

        public static Rect HorizontalEndToEndOf(this Rect source, Rect target)
        {
            source.xMax = target.xMax;
            return source;
        }

        public static Rect ClipHorizontallyFromZeroTo(this Rect source, float width)
        {
            return new Rect(Mathf.Max(0, source.x), source.y, Mathf.Min(source.width, width), source.height);
        }

        public static Rect BorderRight(this Rect source, int borderWidth = 1)
        {
            return new Rect(source.xMax - borderWidth, source.yMin, borderWidth, source.height);
        }

        public static Rect OutlineRight(this Rect source, int borderWidth = 1)
        {
            return new Rect(source.xMax, source.yMin, borderWidth, source.height);
        }

        public static Rect BorderLeft(this Rect source, int borderWidth = 1)
        {
            return new Rect(source.xMin, source.yMin, borderWidth, source.height);
        }

        public static Rect BorderBottom(this Rect source, int borderWidth = 1)
        {
            return new Rect(source.x, source.yMax - borderWidth, source.width, borderWidth);
        }

        public static Rect BorderTop(this Rect source, int borderWidth = 1)
        {
            return new Rect(source.x, source.yMin + borderWidth, source.width, borderWidth);
        }

        public static Rect InnerAlignWithBottomRight(this Rect source, Rect target)
        {
            return new Rect(target.xMax - source.width, target.yMax - source.height, source.width, source.height);
        }

        public static Rect InnerAlignWithCenterRight(this Rect source, Rect target)
        {
            return source.InnerAlignWithBottomRight(target).AlignHorizontallyByCenter(target);
        }

        public static Rect InnerAlignWithCenterLeft(this Rect source, Rect target)
        {
            return source.InnerAlignWithBottomLeft(target).AlignHorizontallyByCenter(target);
        }

        public static Rect InnerAlignWithBottomLeft(this Rect source, Rect target)
        {
            return new Rect(target.x, target.yMax - source.height, source.width, source.height);
        }

        public static Rect InnerAlignWithUpperRight(this Rect source, Rect target)
        {
            return new Rect(target.xMax - source.width, target.y, source.width, source.height);
        }

        public static Rect InnerAlignWithUpperLeft(this Rect source, Rect target)
        {
            return new Rect(target.xMin, target.y, source.width, source.height);
        }

        public static Rect InnerAlignWithUpperCenter(this Rect source, Rect target)
        {
            var rect = source.AlignVerticallyByCenter(target);
            rect.y = target.yMin;
            return rect;
        }

        public static Rect InnerAlignWithBottomCenter(this Rect source, Rect target)
        {
            var rect = source.AlignVerticallyByCenter(target);
            rect.y = target.yMax - rect.height;
            return rect;
        }

        public static Rect LeftOf(this Rect source, Rect target)
        {
            return new Rect(target.x - source.width, source.y, source.width, source.height);
        }

        public static Rect ExtendUpTo(this Rect source, Rect target)
        {
            source.yMin = target.yMax;
            return source;
        }

        public static Rect ExtendDownToTop(this Rect source, Rect target)
        {
            source.yMax = target.yMin;
            return source;
        }

        public static Rect ExtendDownToBottom(this Rect source, Rect target)
        {
            source.yMax = target.yMax;
            return source;
        }

        public static Rect Above(this Rect source, Rect target)
        {
            return new Rect(source.x, target.y - source.height, source.width, source.height);
        }

        public static Rect AboveAll(this Rect source, Rect target, int i)
        {
            return new Rect(source.x, target.y - source.height * i, source.width, source.height);
        }

        public static Rect Cover(this Rect source, params Rect[] targets)
        {
            var x = targets.Min(t => t.x);
            var y = targets.Min(t => t.y);
            var width = targets.Max(t => t.xMax - x);
            var height = targets.Max(t => t.yMax - y);
            return new Rect(x, y, width, height);
        }

        public static bool Contains(this Rect source, Rect target)
        {
            return source.xMin < target.xMin && source.xMax > target.xMax && source.yMin < target.yMin && source.yMax > target.yMax;
        }

        public static Rect StretchedVerticallyAlong(this Rect source, Rect target)
        {
            return new Rect(source.x, source.y, source.width, target.yMax - source.y);
        }

        public static Rect AddHeight(this Rect source, float height)
        {
            return new Rect(source.x, source.y, source.width, source.height + height);
        }

        public static Rect AddWidth(this Rect source, float width)
        {
            return new Rect(source.x, source.y, source.width + width, source.height);
        }

        public static Rect AddPosition(this Rect source, Vector2 position)
        {
            return new Rect(source.x + position.x, source.y + position.y, source.width, source.height);
        }

        public static Rect Clamp(this Rect source, Rect target)
        {
            var x = source.x;
            var y = source.y;

            if (source.width > target.width)
            {
                x = target.center.x - source.width / 2f;
            }
            else
            {
                if (source.xMin < target.xMin)
                {
                    x = target.xMin;
                }
                else if (source.xMax > target.xMax)
                {
                    x = target.xMax - source.width;
                }
            }

            if (source.height > target.height)
            {
                y = target.center.y - source.height / 2f;
            }
            else
            {
                if (source.yMin < target.yMin)
                {
                    y = target.yMin;
                }
                else if (source.yMax > target.yMax)
                {
                    y = target.yMax - source.height;
                }
            }

            return new Rect(x, y, source.width, source.height);
        }
    }
}
