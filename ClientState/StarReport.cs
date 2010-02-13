// ============================================================================
// Nova. (c) 2008 Ken Reed
//
// What we know about each star system we have visited or scanned.
//
// This is free software. You can redistribute it and/or modify it under the
// terms of the GNU General Public License version 2 as published by the Free
// Software Foundation.
// ============================================================================

using System;
using System.Collections;
using System.Drawing;
using NovaCommon;

namespace NovaClient
{

// ============================================================================
/// Report class.
// ============================================================================

   [Serializable]
   public class StarReport
   {
      public NovaCommon.Resources  StarResources = null;
      public NovaCommon.Resources Concentration = null;
      public int        Population;
      public int        Age;
      public string     StarName;
      public int        Radiation;
      public int        Gravity;
      public int        Temperature;
      public string     Owner;
      public Fleet      Starbase;
      public Point      Position;
      public bool       OrbitingFleets;


// ===========================================================================
// Constructor.
// ===========================================================================

      public StarReport(Star star) 
      {
         StarResources  = new NovaCommon.Resources(star.ResourcesOnHand);
         Concentration = new NovaCommon.Resources(star.MineralConcentration);
         Population     = star.Colonists;
         StarName       = star.Name;
         Radiation      = star.Radiation;
         Gravity        = star.Gravity;
         Temperature    = star.Temperature;
         Owner          = star.Owner;
         Starbase       = star.Starbase;
         Position       = star.Position;
         OrbitingFleets = star.OrbitingFleets;
      }
   }
}