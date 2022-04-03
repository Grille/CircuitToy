using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CircuitLib;

public class CircuitNodeList : List<Node>
{
    private Circuit owner;
    public CircuitNodeList(Circuit owner)
    {
        this.owner = owner;
    }

    public new void Add(Node node)
    {
        node.Owner = owner;
        base.Add(node);
    }

    public T Create<T>() where T : Node, new()
    {
        return Create<T>(Vector2.Zero);
    }

    public T Create<T>(float x, float y) where T : Node, new()
    {
        return Create<T>(new Vector2(x, y));
    }

    public T Create<T>(float x, float y, string name) where T : Node, new()
    {
        return Create<T>(new Vector2(x, y), name);
    }

    public T Create<T>(Vector2 pos) where T : Node, new()
    {
        return Create<T>(pos, $"AutID_{Count}");
    }

    public T Create<T>(Vector2 pos, string name) where T : Node, new()
    {
        var node = new T();
        node.Position = pos;
        node.RoundPosition();
        node.Name = name;

        Add(node);
        return node;
    }
}
