// ============================================================================
// Nova. (c) 2008 Ken Reed
//
// Provide research costs, etc.
//
// This is free software. You can redistribute it and/or modify it under the
// terms of the GNU General Public License version 2 as published by the Free
// Software Foundation.
// ============================================================================

using System;
using System.Collections;

using NovaCommon;

namespace NovaClient
{
 
// ============================================================================
// Class to deal with details of research
// ============================================================================

   public class Research
   {
       private static ClientState StateData = ClientState.Data;
 

// ============================================================================
// Return the total resource cost for researching a level (taking into account
// the cost factor specified in the race designer). Note that we skip the first
// few turns of the Fibonacci series as they are too close together.
// ============================================================================

      public static int Cost(int level) 
      {
         int techAjustment    = 0;

         foreach (int levelAttained in StateData.ResearchLevel)
         {
            techAjustment += levelAttained * 10;
         }
 
         // The research cost is based on a Fionacci series (starting) at 5
         // multimplied by 10 then 10 points per tech-level reached in all
         // fields is added. Finally, the cost factor specified in the Race
         // Designer is then added.
          // ??? (priority 3) is this the Stars! costs, or some approximation?

         int         baseCost    = (Fibonacci(level + 5) * 10) + techAjustment;
         int costFactor = (int)StateData.PlayerRace.ResearchCosts[StateData.ResearchTopic];
         
         return (baseCost * costFactor) / 100;
      }


// ============================================================================
// The resources required for each level are based on a Fibonacci series (the
// result of which is multiplied by a factor (TBD) to get the actual number
// required).
// ============================================================================

      private static int Fibonacci(int n)
      {
         if (n < 2) return n;
         return Fibonacci(n-1) + Fibonacci(n-2);
      }

   }
}