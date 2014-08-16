using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FFACETools;

namespace CurePlease
{
    public partial class Form1
    {
        public class Items
        {
            private List<int> TempItemListIDs;

            #region Item Enums

            public enum TempUsableItems : int
            {
                LucidEtherI = 5827,
                LucidEtherII = 5828,
                LucidEtherIII = 5829,
                /*ManaMist = 5833,
                ManaPowder = 4255,
                DustyEther = 5432,
                DustyElixir = 5433,
                LucidElixirI = 5830,
                LucidElixirII = 5831,*/
                Megalixir = 4254
            }

            public enum UsableMPItems : int
            {
                Mulsum = 4156,
                Ether = 4128,
                Ether1 = 4129,
                Ether2 = 4130,
                Ehter3 = 4131,
                HiEther = 4132,
                HiEther1 = 4133,
                HiEther2 = 4134,
                HiEther3 = 4135,
                /*Elixir = ,
                ElixirVitae = ,
                VileElixir = ,
                HiElixir = ,
                VileElixir1 = */
            }

            public enum UsableMPMedicatedItems : int
            {
                //
            }

            #endregion

            public Items()
            {
                TempItemListIDs = new List<int>();
                GetTempItems();

                // Usage: 
                
            }

            private List<int> GetTempItems ()
            {
                for (byte i = 1; i <= _FFACEPL.Item.TemporaryCount; i++)
                {

                    foreach (var tempUsableItem in Enum.GetValues(typeof(TempUsableItems)))
                    {
                        if (_FFACEPL.Item.GetTempItemIDByIndex(i) == (int)tempUsableItem)
                        {
                            TempItemListIDs.Add(_FFACEPL.Item.GetTempItemIDByIndex(i));
                        }
                    }
                    
                }
                return TempItemListIDs;
            }

            private bool HasTempItem (TempUsableItems tempitemname)
            {                
                if (_FFACEPL.Item.GetTempItemCount((ushort)Enum.Parse(typeof(TempUsableItems), tempitemname.ToString())) > 0)
                {
                    return true;
                }                
                return false;                
            }           

            public void TempItemUsageDetermination ()
            {
                // Only continue if we have the option to use low MP items selected
                if (!Properties.Settings.Default.lowMPuseitem)
                    return;

                // Don't use items if your weakened
                if (_FFACEPL.Player.StatusEffects.Any(status => status == StatusEffect.Weakness))
                {
                    return;
                }

                int mpcurrent = _FFACEPL.Player.MPCurrent;
                int mpmax = _FFACEPL.Player.MPMax;
                int mppcurrent = _FFACEPL.Player.MPPCurrent;

                
                if (mpcurrent + 1000 < mpmax)
                {
                    if (HasTempItem(TempUsableItems.LucidEtherIII))
                    {
                        UseItem(TempUsableItems.LucidEtherIII);
                        Thread.Sleep(1500);
                    }
                }
                else if (( mpcurrent + 500 ) < mpmax)
                {
                    if (HasTempItem(TempUsableItems.LucidEtherII))
                    {
                        UseItem(TempUsableItems.LucidEtherII);
                        Thread.Sleep(1500);
                    }
                }
                else if (mppcurrent + 250 < mpmax)
                {
                    if (HasTempItem(TempUsableItems.LucidEtherI))
                    {
                        UseItem(TempUsableItems.LucidEtherI);
                        Thread.Sleep(1500);
                    }
                }
                // If we are out of MP then we need to use the megaelixir
                else if (mpcurrent <= Properties.Settings.Default.mpMinCastValue)
                {
                    if (HasTempItem(TempUsableItems.Megalixir))
                    {
                        UseItem(TempUsableItems.Megalixir);
                        Thread.Sleep(3500);
                    }
                }
            }

            private void UseItem(TempUsableItems item)
            {
                if (_FFACEPL.Player.Status == Status.Standing && HasTempItem(item))
                {
                    _FFACEPL.Windower.SendString(string.Format("/item {0} <me>", FFACE.ParseResources.GetItemName((int)Enum.Parse(typeof(TempUsableItems), item.ToString()))));
                }
            }

            private int GetMPPfromInt()
            {
                return ((_FFACEPL.Player.MPCurrent/_FFACEPL.Player.MPMax)*100);
            }
        }
    }
}
