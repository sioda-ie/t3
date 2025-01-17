﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using T3.Core.Model;
using T3.Core.Operator;
using T3.Core.Resource;
using T3.Editor.Gui.Graph.Helpers;
using T3.Editor.Gui.Graph.Interaction;
using T3.Editor.Gui.Graph.Modification;
using T3.Editor.Gui.InputUi;
using T3.Editor.Gui.Styling;
using T3.Editor.Gui.UiHelpers;

namespace T3.Editor.Gui.Graph.Dialogs
{
    public class AddOutputDialog : ModalDialog
    {
        public AddOutputDialog()
        {
            Flags = ImGuiWindowFlags.NoResize;
        }
        
        public void Draw(Symbol symbol)
        {
            if (BeginDialog("Add output"))
            {
                FormInputs.SetIndent(100);
                //ImGui.SetKeyboardFocusHere();
                FormInputs.AddStringInput("Name", ref _parameterName);
                
                FormInputs.DrawInputLabel("Type");
                TypeSelector.Draw(ref _selectedType);
                
                FormInputs.AddCheckBox("Is time clip", ref _isTimeClip);
                
                var isValid = GraphUtils.IsNewSymbolNameValid(_parameterName) && _selectedType != null;
                FormInputs.ApplyIndent();
                if (CustomComponents.DisablableButton("Add", isValid))
                {
                    InputsAndOutputs.AddOutputToSymbol(_parameterName, _isTimeClip, _selectedType, symbol);
                }

                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }

                EndDialogContent();
            }

            EndDialog();
        }

        private bool _isTimeClip;
        private string _parameterName = string.Empty;
        private Type _selectedType;                                                                        
    }
}