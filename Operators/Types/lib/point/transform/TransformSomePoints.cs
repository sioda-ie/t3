using System;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Interfaces;
using T3.Core.Operator.Slots;

namespace T3.Operators.Types.Id_20338d1c_e4d3_4208_8f65_e1b720b8b563
{
    public class TransformSomePoints : Instance<TransformSomePoints>
,ITransformable
    {

        
        
        [Output(Guid = "f2321270-09ec-4335-87da-0f829c40c61e")]
        public readonly TransformCallbackSlot<T3.Core.DataTypes.BufferWithViews> Output = new TransformCallbackSlot<T3.Core.DataTypes.BufferWithViews>();

        public TransformSomePoints()
        {
            Output.TransformableOp = this;
        }        
        IInputSlot ITransformable.TranslationInput => Translation;
        IInputSlot ITransformable.RotationInput => Rotation;
        IInputSlot ITransformable.ScaleInput => Scale;
        public Action<Instance, EvaluationContext> TransformCallback { get; set; }

        [Input(Guid = "e6fcfecd-6ebe-479b-9454-fb40e9a2b9d0")]
        public readonly InputSlot<T3.Core.DataTypes.BufferWithViews> Points = new InputSlot<T3.Core.DataTypes.BufferWithViews>();

        [Input(Guid = "47997c7d-c1a7-444d-ba6b-d1dcbe935efb")]
        public readonly InputSlot<System.Numerics.Vector3> Translation = new InputSlot<System.Numerics.Vector3>();

        [Input(Guid = "a0de0994-8870-4ff4-add2-04e235ec9a91")]
        public readonly InputSlot<System.Numerics.Vector3> Rotation = new InputSlot<System.Numerics.Vector3>();

        [Input(Guid = "deebe64c-54d7-4d21-9a08-57aa1e39a094")]
        public readonly InputSlot<System.Numerics.Vector3> Scale = new InputSlot<System.Numerics.Vector3>();

        [Input(Guid = "d1e74275-2f54-48d2-a376-1f34b1bde52e")]
        public readonly InputSlot<float> UniformScale = new InputSlot<float>();

        [Input(Guid = "e40f0693-f8eb-415f-81d0-17d37fea0718")]
        public readonly InputSlot<bool> UpdateRotation = new InputSlot<bool>();

        [Input(Guid = "9c62ba5f-a191-4dc7-9886-4ec161f48571")]
        public readonly InputSlot<float> ScaleW = new InputSlot<float>();

        [Input(Guid = "0538efbc-bf43-4d93-b7f3-1e307757d874")]
        public readonly InputSlot<float> OffsetW = new InputSlot<float>();

        [Input(Guid = "d7a66d66-35d6-4f2d-8b99-5df2a5e7a65b")]
        public readonly InputSlot<int> Space = new InputSlot<int>();

        [Input(Guid = "79bbf891-3dae-43a4-a774-558d4909c269")]
        public readonly InputSlot<bool> WIsWeight = new InputSlot<bool>();

        [Input(Guid = "d92bd07c-255a-4dbd-90cf-860493eeb0cf")]
        public readonly InputSlot<float> Start = new InputSlot<float>();

        [Input(Guid = "7ddbe8c0-05ed-46fe-ae79-f6110f4fd65a")]
        public readonly InputSlot<float> LengthFactor = new InputSlot<float>();

        [Input(Guid = "605dd318-4462-43c3-940c-6709a75b7f95")]
        public readonly InputSlot<int> Take = new InputSlot<int>();

        [Input(Guid = "83ab882e-086e-4533-b20a-cf8ed8339f7b")]
        public readonly InputSlot<int> Skip = new InputSlot<int>();

        [Input(Guid = "ac9fb4d0-683a-4175-987a-bc68bd1046e0")]
        public readonly InputSlot<float> Scatter = new InputSlot<float>();

        [Input(Guid = "4313cb43-3b76-4a48-a052-79a3a26bfe79")]
        public readonly MultiInputSlot<bool> OnlyKeepTake = new MultiInputSlot<bool>();

        [Input(Guid = "85856e10-0998-4fc3-aa0d-d715c3da9b2a")]
        public readonly InputSlot<float> TestParam = new InputSlot<float>();
        
        
        
        private enum Spaces
        {
            PointSpace,
            ObjectSpace,
        }
        
        
        
        
        
    }
}
