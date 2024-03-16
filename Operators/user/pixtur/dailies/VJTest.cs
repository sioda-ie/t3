using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace user.pixtur.dailies
{
	[Guid("604bd66e-f9ce-45f6-9fac-a8620418c73b")]
    public class VJTest : Instance<VJTest>
    {
        [Output(Guid = "07cf3c25-0cbb-414b-94b5-807c50c709d3")]
        public readonly Slot<Texture2D> ColorBuffer = new();


    }
}
