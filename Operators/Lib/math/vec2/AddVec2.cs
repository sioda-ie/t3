using System.Runtime.InteropServices;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace lib.math.vec2
{
	[Guid("3b42b2dc-09dd-4f7a-9cf8-5988f26fbda8")]
    public class AddVec2 : Instance<AddVec2>
    {
        [Output(Guid = "2CA0473D-0B36-4CA2-B532-9186E4556D67")]
        public readonly Slot<System.Numerics.Vector2> Result = new();


        public AddVec2()
        {
            Result.UpdateAction = Update;
        }

        private void Update(EvaluationContext context)
        {
            Result.Value = Input1.GetValue(context) + Input2.GetValue(context);
        }


        [Input(Guid = "4C37316D-618B-40B8-A8E3-73FA02F289B0")]
        public readonly InputSlot<System.Numerics.Vector2> Input1 = new();

        [Input(Guid = "8130C17F-CEC2-4E8E-ABBA-C89195010B87")]
        public readonly InputSlot<System.Numerics.Vector2> Input2 = new();

    }
}
