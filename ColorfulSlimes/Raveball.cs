using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class Raveball : RegisteredActorBehaviour, RegistryFixedUpdateable
    {
        private float lastColorChangeTime;

        public void RegistryFixedUpdate()
        {
            if (Time.time - lastColorChangeTime >.25f)
            {
                lastColorChangeTime = Time.time;
                SetColor(new Color(Random.value, Random.value, Random.value));
            }
        }

        public void SetColor(Color color)
        {
            foreach (var v in GetComponentsInChildren<Renderer>())
            {
                var mat = v.material;
                mat.color = color;
                v.material = mat;
            }
        }
    }
}
