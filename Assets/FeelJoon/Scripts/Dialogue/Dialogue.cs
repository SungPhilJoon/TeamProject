using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    [Serializable]
    public class Dialogue
    {
        public string name;

        [TextArea(3, 10)]
        public string[] sentences;
    }
}