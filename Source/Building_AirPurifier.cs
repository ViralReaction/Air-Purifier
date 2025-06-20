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
            compPowerTrader = GetComp<CompPowerTrader>();
            compRefuelable = GetComp<CompRefuelable>();
            CacheAdjacentCells();
        }

        private CompPowerTrader compPowerTrader;
        private CompRefuelable compRefuelable;
        private List<IntVec3> cachedAdjacentCells;
        private IntVec3 cachedPosition;

        private List<IntVec3> CacheAdjacentCells()
        {
            if (cachedAdjacentCells != null && cachedPosition == Position) return cachedAdjacentCells;
            cachedPosition = Position;
            cachedAdjacentCells = GenAdjFast.AdjacentCells8Way(Position).ToList();
            return cachedAdjacentCells;
        }
        public override void TickRare()
        {
            if (!compPowerTrader.PowerOn || !compRefuelable.HasFuel || this.IsOutside()) return;
            if (Position.GetGas(Map) is not Smoke) return;
            Room purifierRoom = this.GetRoom();
            if (purifierRoom is null) return;
            List<IntVec3> adjacentCells = CacheAdjacentCells();
            float totalFuelConsumed = 0f;
            for (int i = 0; i < adjacentCells.Count; i++)
            {
                IntVec3 adjacentCell = adjacentCells[i];
                if (adjacentCell.GetGas(Map) is not { } gas) continue;
                if (gas is not Smoke) continue;
                gas.Destroy();
                totalFuelConsumed += compRefuelable.ConsumptionRatePerTick;

            }
            if (totalFuelConsumed > 0)
            {
                compRefuelable.ConsumeFuel(totalFuelConsumed);
            }

        }
    }
}