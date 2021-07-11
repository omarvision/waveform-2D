using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Sprite))]
public class Track : MonoBehaviour
{
    public int width = 1024;
    public int height = 64;
    public Color background = Color.black;
    public Color foreground = Color.yellow;
    public GameObject arrow = null;
    public Camera cam = null;

    private AudioSource aud = null;
    private SpriteRenderer sprend = null;
    private int samplesize;
    private float[] samples = null;
    private float[] waveform = null;
    private float arrowoffsetx;

    private void Start()
    {
        // reference components on the gameobject
        aud = this.GetComponent<AudioSource>();
        sprend = this.GetComponent<SpriteRenderer>();

        Texture2D texwav = GetWaveform();
        Rect rect = new Rect(Vector2.zero, new Vector2(width, height));
        sprend.sprite = Sprite.Create(texwav, rect, Vector2.zero);

        arrow.transform.position = new Vector3(0f, 0f);
        arrowoffsetx = -(arrow.GetComponent<SpriteRenderer>().size.x / 2f);

        cam.transform.position = new Vector3(0f, 0f, -1f);
        cam.transform.Translate(Vector3.right * (sprend.size.x / 2f));
    }
    private void Update()
    {
        // move the arrow
        float xoffset = (aud.time / aud.clip.length) * sprend.size.x;
        arrow.transform.position = new Vector3(xoffset + arrowoffsetx, 0);
    }
    private Texture2D GetWaveform()
    {
        int halfheight = height / 2;
        float heightscale = (float)height * 0.75f;

        // get the sound data
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        waveform = new float[width];

        samplesize = aud.clip.samples * aud.clip.channels;
        samples = new float[samplesize];
        aud.clip.GetData(samples, 0);

        int packsize = (samplesize / width);
        for (int w = 0; w < width; w++)
        {
            waveform[w] = Mathf.Abs(samples[w * packsize]);
        }

        // map the sound data to texture
        // 1 - clear
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, background);
            }
        }

        // 2 - plot
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < waveform[x] * heightscale; y++)
            {
                tex.SetPixel(x, halfheight + y, foreground);
                tex.SetPixel(x, halfheight - y, foreground);
            }
        }

        tex.Apply();

        return tex;
    }
}
