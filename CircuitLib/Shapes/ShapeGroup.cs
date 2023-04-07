using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Math;
using CircuitLib.Rendering;

namespace CircuitLib.Shapes;

public sealed class ShapeCollection : Shape, ICollection<Shape>
{
    List<Shape> shapes;
    public int Count => shapes.Count;

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(Shape item)
    {
        shapes.Add(item);
    }

    public override void CalcBoundings()
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        shapes.Clear();
    }

    public override Shape Clone()
    {
        throw new NotImplementedException();
    }

    public bool Contains(Shape item) => shapes.Contains(item);


    public void CopyTo(Shape[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public override bool Draw(IRendererBackend ctx, int paint)
    {
        throw new NotImplementedException();
    }

    public override void Fill(IRendererBackend ctx, int paint)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Shape> GetEnumerator() => shapes.GetEnumerator();

    public override bool IsColliding(Vector2 position)
    {
        
        foreach (var shape in shapes)
        {
            if (shape.IsColliding(position)) return true;
        }
        return false;
    }

    public override bool IsColliding(BoundingBox bounds)
    {
        foreach (var shape in shapes)
        {
            if (shape.IsColliding(bounds)) return true;
        }
        return false;
    }

    public bool Remove(Shape item) => shapes.Remove(item);

    IEnumerator IEnumerable.GetEnumerator() => shapes.GetEnumerator();
}
