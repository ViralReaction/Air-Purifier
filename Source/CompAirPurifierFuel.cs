using RimWorld;
using Verse;

namespace AirPurifier
{
    public class CompAirPurifierFuel : CompRefuelable
    {
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.allowAutoRefuel = this.Props.initialAllowAutoRefuel;
            this.fuel = this.Props.fuelCapacity * this.Props.initialFuelPercent;
            this.flickComp = this.parent.GetComp<CompFlickable>();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.fuel, "fuel", 0f, false);
            //Scribe_Values.Look<float>(ref this.configuredTargetFuelLevel, "configuredTargetFuelLevel", -1f, false);
            Scribe_Values.Look<bool>(ref this.allowAutoRefuel, "allowAutoRefuel", false, false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && !this.Props.showAllowAutoRefuelToggle)
            {
                this.allowAutoRefuel = this.Props.initialAllowAutoRefuel;
            }
        }
        public override void CompTick()
        {
        }
        public void ConsumeFilter(float amount)
        {
            Log.Message("TEST" + "Fuel Level" + fuel);
            if (fuel <= 0f)
            {
                return;
            }
            Log.Message("Before subtraction" + fuel);
            fuel -= amount;
            Log.Message("After subraction" + fuel);
            if (fuel <= 0f)
            {
                fuel = 0f;
                parent.BroadcastCompSignal("RanOutOfFuel");
            }
        }

        public new void Refuel(float amount)
        {
            if (this.fuel > this.Props.fuelCapacity)
            {
                this.fuel = this.Props.fuelCapacity;
            }
            this.parent.BroadcastCompSignal("Refueled");
        }

        private float fuel;
        private CompFlickable flickComp;
    }
       
}