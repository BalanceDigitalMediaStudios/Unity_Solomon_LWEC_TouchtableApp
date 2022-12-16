using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class Habitat
    {
        public int id;
        public string title;
        public string image_src;
        public string button_src;
    }

    [System.Serializable]
    public class Sticker
    {
        public int id;
        public int habitatId;
        public string image_src;
        public bool is_earned;
    }
}
