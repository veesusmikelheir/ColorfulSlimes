using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes
{
    public class Raveball : RegisteredActorBehaviour, ExtendedData.Participant, RegistryFixedUpdateable
    {
        private float lastColorChangeTime;

        void Awake()
        {
            if (GetComponent<Raveball>() != this)
                Destroy(this);
        }

        public void ReadData(CompoundDataPiece piece) => enabled = piece.GetValue<bool>("enabled");

        public void WriteData(CompoundDataPiece piece) => piece.SetValue("enabled", enabled);

        public void RegistryFixedUpdate()
        {
            if (enabled && Config.SHOULD_RANDOMIZE_WITH_DISCO)
            {
                if (Time.time - lastColorChangeTime >.25f)
                {
                    lastColorChangeTime = Time.time;

                    Color[] colors = new Color[]
                    {
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],

                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)],
                        GeneratedColors[Random.Range(0, GeneratedColors.Count)]
                    };

                    SetColor(colors);
                }
            }
        }

        public void SetColor(Color[] colors)
        {
            foreach (var v in GetComponentsInChildren<Renderer>())
                v.material.ColorX8Shader(colors);
        }
    }
}
