using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EstrogenMod
{
    internal class EstrogenBox : MonoBehaviour
    {
        public int pills = 10;

        void Update()
        {
            if (pills < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
