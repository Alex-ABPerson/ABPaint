// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 03-17-2018
//
// Last Modified By : Alex
// Last Modified On : 03-24-2018
// ***********************************************************************
// <copyright file="PerformableAction.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
        public PerformableAction Action;
        public object Param1;
        public object Param2;
        public object Param3;
        public object Param4;
        public object Param5;

        public PerformAction(PerformableAction action)
        {
            Action = action;
            Param1 = null;
            Param2 = null;
            Param3 = null;
            Param4 = null;
            Param5 = null;
        }

        public PerformAction(PerformableAction action, object param1)
        {
            Action = action;
            Param1 = param1;
            Param2 = null;
            Param3 = null;
            Param4 = null;
            Param5 = null;
        }

        public PerformAction(PerformableAction action, object param1, object param2)
        {
            Action = action;
            Param1 = param1;
            Param2 = param2;
            Param3 = null;
            Param4 = null;
            Param5 = null;
        }

        public PerformAction(PerformableAction action, object param1, object param2, object param3)
        {
            Action = action;
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = null;
            Param5 = null;
        }

        public PerformAction(PerformableAction action, object param1, object param2, object param3, object param4)
        {
            Action = action;
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = null;
        }

        public PerformAction(PerformableAction action, object param1, object param2, object param3, object param4, object param5)
        {
            Action = action;
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj is PerformAction paction)
                if (Action == paction.Action && Param1 == paction.Param1 && Param2 == paction.Param2)
                    return true;

            return false;
        }

        public static bool operator ==(PerformAction x, PerformAction y)
        {
            return Compare(x, y);
        }

        public static bool operator !=(PerformAction x, PerformAction y)
        {
            return (Compare(x, y)) ? false : true;
        }

        private static bool Compare(Object obj1, Object obj2)
        {
            if (obj1 is PerformAction paction1)
                if (obj2 is PerformAction paction2)
                        if (paction1.Action == paction2.Action && paction1.Param1 == paction2.Param1 && paction1.Param2 == paction2.Param2)
                            return true;

            return false;
        }
    }
}
