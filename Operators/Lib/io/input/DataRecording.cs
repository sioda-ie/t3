using System.Runtime.InteropServices;
using T3.Core.DataTypes.DataSet;
using T3.Core.Logging;
using T3.Core.Operator;
using T3.Core.Operator.Attributes;
using T3.Core.Operator.Interfaces;
using T3.Core.Operator.Slots;

namespace lib.io.input
{
	[Guid("4023bcbf-74a6-4e50-a12e-4c22be5dbbdf")]
    public class DataRecording : Instance<DataRecording>,  IStatusProvider, ICustomDropdownHolder
    {
        [Output(Guid = "8c911387-bb2f-4d72-8528-b9f2d8cfe250")]
        public readonly Slot<DataSet> DataSet = new();
        
        public DataRecording()
        {
            DataSet.UpdateAction = Update;
        }

        private void Update(EvaluationContext context)
        {
            var dataSetsById = DataRecordingBucket.DataSetsById;
            if (dataSetsById.Count == 0)
            {
                _lastErrorMessage = "No active datasets to chose from";
                return;
            }

            var id = ActiveDataSetId.GetValue(context);
            if (!dataSetsById.TryGetValue(id, out var activeDataSet))
            {
                _lastErrorMessage = $"Can't find dataset {id}";
                return;
            }
            
            DataSet.Value = activeDataSet;
            _lastErrorMessage = null;
        }

        private string _lastErrorMessage;
        
        public IStatusProvider.StatusLevel GetStatusLevel()
        {
            return string.IsNullOrEmpty(_lastErrorMessage) ? IStatusProvider.StatusLevel.Success : IStatusProvider.StatusLevel.Warning;
        }

        public string GetStatusMessage()
        {
            return _lastErrorMessage;
        }

        
        public string GetValueForInput(Guid inputId)
        {
            if (inputId != ActiveDataSetId.Id)
            {
                Log.Warning("Request invalid input id", this);
                return "???";
            }

            return ActiveDataSetId.Value;
        }

        public IEnumerable<string> GetOptionsForInput(Guid inputId)
        {
            if (inputId != ActiveDataSetId.Id)
            {
                yield return "undefined";
                yield break;
            }
       
            var dataSetsById = DataRecordingBucket.DataSetsById;
            foreach (var s in dataSetsById.Keys)
            {
                yield return s;
            }
        }

        public void HandleResultForInput(Guid inputId, string result)
        {
            ActiveDataSetId.SetTypedInputValue(result);
        }
        
        [Input(Guid = "48e9dd6c-87d2-4701-96ab-3971f3150ff1")]
        public readonly InputSlot<bool> ResetTrigger = new();

        [Input(Guid = "88D15473-BD02-48CC-A1AF-B822485AB58F")]
        public readonly InputSlot<string> ActiveDataSetId = new();
    }
}
