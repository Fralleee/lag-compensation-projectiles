using UnityEngine;

namespace PolygonArsenal
{
    public class PolygonSoundSpawn : MonoBehaviour
    {

        public GameObject PrefabSound;

        public bool DestroyWhenDone = true;
        public bool SoundPrefabIsChild = false;
        [Range(0.01f, 10f)]
        public float PitchRandomMultiplier = 1f;

        // Use this for initialization
        void Start()
        {
            //Spawn the sound object
            GameObject m_Sound = Instantiate(PrefabSound, transform.position, Quaternion.identity);
            AudioSource m_Source = m_Sound.GetComponent<AudioSource>();

            //Attach object to parent if true
            if (SoundPrefabIsChild)
                m_Sound.transform.SetParent(transform);

            //Multiply pitch
            if (PitchRandomMultiplier != 1)
            {
                if (Random.value < .5)
                    m_Source.pitch *= Random.Range(1 / PitchRandomMultiplier, 1);
                else
                    m_Source.pitch *= Random.Range(1, PitchRandomMultiplier);
            }

            //Set lifespan if true
            if (DestroyWhenDone)
            {
                float life = m_Source.clip.length / m_Source.pitch;
                Destroy(m_Sound, life);
            }
        }
    }
}
