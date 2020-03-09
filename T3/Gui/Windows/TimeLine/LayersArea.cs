﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using T3.Core.Animation;
using T3.Core.Logging;
using T3.Core.Operator;
using T3.Gui.Commands;
using T3.Gui.Graph.Interaction;
using T3.Gui.Interaction.Snapping;
using T3.Gui.Selection;
using UiHelpers;

namespace T3.Gui.Windows.TimeLine
{
    /// <summary>
    /// Shows a list of Layers with <see cref="TimeClip"/>s
    /// </summary>
    public class LayersArea : ITimeElementSelectionHolder, IValueSnapAttractor
    {
        public LayersArea(ValueSnapHandler snapHandler)
        {
            _snapHandler = snapHandler;
        }

        public void Draw(Instance compositionOp)
        {
            _drawList = ImGui.GetWindowDrawList();
            _compositionOp = compositionOp;

            ImGui.BeginGroup();
            {
                ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(0, 3)); // keep some padding 
                _minScreenPos = ImGui.GetCursorScreenPos();
                DrawAllLayers(NodeOperations.GetAllTimeClips(_compositionOp));
                DrawContextMenu();
            }
            ImGui.EndGroup();
        }

        bool _contextMenuIsOpen;

        private void DrawContextMenu()
        {
            if (!_contextMenuIsOpen && !ImGui.IsWindowHovered())
                return;

            // This is a horrible hack to distinguish right mouse click from right mouse drag
            var rightMouseDragDelta = (ImGui.GetIO().MouseClickedPos[1] - ImGui.GetIO().MousePos).Length();
            if (!_contextMenuIsOpen && rightMouseDragDelta > 3)
                return;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 8));
            if (ImGui.BeginPopupContextWindow("context_menu"))
            {
                _contextMenuIsOpen = true;
                if (ImGui.MenuItem("Delete", null, false, _selectedItems.Count > 0))
                {
                    UndoRedoStack.AddAndExecute(new TimeClipDeleteCommand(_compositionOp, _selectedItems));
                    _selectedItems.Clear();
                }

                ImGui.EndPopup();
            }
            else
            {
                _contextMenuIsOpen = false;
            }

            ImGui.PopStyleVar();
        }
        
        int _minLayerIndex = int.MaxValue;
        int _maxLayerIndex = int.MinValue;
        

        private void DrawAllLayers(List<ITimeClip> clips)
        {
            if (clips.Count == 0)
                return;

            _minLayerIndex = int.MaxValue;
            _maxLayerIndex = int.MinValue;

            foreach (var clip in clips)
            {
                _minLayerIndex = Math.Min(clip.LayerIndex, _minLayerIndex);
                _maxLayerIndex = Math.Max(clip.LayerIndex, _maxLayerIndex);
            }



            
            
            // Draw layer lines
            var min = ImGui.GetCursorScreenPos();
            var max = min + new Vector2(ImGui.GetContentRegionAvail().X, LayerHeight * (_maxLayerIndex - _minLayerIndex) - 1);
            var layerArea = new ImRect(min, max);

            for (var index = 0; index < clips.Count; index++)
            {
                var clip = clips[index];
                DrawClip(clip, layerArea, _minLayerIndex);
            }

            _drawList.AddRectFilled(new Vector2(min.X, max.Y),
                                    new Vector2(max.X, max.Y + 1), Color.Black);

            ImGui.SetCursorScreenPos(min + new Vector2(0, LayerHeight));
        }

        private void DrawClip(ITimeClip timeClip, ImRect layerArea, int minLayerIndex)
        {
            var xStartTime = TimeLineCanvas.Current.TransformPositionX(timeClip.TimeRange.Start);
            var xEndTime = TimeLineCanvas.Current.TransformPositionX(timeClip.TimeRange.End);
            var position = new Vector2(xStartTime,
                                       layerArea.Min.Y + (timeClip.LayerIndex - minLayerIndex) * LayerHeight);

            var clipWidth = xEndTime - xStartTime;
            var showSizeHandles = clipWidth > 4 * HandleWidth;
            var bodyWidth = showSizeHandles
                                ? (clipWidth - 2 * HandleWidth)
                                : clipWidth;

            var bodySize = new Vector2(bodyWidth, LayerHeight);
            var clipSize = new Vector2(clipWidth, LayerHeight);

            var symbolUi = SymbolUiRegistry.Entries[_compositionOp.Symbol.Id];
            var symbolChildUi = symbolUi.ChildUis.Single(child => child.Id == timeClip.Id);

            ImGui.PushID(symbolChildUi.Id.GetHashCode());

            var isSelected = _selectedItems.Contains(timeClip);
            //symbolChildUi.IsSelected = isSelected;

            var color = new Color(0.5f);
            _drawList.AddRectFilled(position, position + clipSize - new Vector2(1, 0), color);
            if (isSelected)
                _drawList.AddRect(position, position + clipSize - new Vector2(1, 0), Color.White);

            _drawList.AddText(position + new Vector2(4, 4), isSelected ? Color.White : Color.Black, symbolChildUi.SymbolChild.ReadableName);

            ImGui.SetCursorScreenPos(showSizeHandles ? (position + _handleOffset) : position);

            var wasClicked = ImGui.InvisibleButton("body", bodySize);

            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
            {
                var instance = _compositionOp.Children.Single(child => child.SymbolChildId == symbolChildUi.Id);
                SelectionManager.AddSelection(symbolChildUi, instance);
                _selectedItems.Clear();
                _selectedItems.Add(timeClip);
            }

            if (ImGui.IsItemHovered())
            {
                T3Ui.AddHoveredId(symbolChildUi.Id);
            }

            var notClickingOrDragging = !ImGui.IsItemActive() && !ImGui.IsMouseDragging(0);
            if (notClickingOrDragging && _moveClipsCommand != null)
            {
                TimeLineCanvas.Current.CompleteDragCommand();

                if (_moveClipsCommand != null)
                {
                    _moveClipsCommand.StoreCurrentValues();
                    UndoRedoStack.Add(_moveClipsCommand);
                    _moveClipsCommand = null;
                }
            }

            HandleDragging(timeClip, isSelected, wasClicked, HandleDragMode.Body, position);

            var handleSize = showSizeHandles ? new Vector2(HandleWidth, LayerHeight) : Vector2.One;

            ImGui.SetCursorScreenPos(position);
            var aHandleClicked = ImGui.InvisibleButton("startHandle", handleSize);
            HandleDragging(timeClip, isSelected, false, HandleDragMode.Start, position);

            ImGui.SetCursorScreenPos(position + new Vector2(bodyWidth + HandleWidth, 0));
            aHandleClicked |= ImGui.InvisibleButton("endHandle", handleSize);
            HandleDragging(timeClip, isSelected, false, HandleDragMode.End, position);

            if (aHandleClicked)
            {
                TimeLineCanvas.Current.CompleteDragCommand();

                if (_moveClipsCommand != null)
                {
                    _moveClipsCommand.StoreCurrentValues();
                    UndoRedoStack.Add(_moveClipsCommand);
                    _moveClipsCommand = null;
                }
            }

            ImGui.PopID();
        }

        private enum HandleDragMode
        {
            Body = 0,
            Start,
            End,
        }

        //private float _dragStartedAtTime;
        private float _timeWithinDraggedClip;
        private float _posYInsideDraggedClip;

        private void HandleDragging(ITimeClip timeClip, bool isSelected, bool wasClicked, HandleDragMode mode, Vector2 position)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(mode == HandleDragMode.Body
                                         ? ImGuiMouseCursor.Hand
                                         : ImGuiMouseCursor.ResizeEW);
            }

            if (!wasClicked && (!ImGui.IsItemActive() || !ImGui.IsMouseDragging(0, 1f)))
                return;

            if (ImGui.GetIO().KeyCtrl)
            {
                if (isSelected)
                    _selectedItems.Remove(timeClip);

                return;
            }

            if (!isSelected)
            {
                if (!ImGui.GetIO().KeyShift)
                {
                    TimeLineCanvas.Current.ClearSelection();
                }

                _selectedItems.Add(timeClip);
            }

            var mousePos = ImGui.GetIO().MousePos;
            if (_moveClipsCommand == null)
            {
                var dragStartedAtTime = TimeLineCanvas.Current.InverseTransformPositionX(mousePos.X);
                _timeWithinDraggedClip = dragStartedAtTime - timeClip.TimeRange.Start;
                _posYInsideDraggedClip =mousePos.Y -  position.Y;
                TimeLineCanvas.Current.StartDragCommand();
            }

            switch (mode)
            {
                case HandleDragMode.Body:
                    var currentDragTime = TimeLineCanvas.Current.InverseTransformPositionX(mousePos.X);

                    var newStartTime = currentDragTime - _timeWithinDraggedClip;
                    
                    var newDragPosY =  mousePos.Y - position.Y;
                    var dy =   _posYInsideDraggedClip-newDragPosY;

                    if (_snapHandler.CheckForSnapping(ref newStartTime))
                    {
                        TimeLineCanvas.Current.UpdateDragCommand(newStartTime - timeClip.TimeRange.Start, dy);
                        return;
                    }

                    var newEndTime = newStartTime + timeClip.TimeRange.Duration;
                    _snapHandler.CheckForSnapping(ref newEndTime);

                    TimeLineCanvas.Current.UpdateDragCommand(newEndTime - timeClip.TimeRange.End, dy);
                    break;

                case HandleDragMode.Start:
                    var newDragStartTime = TimeLineCanvas.Current.InverseTransformPositionX(mousePos.X);
                    _snapHandler.CheckForSnapping(ref newDragStartTime);
                    TimeLineCanvas.Current.UpdateDragStartCommand(newDragStartTime - timeClip.TimeRange.Start, 0);
                    break;

                case HandleDragMode.End:
                    var newDragTime = TimeLineCanvas.Current.InverseTransformPositionX(mousePos.X);
                    _snapHandler.CheckForSnapping(ref newDragTime);

                    TimeLineCanvas.Current.UpdateDragEndCommand(newDragTime - timeClip.TimeRange.End, 0);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
        
        
        #region implement selection holder interface --------------------------------------------
        void ITimeElementSelectionHolder.ClearSelection()
        {
            _selectedItems.Clear();
        }

        public void UpdateSelectionForArea(ImRect screenArea, SelectMode selectMode)
        {
            if (selectMode == SelectMode.Replace)
                _selectedItems.Clear();

            var startTime = TimeLineCanvas.Current.InverseTransformPositionX(screenArea.Min.X);
            var endTime = TimeLineCanvas.Current.InverseTransformPositionX(screenArea.Max.X);

            var layerMinIndex = (screenArea.Min.Y - _minScreenPos.Y) / LayerHeight + _minLayerIndex;
            var layerMaxIndex = (screenArea.Max.Y - _minScreenPos.Y) / LayerHeight + _minLayerIndex;
            
            var allClips = NodeOperations.GetAllTimeClips(_compositionOp);

            var matchingClips = allClips.FindAll(clip => clip.TimeRange.Start <= endTime
                                                         && clip.TimeRange.End >= startTime
                                                         && clip.LayerIndex <= layerMaxIndex
                                                         && clip.LayerIndex >= layerMinIndex-1);
            switch (selectMode)
            {
                case SelectMode.Add:
                case SelectMode.Replace:
                    _selectedItems.UnionWith(matchingClips);
                    break;
                case SelectMode.Remove:
                    _selectedItems.ExceptWith(matchingClips);
                    break;
            }
        }

        ICommand ITimeElementSelectionHolder.StartDragCommand()
        {
            _moveClipsCommand = new MoveTimeClipCommand(_compositionOp, _selectedItems.ToList());
            return _moveClipsCommand;
        }

        void ITimeElementSelectionHolder.UpdateDragCommand(double dt, double dy)
        {
            foreach (var clip in _selectedItems)
            {
                clip.TimeRange.Start += (float)dt;
                clip.TimeRange.End += (float)dt;

                
                if (clip.LayerIndex > _minLayerIndex && dy > LayerHeight)
                {
                    clip.LayerIndex--;
                }
                else if (clip.LayerIndex == _minLayerIndex &&  dy > LayerHeight + 20)
                {
                    clip.LayerIndex--;
                    _posYInsideDraggedClip -= 10;
                }
                else if (dy < -LayerHeight)
                {
                    clip.LayerIndex++;
                }
            }
        }

        void ITimeElementSelectionHolder.UpdateDragStartCommand(double dt, double dv)
        {
            foreach (var clip in _selectedItems)
            {
                // Keep 1 frame min duration
                clip.TimeRange.Start = (float)Math.Min(clip.TimeRange.Start + dt, clip.TimeRange.End - MinDuration);
            }
        }

        void ITimeElementSelectionHolder.UpdateDragEndCommand(double dt, double dv)
        {
            foreach (var clip in _selectedItems)
            {
                // Keep 1 frame min duration
                clip.TimeRange.End = (float)Math.Max(clip.TimeRange.End + dt, clip.TimeRange.Start + MinDuration);
            }
        }

        void ITimeElementSelectionHolder.UpdateDragStretchCommand(double scaleU, double scaleV, double originU, double originV)
        {
            foreach (var clip in _selectedItems)
            {
                clip.TimeRange.Start = (float)(originU + (clip.TimeRange.Start - originU) * scaleU);
                clip.TimeRange.End = (float)Math.Max(originU + (clip.TimeRange.End - originU) * scaleU, clip.TimeRange.Start + MinDuration);
            }
        }

        private const float MinDuration = 1 / 60f; // In bars

        public TimeRange GetSelectionTimeRange()
        {
            var timeRange = TimeRange.Undefined;
            foreach (var s in _selectedItems)
            {
                timeRange.Unite(s.TimeRange.Start);
                timeRange.Unite(s.TimeRange.End);
            }

            return timeRange;
        }

        void ITimeElementSelectionHolder.CompleteDragCommand()
        {
            if (_moveClipsCommand == null)
                return;

            _moveClipsCommand.StoreCurrentValues();
            UndoRedoStack.Add(_moveClipsCommand);
            _moveClipsCommand = null;
        }

        void ITimeElementSelectionHolder.DeleteSelectedElements()
        {
            //TODO: Implement
        }
        #endregion

        #region implement snapping interface -----------------------------------
        /// <summary>
        /// Snap to all non-selected Clips
        /// </summary>
        SnapResult IValueSnapAttractor.CheckForSnap(double targetTime)
        {
            SnapResult bestSnapResult = null;
            var snapThresholdOnCanvas = TimeLineCanvas.Current.InverseTransformDirection(new Vector2(6, 0)).X;

            var allClips = NodeOperations.GetAllTimeClips(_compositionOp);

            foreach (var clip in allClips)
            {
                if (_selectedItems.Contains(clip))
                    continue;

                KeyframeOperations.CheckForBetterSnapping(targetTime, clip.TimeRange.Start, snapThresholdOnCanvas, ref bestSnapResult);
                KeyframeOperations.CheckForBetterSnapping(targetTime, clip.TimeRange.End, snapThresholdOnCanvas, ref bestSnapResult);
            }

            return bestSnapResult;
        }
        #endregion

        private Vector2 _minScreenPos;

        private readonly HashSet<ITimeClip> _selectedItems = new HashSet<ITimeClip>();
        private static MoveTimeClipCommand _moveClipsCommand;
        private const int LayerHeight = 25;
        private const float HandleWidth = 5;
        private readonly Vector2 _handleOffset = new Vector2(HandleWidth, 0);

        private ImDrawListPtr _drawList;
        private Instance _compositionOp;
        private readonly ValueSnapHandler _snapHandler;
    }
}