using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace Operators.User.pixtur.research
{
	[Guid("24dc052c-562e-4b4e-a59b-ab9bf55ba01d")]
    public class TypeDesignExperiments : Instance<TypeDesignExperiments>
    {
        [Output(Guid = "93d5e7b7-2e2d-468d-80e6-322ba4134190")]
        public readonly Slot<Texture2D> Output = new();


    }
}

