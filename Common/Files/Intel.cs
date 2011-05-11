#region Copyright Notice
// ============================================================================
// Copyright (C) 2008 Ken Reed
// Copyright (C) 2009, 2010, 2011 The Stars-Nova Project
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
// This module contains the data that is generated by the Nova Console to
// generate a turn (including the very first one). This is the Intel sent to 
// the player.
// ===========================================================================
#endregion

#region Using Statements

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

using Nova.Common.Components;
using Nova.Common.DataStructures;
using NUnit.Framework;

#endregion

// ============================================================================
// Manipulation of the turn data that is created by the Nova Console and read
// by the Nova GUI.
// ============================================================================

namespace Nova.Common
{
   [Serializable]
   public sealed class Intel
   {

       // ============================================================================
       // The data items created by the Nova Console and read by the Nova GUI.
       // ============================================================================

       public int TurnYear = 2100;
       public Race MyRace = new Race();
       public ArrayList Messages = new ArrayList();
       public ArrayList Battles = new ArrayList();
       public ArrayList AllRaceNames = new ArrayList();
       public ArrayList AllScores = new ArrayList();

       /// <summary>
       /// Provide access for the client/GUI to a list of race icons for races the player has encountered, indexed by race name.
       /// Disambiguation: The AllRaceIcons singleton is a collection of all posible icons for use when creating races.
       /// The server/console should obtain race icon information from ServerState.Data.AllRaces. The client can not maintain such a collection as it has insuficient data on all races.
       /// </summary>
       public Hashtable RaceIcons = new Hashtable();

       public Hashtable AllFleets = new Hashtable();
       public Hashtable AllDesigns = new Hashtable();
       public Hashtable AllStars = new Hashtable();
       public Hashtable AllMinefields = new Hashtable();
       public TechLevel NewResearchLevels = new TechLevel();
       public TechLevel ResearchResources = new TechLevel();

       /// <summary>
       /// Default constructor.
       /// </summary>
       public Intel()
       {
       }

       /// ----------------------------------------------------------------------------
       /// <summary>
       /// Reset all data structures.
       /// </summary>
       /// ----------------------------------------------------------------------------
       public void Clear()
       {
           MyRace = null;
           AllDesigns.Clear();
           AllFleets.Clear();
           AllRaceNames.Clear();
           AllStars.Clear();
           AllMinefields.Clear();
           Battles.Clear();
           Messages.Clear();
           NewResearchLevels.Zero();
           ResearchResources.Zero();
            
           TurnYear = 2100;
       }

       #region To From Xml


       /// ----------------------------------------------------------------------------
       /// <summary>
       /// Load <see cref="Intel">Intel</see> from an xml document 
       /// </summary>
       /// <param name="xmldoc">produced using XmlDocument.Load(filename)</param>
       /// ----------------------------------------------------------------------------
       public Intel(XmlDocument xmldoc)
       {

           // re-initialise
           TurnYear = 2100;
           MyRace = new Race();
           Messages = new ArrayList();
           Battles = new ArrayList();
           AllRaceNames = new ArrayList();
           AllScores = new ArrayList();
           RaceIcons = new Hashtable();
           AllFleets = new Hashtable();
           AllDesigns = new Hashtable();
           AllStars = new Hashtable();
           AllMinefields = new Hashtable();
           NewResearchLevels = new TechLevel();
           ResearchResources = new TechLevel();

           XmlNode xmlnode = xmldoc.DocumentElement;
           while (xmlnode != null)
           {
               try
               {
                   switch (xmlnode.Name.ToLower())
                   {
                       case "root":
                           xmlnode = xmlnode.FirstChild;
                           continue;
                       case "intel":
                           xmlnode = xmlnode.FirstChild;
                           continue;

                       case "turnyear":
                           TurnYear = int.Parse(xmlnode.FirstChild.Value, System.Globalization.CultureInfo.InvariantCulture);
                           break;

                       case "race":
                           MyRace = new Race();
                           MyRace.LoadRaceFromXml(xmlnode);
                           break;

                       case "message":
                           Message message = new Message(xmlnode);
                           Messages.Add(message);
                           break;

                       case "battlereport":
                           BattleReport battle = new BattleReport(xmlnode);
                           Battles.Add(battle);
                           break;

                       case "racename":
                           AllRaceNames.Add(xmlnode.FirstChild.Value);
                           break;

                       case "scorerecord":
                           ScoreRecord newScore = new ScoreRecord(xmlnode);
                           AllScores.Add(newScore);
                           break;

                           
                       case "raceiconrecord":
                           // The race icon record is a dictionary entry (i.e. key/value pair) which links the race name to a given RaceIcon.
                           // This provides the client/GUI with a way to find icons for known races (as it doesn't/can't have AllRace data). 
                           // Both key and value must be serialised as the race name is not something that can be generated from the icon.
                           try
                           {
                               // first node should be the race name
                               string raceName;
                               XmlNode raceNameNode = xmlnode.FirstChild;
                               if (raceNameNode.Name.ToLower() == "racename")
                               {
                                   raceName = raceNameNode.FirstChild.Value;
                               }
                               else
                               {
                                   throw new Exception("Intel.cs : Xml Constructor - failed to load race icon record: race name not found.");
                               }

                               // second node should be the RaceIcon
                               RaceIcon newIcon = null;
                               XmlNode iconSourceNode = raceNameNode.NextSibling;
                               if (iconSourceNode.Name.ToLower() == "raceicon")
                               {
                                   newIcon = new RaceIcon(xmlnode.FirstChild);
                               }
                               else
                               {
                                   throw new Exception("Intel.cs : Xml Constructor - failed to load race icon record: RaceIcon not found");
                               }

                               RaceIcons.Add(raceName, newIcon); 
                           }
                           catch (Exception e)
                           {
                               Report.FatalError(e.Message + "\n Details: \n" + e);

                           }
                           break;
                           
                           // FIXME (priority 8) - these were all serialised from hashtables, but the key was not saved. 
                           // It may be ok to regenerate the key, but saving and loading the key may be better (unless we don't trust the .intel file???).

                       case "fleet":
                           Fleet newFleet = new Fleet(xmlnode);
                           AllFleets[newFleet.Key] = newFleet;
                           //AllFleets.Add(newFleet.Key, newFleet);
                           break;

                       case "design":
                           Design newDesign = new Design(xmlnode);
                           AllDesigns[newDesign.Key] = newDesign;
                           //AllDesigns.Add(newDesign.Key, newDesign);
                           break;

                       case "shipdesign":
                           ShipDesign newShipDesign = new ShipDesign(xmlnode);
                           AllDesigns[newShipDesign.Key] = newShipDesign;
                           //AllDesigns.Add(newShipDesign.Key, newShipDesign);
                           break;

                       case "star":
                           Star newStar = new Star(xmlnode);
                           AllStars.Add(newStar.Key, newStar);
                           break;

                       case "minefield":
                           Minefield minefield = new Minefield(xmlnode);
                           AllMinefields.Add(minefield.Key, minefield);
                           break;
                        
                       case "newtechlevels":
                           TechLevel newTechLevel = new TechLevel(xmlnode);
                           NewResearchLevels = newTechLevel;
                           break;
                       case "researchresources":
                           TechLevel researchResources = new TechLevel(xmlnode);
                           ResearchResources = researchResources;
                           break;

                       default: break;
                   }

               }
               catch (Exception e)
               {
                   Report.FatalError(e.Message + "\n Details: \n" + e);
               }

               xmlnode = xmlnode.NextSibling;
           }
       }


