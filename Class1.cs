using System;
using System.Windows.Forms;
using RDR2;
using RDR2.UI;
using RDR2.Native;
using RDR2.Math;

namespace ShrinkRay
{
    public class Main : Script
    {
        public Main()
        {
            KeyDown += OnKeyDown;

            Interval = 1;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // get player id
            Player playerId = Function.Call<Player>(Hash.PLAYER_ID);
            // get player ped
            Ped playerPed = Game.Player.Character;
            // get hash of High Roller Revolver
            uint hash = Function.Call<uint>(Hash.GET_HASH_KEY, "WEAPON_REVOLVER_DOUBLEACTION_GAMBLER");

            // check if numpad 1 is pressed
            if (e.KeyCode == Keys.F9)
            {
                // give player weapon
                Function.Call(Hash.GIVE_DELAYED_WEAPON_TO_PED, playerPed, hash, 100, 1, 0x2cd419dc);
                // give player ammo for weapon
                Function.Call(Hash.SET_PED_AMMO, playerPed, hash, 100);
                // set player's current weapon to High Roller Revolver
                Function.Call(Hash.SET_CURRENT_PED_WEAPON, playerPed, hash, 1, 0, 0, 0);
                // tell player they just received High Roller Revolver
                RDR2.UI.Screen.ShowSubtitle("High Roller Double-Action Revolver");
            }

            // check if shrink is true and E is pressed
            if (e.KeyCode == Keys.Z)
            {
                // so I can use "&" to set the value (thank you Dot.)
                unsafe
                {
                    // make sure variable is available
                    uint weaponHash;

                    // get current weapon hash
                    if (Function.Call<bool>(Hash.GET_CURRENT_PED_WEAPON, playerPed, &weaponHash, 0, 0, 0))
                    {
                        // compare current weapon hash to desired weapon hash
                        if (weaponHash == hash)
                        {
                            // check if player is targetting or aiming at something (thank you Slinky)
                            if (Function.Call<bool>(Hash.IS_PLAYER_TARGETTING_ANYTHING, playerId) || Function.Call<bool>(Hash.IS_PLAYER_FREE_AIMING, playerId))
                            {
                                // make sure variable is available
                                int targettedPed;
                                // should get the entity I'm aiming at, then run the code
                                if (Function.Call<bool>(Hash.GET_PLAYER_TARGET_ENTITY, playerId, &targettedPed) || Function.Call<bool>(Hash.GET_ENTITY_PLAYER_IS_FREE_AIMING_AT, playerId, &targettedPed))
                                {
                                    // check it entity is human and alive
                                    if (Function.Call<bool>(Hash.IS_PED_HUMAN, targettedPed) && !Function.Call<bool>(Hash.IS_ENTITY_DEAD, targettedPed))
                                    {
                                        // set ped scale to half
                                        Function.Call(Hash._SET_PED_SCALE, targettedPed, 0.5);
                                        // lets player know ped has been shrunk
                                        RDR2.UI.Screen.ShowSubtitle("Shrunk");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}