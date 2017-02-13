using TweakScale.Annotations;
using UnityEngine;
using System.Linq;

using System;


namespace TweakScale
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class PrefabDryCostWriter : SingletonBehavior<PrefabDryCostWriter>
    {
        [UsedImplicitly]
        private void Start()
        {
            Debug.Log("TweakScale::PrefabDryCostWriter: Start");
            WriteDryCost();
        }

        private void WriteDryCost()
        {
            var partsList = PartLoader.LoadedPartsList;

            foreach (var p in partsList)
            {
                var prefab = p.partPrefab;
                try
                {
                    if (!prefab.Modules.Contains("TweakScale"))
                        continue;

                    var m = prefab.Modules["TweakScale"] as TweakScale;
                    m.DryCost = (float)(p.cost - prefab.Resources.Cast<PartResource>().Aggregate(0.0, (a, b) => a + b.maxAmount * b.info.unitCost));
                    if (prefab.Modules.Contains("FSfuelSwitch"))
                        m.ignoreResourcesForCost = true;

                    if (m.DryCost < 0)
                    {
                        Debug.LogError("TweakScale::PrefabDryCostWriter: negative dryCost: part=" + p.name + ", DryCost=" + m.DryCost.ToString());
                        m.DryCost = 0;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("TweakScale::PrefabDryCostWriter: Bad module in prefab list: " + ex.Message);
                }

            }
        }
    }
}
