using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Feng : Player
    {
        public Feng() : base()
        {
            SetHitboxData();
            champ = Champions.Feng;
        }

        public override void SetHitboxData()
        {
            Hitboxes = new Dictionary<string, List<List<List<Hitbox>>>>();
            List<List<List<Hitbox>>> finalTmp;
            List<List<Hitbox>> tmpLists;
            List<Hitbox> tmpBoxes;
            foreach (KeyValuePair<string, List<List<List<Hitbox>>>> data in HitboxManager.HitboxData[Champions.Feng])
            {
                finalTmp = new List<List<List<Hitbox>>>();
                foreach (List<List<Hitbox>> listBoxes in data.Value)
                {
                    tmpLists = new List<List<Hitbox>>();

                    tmpBoxes = new List<Hitbox>();
                    foreach (Hitbox box in listBoxes[0]) //green
                    {
                        tmpBoxes.Add(new Hitbox(box));
                    }
                    tmpLists.Add(tmpBoxes);

                    tmpBoxes = new List<Hitbox>();
                    foreach (Hitbox box in listBoxes[1]) // red
                    {
                        tmpBoxes.Add(new Hitbox(box));
                    }
                    tmpLists.Add(tmpBoxes);

                    finalTmp.Add(tmpLists);
                }
                Hitboxes.Add(data.Key, finalTmp);
            }
        }

    }
}
