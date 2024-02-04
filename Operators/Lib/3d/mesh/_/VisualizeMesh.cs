using System.Runtime.InteropServices;
using T3.Core.DataTypes;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace lib._3d.mesh._
{
	[Guid("f8b12b4f-c10b-4e8b-9a69-344dbe8a063e")]
    public class VisualizeMesh : Instance<VisualizeMesh>
    {
        [Output(Guid = "5aa00627-91e7-449a-90e5-9f6df0d3eb14")]
        public readonly Slot<Command> Output = new();


        [Input(Guid = "ae443ba9-13b9-4692-97c4-a22d7acafcd4")]
        public readonly InputSlot<MeshBuffers> Mesh = new();

        [Input(Guid = "163db356-b44a-490c-a9ee-21b18a6e02ed")]
        public readonly InputSlot<bool> ShowWireframe = new();

        [Input(Guid = "c7887b49-a16e-4bf1-ada2-19b250972fb5")]
        public readonly InputSlot<bool> ShowIndices = new();

        [Input(Guid = "abab5c80-fa17-4d1c-aa28-3829be755d99")]
        public readonly InputSlot<bool> ShowTBNSpace = new();

        [Input(Guid = "76147372-f850-4aa3-ad3a-af4c374e1f2a")]
        public readonly InputSlot<bool> ShowVerticeSelection = new();

    }
}

