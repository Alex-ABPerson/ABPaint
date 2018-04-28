// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 02-24-2018
//
// Last Modified By : Alex
// Last Modified On : 02-24-2018
// ***********************************************************************
// <copyright file="Corner.cs" company="">
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
    public enum Corner
    {
        /// <summary>
        /// No corner.
        /// </summary>
        None = 0,
        /// <summary>
        /// The top-left corner.
        /// </summary>
        TopLeft = 1,
        /// <summary>
        /// The top-right corner.
        /// </summary>
        TopRight = 2,
        /// <summary>
        /// The bottom-left corner.
        /// </summary>
        BottomLeft = 3,
        /// <summary>
        /// The bottom-right corner.
        /// </summary>
        BottomRight = 4
    }
}
