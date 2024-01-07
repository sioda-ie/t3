using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace user.pixtur.examples
{
	[Guid("214828be-a784-4da7-8f0c-303f5ea8c486")]
    public class SvgLineTransitionExample : Instance<SvgLineTransitionExample>
    {
        [Output(Guid = "7125321e-7b82-47f1-9c8c-170677adcd6e")]
        public readonly Slot<Texture2D> ImgOutput = new();


    }
}

