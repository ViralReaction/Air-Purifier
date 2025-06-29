using RimWorld;
using Verse;
using CombatExtended;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AirPurifier
{
    public class Building_AirPurifier : Building
    {
        private CompPowerTrader compPowerTrader;
        private CompRefuelable compRefuelable;
        private List<IntVec3> cachedAdjacentCells = [];
        private IntVec3 cachedPosition;
        private float fuelConsumptionRate;
        private const float MaxSmokeDensity = 10f;


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compPowerTrader = GetComp<CompPowerTrader>();
            compRefuelable = GetComp<CompRefuelable>();
            SmokeConsumptionExtension modExtension = def.GetModExtension<SmokeConsumptionExtension>();
            if (compPowerTrader == null)
            {
                Log.Error($"Air Purifier :: Missing required comp power trader. Removing {this}");
                Destroy();
                return;
            }
            if (compRefuelable == null)
            {
                Log.Error($"Air Purifier :: Missing required comp refuelable. Removing {this}");
                Destroy();
                return;
            }
            fuelConsumptionRate = modExtension?.consumptionRate ?? 0.015f;
            CacheAdjacentCells();
        }

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
                if (gas is not Smoke smoke) continue;
                float densityFactor = Mathf.Clamp01(smoke.density / MaxSmokeDensity);
                float scaledConsumption = fuelConsumptionRate * densityFactor;
                totalFuelConsumed += scaledConsumption;
                smoke.Destroy();

            }
            if (totalFuelConsumed > 0)
            {
                compRefuelable.ConsumeFuel(totalFuelConsumed);
            }
        }
    }
}