       /// ----------------------------------------------------------------------------
       /// <summary>
       /// Save: Serialise this object to an <see cref="XmlElement"/>.
       /// </summary>
       /// <param name="xmldoc">The parent <see cref="XmlDocument"/>.</param>
       /// <returns>An <see cref="XmlElement"/> representation of the Intel</returns>
       /// ----------------------------------------------------------------------------     
       public XmlElement ToXml(XmlDocument xmldoc)
       {
           // create the outer element
           XmlElement xmlelIntel = xmldoc.CreateElement("Intel");

           // TurnYear
           Global.SaveData(xmldoc, xmlelIntel, "TurnYear", TurnYear.ToString(System.Globalization.CultureInfo.InvariantCulture));

           // MyRace 
           xmlelIntel.AppendChild(MyRace.ToXml(xmldoc));

           // Messages 
           if (Messages.Count > 0)
           {
               foreach (Message message in Messages)
               {
                   xmlelIntel.AppendChild(message.ToXml(xmldoc));
               }
           }

           // Battles 
           if (Battles.Count > 0)
           {
               foreach (BattleReport battle in Battles)
               {
                   xmlelIntel.AppendChild(battle.ToXml(xmldoc));
               }
           }

           // AllRaceNames 
           foreach (string raceName in AllRaceNames)
           {
               Global.SaveData(xmldoc, xmlelIntel, "RaceName", raceName.ToString());
           }

           // AllScores 
           foreach (ScoreRecord score in AllScores)
           {
               xmlelIntel.AppendChild(score.ToXml(xmldoc));
           }

           // RaceIcons
           foreach (DictionaryEntry raceIconRecord in RaceIcons)
           {
               XmlElement xmlelRaceIconRecord = xmldoc.CreateElement("RaceIconRecord");
               Global.SaveData(xmldoc, xmlelRaceIconRecord, "RaceName", raceIconRecord.Key.ToString());
               xmlelRaceIconRecord.AppendChild(((RaceIcon)raceIconRecord.Value).ToXml(xmldoc));
               xmlelIntel.AppendChild(xmlelRaceIconRecord);
           }

           // AllFleets
           foreach (Fleet fleet in AllFleets.Values)
           {
               xmlelIntel.AppendChild(fleet.ToXml(xmldoc));
           }

           // AllDesigns 
           foreach (Design design in AllDesigns.Values)
           {
               if (design.Type == "Starbase" || design.Type == "Ship")
               {
                   xmlelIntel.AppendChild(((ShipDesign)design).ToXml(xmldoc));
               }
               else
               {
                   xmlelIntel.AppendChild(design.ToXml(xmldoc));
               }
           }

           // AllStars
           foreach (Star star in AllStars.Values)
           {
               xmlelIntel.AppendChild(star.ToXml(xmldoc));
           }

           // AllMinefields
           foreach (Minefield mine in AllMinefields)
           {
               xmlelIntel.AppendChild(mine.ToXml(xmldoc));
           }
            
           // AllNewResearchLevels
           // Only write out relevant information for this race.
           // There might not be any new levels...
           if (NewResearchLevels != null)
           {          
               xmlelIntel.AppendChild(NewResearchLevels.ToXml(xmldoc, "NewTechLevels"));
           }
           xmlelIntel.AppendChild(ResearchResources.ToXml(xmldoc, "ResearchResources"));

           // return the outer element
           return xmlelIntel;
       }

       #endregion
   }

}
