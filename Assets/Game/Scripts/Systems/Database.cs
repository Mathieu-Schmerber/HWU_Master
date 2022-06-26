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
                
                public static Game.Systems.Run.Rooms.CombatRoomData Combats = (Game.Systems.Run.Rooms.CombatRoomData)Instance.DatabaseAsset.Sections[1].Sections[0].Assets[1].Prefab;
                
                public static Game.Systems.Run.Rooms.RewardRoomData Events = (Game.Systems.Run.Rooms.RewardRoomData)Instance.DatabaseAsset.Sections[1].Sections[0].Assets[2].Prefab;
                
                public static IEnumerable<T> All<T>()
                    where T : UnityEngine.Object
                {
                    UnityEngine.Object[] all = new UnityEngine.Object[3] {Settings, Combats, Events};
                    return all.Where(x => x is T).Select(x => (T)x);
                }
            }
            
            public class Item
            {
                
                public static Game.Systems.Items.ItemSettingsData Settings = (Game.Systems.Items.ItemSettingsData)Instance.DatabaseAsset.Sections[1].Sections[1].Assets[0].Prefab;
                
                public static UnityEngine.GameObject LootedItem = (UnityEngine.GameObject)Instance.DatabaseAsset.Sections[1].Sections[1].Assets[1].Prefab;
                
                public static IEnumerable<T> All<T>()
                    where T : UnityEngine.Object
                {
                    UnityEngine.Object[] all = new UnityEngine.Object[2] {Settings, LootedItem};
                    return all.Where(x => x is T).Select(x => (T)x);
                }
                
                public class Items
                {
                    
                    public static Game.Systems.Items.SpecialItemData BerserkerMask = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[0].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData BloodChains = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[1].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData BombWarp = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[2].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData DemonsEye = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[3].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData GoldenClover = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[4].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData LunarisLightning = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[5].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData SharpenedBlade = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[6].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData SolarisBook = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[7].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData ArcaneIngot = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[8].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData LavaPotion = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[9].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData MandragoraRoot = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[10].Prefab;
                    
                    public static Game.Systems.Items.SpecialItemData SolarisOrbs = (Game.Systems.Items.SpecialItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[11].Prefab;
                    
                    public static Game.Systems.Items.ItemSettingsData Settings = (Game.Systems.Items.ItemSettingsData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[12].Prefab;
                    
                    public static Game.Systems.Items.StatItemData ArmorScraps = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[13].Prefab;
                    
                    public static Game.Systems.Items.StatItemData AssasinsBane = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[14].Prefab;
                    
                    public static Game.Systems.Items.StatItemData BatBat = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[15].Prefab;
                    
                    public static Game.Systems.Items.StatItemData BrokenWatch = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[16].Prefab;
                    
                    public static Game.Systems.Items.StatItemData DawnsBelt = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[17].Prefab;
                    
                    public static Game.Systems.Items.StatItemData ExecutionBlade = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[18].Prefab;
                    
                    public static Game.Systems.Items.StatItemData GloriousBoulder = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[19].Prefab;
                    
                    public static Game.Systems.Items.StatItemData HolyGrail = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[20].Prefab;
                    
                    public static Game.Systems.Items.StatItemData LuckyStar = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[21].Prefab;
                    
                    public static Game.Systems.Items.StatItemData LunarisRing = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[22].Prefab;
                    
                    public static Game.Systems.Items.StatItemData MaliceShard = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[23].Prefab;
                    
                    public static Game.Systems.Items.StatItemData MeltingPot = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[24].Prefab;
                    
                    public static Game.Systems.Items.StatItemData OminousStick = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[25].Prefab;
                    
                    public static Game.Systems.Items.StatItemData PhoenixFeather = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[26].Prefab;
                    
                    public static Game.Systems.Items.StatItemData SevenleagueBoots = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[27].Prefab;
                    
                    public static Game.Systems.Items.StatItemData ShamansWrath = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[28].Prefab;
                    
                    public static Game.Systems.Items.StatItemData SteelHammer = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[29].Prefab;
                    
                    public static Game.Systems.Items.StatItemData StoneBeads = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[30].Prefab;
                    
                    public static Game.Systems.Items.StatItemData VampiresTooth = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[31].Prefab;
                    
                    public static Game.Systems.Items.StatItemData WarriorsMight = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[32].Prefab;
                    
                    public static Game.Systems.Items.StatItemData Test = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[33].Prefab;
                    
                    public static Game.Systems.Items.StatItemData Test1 = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[34].Prefab;
                    
                    public static Game.Systems.Items.StatItemData Test2 = (Game.Systems.Items.StatItemData)Instance.DatabaseAsset.Sections[1].Sections[1].Sections[0].Assets[35].Prefab;
                    
                    public static IEnumerable<T> All<T>()
                        where T : UnityEngine.Object
                    {
                        UnityEngine.Object[] all = new UnityEngine.Object[36] {BerserkerMask, BloodChains, BombWarp, DemonsEye, GoldenClover, LunarisLightning, SharpenedBlade, SolarisBook, ArcaneIngot, LavaPotion, MandragoraRoot, SolarisOrbs, Settings, ArmorScraps, AssasinsBane, BatBat, BrokenWatch, DawnsBelt, ExecutionBlade, GloriousBoulder, HolyGrail, LuckyStar, LunarisRing, MaliceShard, MeltingPot, OminousStick, PhoenixFeather, SevenleagueBoots, ShamansWrath, SteelHammer, StoneBeads, VampiresTooth, WarriorsMight, Test, Test1, Test2};
                        return all.Where(x => x is T).Select(x => (T)x);
                    }
                }
            }
        }
        
        public class Previsualisations
        {
            
            public static UnityEngine.GameObject Circle = (UnityEngine.GameObject)Instance.DatabaseAsset.Sections[2].Assets[0].Prefab;
            
            public static IEnumerable<T> All<T>()
                where T : UnityEngine.Object
            {
                UnityEngine.Object[] all = new UnityEngine.Object[1] {Circle};
                return all.Where(x => x is T).Select(x => (T)x);
            }
        }
    }
}
