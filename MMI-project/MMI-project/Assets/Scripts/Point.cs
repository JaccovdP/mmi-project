using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Point
{
    public int x, y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point(Vector2 vec)
    {
        this.x = (int)vec.x;
        this.y = (int)vec.y;
    }

    public float Distance(Point p)
    {
        return Mathf.Sqrt(Mathf.Pow(p.x - x, 2) + Mathf.Pow(p.y - y, 2));
    }

    public static Point operator+(Point a, Vector2 b)
    {
        return new Point((int)(a.x + b.x), (int)(a.y + b.y));
    }

    public static Vector2 operator -(Point a, Vector2 b)
    {
        return new Vector2(a.x - b.x, a.y - b.y);
    }
}

