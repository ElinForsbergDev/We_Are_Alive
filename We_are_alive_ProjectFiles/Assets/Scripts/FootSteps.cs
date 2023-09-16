using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootSteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] groundClips;
    [SerializeField] private AudioClip[] metalClips;
    [SerializeField] private AudioClip[] mudClips;
    [SerializeField] private AudioClip[] grassClips;
    [SerializeField] private AudioClip[] gravelClips;

    private AudioSource audioSource;

    private TerrainDetector terrainDetector;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        terrainDetector = new TerrainDetector();
    }

    //Step is an event from the Animation itself, everytime the animation fires "Step" , a Clip gets played
    // No, I think we call it from another class
    public void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    // Gets random sound clip from current terrain 
    private AudioClip GetRandomClip()
    {
        int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(transform.position);
        switch (terrainTextureIndex)
        {
            case 0:
                return GetClipFromArray(groundClips);
            case 1:
                return GetClipFromArray(groundClips);
            case 2:
                return GetClipFromArray(metalClips);
            case 3:
                return GetClipFromArray(mudClips);
            case 4:
                return GetClipFromArray(grassClips);
            case 5:
            case 6:
                return GetClipFromArray(gravelClips);
            default:
                return GetClipFromArray(groundClips);

        }
    }

    // Actually gets random clip from a given array
    private AudioClip GetClipFromArray(AudioClip[] clips)
    {
        return clips.Length > 0 ? clips[Random.Range(0, clips.Length)] : null;
    }
}
