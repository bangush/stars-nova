﻿#region Copyright Notice
// ============================================================================
// Copyright (C) 2008 Ken Reed
// Copyright (C) 2009, 2010 stars-nova
//
// This file is part of Stars-Nova.
// See <http://sourceforge.net/projects/stars-nova/>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as
// published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>
// ===========================================================================
#endregion

#region Module Description
// ===========================================================================
// This module provides a WaypointListBox which is a List box that passes on
// the delete key.
// ===========================================================================
#endregion

namespace Nova.WinForms.Gui
{
    using System.Windows.Forms;

    /// <Summary>
    /// This module provides a WaypointListBox which is a List box that passes on
    /// the delete key.
    /// </Summary>
    public class WaypointListBox : ListBox
    {
        /// <Summary>
        /// Determine if a key press is resolved by this object or passed on to other conrtols. 
        /// This overrides the default behavior of a ListBox to pass on the delete key so it can
        /// be used to delete a waypoint.
        /// </Summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                return true;
            }
            else
            {
                return base.IsInputKey(keyData);
            }
        }
    }
}
