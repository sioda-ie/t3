﻿using System.Numerics;
using ImGuiNET;
using T3.Core.Operator;
using T3.Gui.Interaction.Variations;
using T3.Gui.Interaction.Variations.Model;
using T3.Gui.Styling;

namespace T3.Gui.Windows.Variations
{
    public class PresetCanvas : VariationBaseCanvas
    {
        public override void DrawToolbarFunctions()
        {
            var s = ImGui.GetFrameHeight();
            if (VariationHandling.ActivePoolForPresets == null)
                return;
            
            if (CustomComponents.IconButton(Icon.Plus, "##addbutton", new Vector2(s, s)))
            {
                CreateVariation();
            }
        }

        protected override Instance InstanceForBlendOperations => VariationHandling.ActiveInstanceForPresets;
        protected override SymbolVariationPool PoolForBlendOperations => VariationHandling.ActivePoolForPresets;

        protected override void DrawAdditionalContextMenuContent()
        {
        }

        public override Variation CreateVariation()
        {
            var newVariation = VariationHandling.ActivePoolForPresets.CreatePresetForInstanceSymbol(VariationHandling.ActiveInstanceForPresets);
            if (newVariation != null)
            {
                newVariation.PosOnCanvas = VariationBaseCanvas.FindFreePositionForNewThumbnail(VariationHandling.ActivePoolForPresets.Variations);
                VariationThumbnail.VariationForRenaming = newVariation;
            }

            Selection.SetSelection(newVariation);
            ResetView();
            TriggerThumbnailUpdate();
            return newVariation;
        }
    }
}