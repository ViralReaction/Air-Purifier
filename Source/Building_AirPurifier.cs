using RimWorld;
using Verse;
using CombatExtended;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AirPurifier
{
    public class Building_AirPurifier : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.compPowerTrader = base.GetComp<CompPowerTrader>();
            this.compRefuelable = base.GetComp<CompRefuelable>();
            this.CacheAdjacentCells();
        }

        public CompPowerTrader compPowerTrader;
        public CompRefuelable compRefuelable;
        private List<IntVec3> cachedAdjacentCells;
        private IntVec3 cachedPosition;

        public List<IntVec3> CacheAdjacentCells()
        {
            if (cachedAdjacentCells == null || cachedPosition != Position)
            {
                cachedPosition = Position;
                cachedAdjacentCells = GenAdjFast.AdjacentCells8Way(Position).ToList();
            }
            return cachedAdjacentCells;
        }
        public override void TickRare()
        {
            if (this.compPowerTrader.PowerOn && this.compRefuelable.HasFuel)

            {
                if (this.Position.GetGas(Map) is Smoke )
                {
                    var purifierRoom = this.GetRoom();

                    if (purifierRoom is not null)
                    {
                        var adjacentCells = CacheAdjacentCells();
                        for (int i = 0; i < adjacentCells.Count; i++)
                        {
                            IntVec3 adjacentCell = adjacentCells[i];
                            if (adjacentCell.GetGas(Map) is Smoke existingSmoke)
                            {
                                existingSmoke.Destroy();
                                this.compRefuelable.ConsumeFuel(0.0025f);
                            }
                        }
                    }
                    this.compRefuelable.ConsumeFuel(0.00027777778f);
                }
                
            }
        }

    }
}