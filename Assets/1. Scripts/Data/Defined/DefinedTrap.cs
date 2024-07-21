using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinedTrap
{
    public struct SwampTrap
    {
        public int _id;
        public TRAP_TYPE _type;
        public Vector3 _positon;

        public SwampTrap(int id, TRAP_TYPE type, Vector3 position)
        {
            _id = id;
            _type = type;
            _positon = position;
        }
    }
}
