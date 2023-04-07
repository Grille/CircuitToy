using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitLib.Interface;
using CircuitLib.Rendering;

namespace CircuitToy.Rendering;

class GdiRenderer : Renderer<GdiRendererBackend>
{


    public GdiRenderer(GdiRendererBackend ctx, Theme theme, CircuitEditor editor) : base(ctx, theme, editor)
    {
        
    }
}
