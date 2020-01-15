﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.Windows;
using System.Windows.Controls;


namespace MinimalRune.Windows.Charts.Interactivity
{
    /// <summary>
    /// Represents a selection rectangle.
    /// </summary>
    public class SelectionRectangle : Control
    {
        

        



        

        



        

        


        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionRectangle"/> class.
        /// </summary>
        public SelectionRectangle()
        {
            DefaultStyleKey = typeof(SelectionRectangle);
        }
#else
        /// <summary>
        /// Initializes static members of the <see cref="SelectionRectangle"/> class.
        /// </summary>
        static SelectionRectangle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionRectangle), new FrameworkPropertyMetadata(typeof(SelectionRectangle)));
        }




        

        

    }
}
