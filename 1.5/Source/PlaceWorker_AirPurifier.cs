using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace AirPurifier
{

    public class PlaceWorker_AirPurifier : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            Room room = center.GetRoom(currentMap);
            if (room is not null && !room.UsesOutdoorTemperature)
            {
                GenDraw.DrawFieldEdges(room.Cells.ToList<IntVec3>(), Color.green, null);
            }
        }
    }

}
