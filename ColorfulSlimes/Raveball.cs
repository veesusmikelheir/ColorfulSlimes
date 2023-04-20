using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColorfulSlimes
{
    public class Raveball : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
    {
        private float lastColorChangeTime;

        public void ReadData(CompoundDataPiece piece) => enabled = piece.GetValue<bool>("enabled");

        public void WriteData(CompoundDataPiece piece) => piece.SetPiece("enabled", enabled);

        public void RegistryFixedUpdate()
        {
            if (enabled && Configuration.SHOULD_RANDOMIZE_WITH_DISCO)
            {
                if (Time.time - lastColorChangeTime >.25f)
                {
                    lastColorChangeTime = Time.time;

                    List<Color> generatedColors = Main.generatedColors;
                    Color[] currentRaveColors = new Color[]
                    {
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)],
                        generatedColors[Random.Range(0, generatedColors.Count)]
                    };

                    SetColor(currentRaveColors);
                }
            }
        }

        public void SetColor(Color[] colors)
        {
            foreach (var v in GetComponentsInChildren<Renderer>())
            {
                v.material.ColorX8Shader(colors);
            }
        }
    }
}
