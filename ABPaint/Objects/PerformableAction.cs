using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Objects
{
    public enum PerformableAction
    {
        // General Program Events
        Paint = 0,
        SelectionToolSelect = 1,
        UpdatePreview = 2,
        
        Delete = 3,
        AddElement = 4,

        // Power Tools

        UseTool = 5,
        ApplyTool = 6,
        CancelTool = 7,

        // Other

        Cut = 8,
        Copy = 9,
        Paste = 10,
        Fill = 11,
    }

    public struct PerformAction
    {
        public PerformableAction action;
        public object param1;
        public object param2;

        public PerformAction(PerformableAction _action)
        {
            action = _action;
            param1 = null;
            param2 = null;
        }

        public PerformAction(PerformableAction _action, object _param1)
        {
            action = _action;
            param1 = _param1;
            param2 = null;
        }

        public PerformAction(PerformableAction _action, object _param1, object _param2)
        {
            action = _action;
            param1 = _param1;
            param2 = _param2;
        }
    }
}
