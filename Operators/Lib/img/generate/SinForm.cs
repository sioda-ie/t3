using System.Runtime.InteropServices;
using T3.Core.DataTypes.Vector;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Slots;

namespace lib.img.generate
{
	[Guid("0626aba2-ed0c-40c4-9b50-2e729d0d8d86")]
    public class SinForm : Instance<SinForm>
    {
        [Output(Guid = "719c0dbb-788c-4156-b776-b893ff616416")]
        public readonly Slot<SharpDX.Direct3D11.Texture2D> TextureOutput = new();

        [Input(Guid = "7eca4ec2-40f1-4699-8c1f-9e196917df67")]
        public readonly InputSlot<Int2> Resolution = new();

        [Input(Guid = "5ac1e50d-21af-40e8-ba0b-45a3e8cdd75d")]
        public readonly InputSlot<SharpDX.Direct3D11.Texture2D> Image = new();

        [Input(Guid = "ea8656ce-a75d-4cfd-9deb-cd3a36294399")]
        public readonly InputSlot<System.Numerics.Vector4> Fill = new();

        [Input(Guid = "924c36ab-7d36-4a3c-8dd2-72648fda1711")]
        public readonly InputSlot<System.Numerics.Vector4> Background = new();

        [Input(Guid = "78259bce-6e58-424c-9d06-5eea1f37606a")]
        public readonly InputSlot<System.Numerics.Vector2> Size = new();

        [Input(Guid = "23101a83-14d3-489e-a613-39439fb9a20e")]
        public readonly InputSlot<System.Numerics.Vector2> Offset = new();

        [Input(Guid = "40121009-b4b4-4613-9655-af09dcfea402")]
        public readonly InputSlot<System.Numerics.Vector2> OffsetCopies = new();

        [Input(Guid = "199467aa-ac7a-4851-94b8-9e0f5f64d6c0")]
        public readonly InputSlot<float> Rotate = new();

        [Input(Guid = "c6fbd3b2-45da-40bc-958a-8ab13b8cdf2b")]
        public readonly InputSlot<float> LineWidth = new();

        [Input(Guid = "aa818499-5322-47f4-b08d-d21bbd0b72c8")]
        public readonly InputSlot<float> Fade = new();

        [Input(Guid = "33f88b64-3d8f-4b07-afdc-18d3b3eb63c8")]
        public readonly InputSlot<float> Copies = new();
    }
}

