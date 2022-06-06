//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Databases
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Nawlian.Lib.Utils.Database;
    using Nawlian.Lib.Utils;
    using System.Linq;
    
    
    public class Database : Singleton<Database>
    {
        
        private Nawlian.Lib.Utils.Database.DatabaseData _databaseAsset;
        
        public virtual Nawlian.Lib.Utils.Database.DatabaseData DatabaseAsset
        {
            get
            {
                return _databaseAsset ??= Resources.Load<DatabaseData>("Data/Database");
            }
        }
        
        public class Templates
        {
            
            public class Editor
            {
                
                public static UnityEngine.GameObject AnimationPreview = (UnityEngine.GameObject)Instance.DatabaseAsset.Sections[0].Sections[0].Assets[0].Prefab;
                
                public static UnityEngine.GameObject RoomLogic = (UnityEngine.GameObject)Instance.DatabaseAsset.Sections[0].Sections[0].Assets[1].Prefab;
                
                public static IEnumerable<T> All<T>()
                    where T : UnityEngine.Object
                {
                    UnityEngine.Object[] all = new UnityEngine.Object[2] {AnimationPreview, RoomLogic};
                    return all.Where(x => x is T).Select(x => (T)x);
                }
            }
        }
        
        public class Data
        {
            
            public class Run
            {
                
                public static Game.Systems.Run.RunSettingsData Settings = (Game.Systems.Run.RunSettingsData)Instance.DatabaseAsset.Sections[1].Sections[0].Assets[0].Prefab;
                
                public static IEnumerable<T> All<T>()
                    where T : UnityEngine.Object
                {
                    UnityEngine.Object[] all = new UnityEngine.Object[1] {Settings};
                    return all.Where(x => x is T).Select(x => (T)x);
                }
            }
        }
    }
}
