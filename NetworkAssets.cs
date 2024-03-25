using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MoreUpgrades
{
    public class NetworkAssets
    {
        static AssetBundle assets;
        static public AssetBundle LoadAsset()
        {
            if (assets != null)
                return assets;
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assets = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "modassets"));
            return assets;
        }
    }
}
