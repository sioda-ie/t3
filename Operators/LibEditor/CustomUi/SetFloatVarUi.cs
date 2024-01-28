﻿using ImGuiNET;
using lib.exec.context;
using T3.Core.Operator;
using T3.Editor.Gui.ChildUi.WidgetUi;
using T3.Editor.Gui.UiHelpers;
using T3.Editor.UiModel;

namespace libEditor.CustomUi
{
    public static class SetFloatVarUi
    {
        public static SymbolChildUi.CustomUiResult DrawChildUi(Instance instance1, ImDrawListPtr drawList, ImRect area)
        {
            if (!(instance1 is SetFloatVar instance))
                return SymbolChildUi.CustomUiResult.PreventOpenSubGraph;

            var symbolChild = instance.Parent.Symbol.Children.Single(c => c.Id == instance.SymbolChildId);
            drawList.PushClipRect(area.Min, area.Max, true);
            
            var value = instance.Value.TypedInputValue.Value; 
            
            if (!string.IsNullOrEmpty(symbolChild.Name))
            {
                WidgetElements.DrawPrimaryTitle(drawList, area, symbolChild.Name);
            }
            else
            {
                WidgetElements.DrawPrimaryTitle(drawList, area, "Set " + instance.VariableName.TypedInputValue.Value);
            }

            WidgetElements.DrawSmallValue(drawList, area, $"{value:0.000}");
            
            drawList.PopClipRect();
            return SymbolChildUi.CustomUiResult.Rendered | SymbolChildUi.CustomUiResult.PreventInputLabels | SymbolChildUi.CustomUiResult.PreventOpenSubGraph;
        }

    }
}