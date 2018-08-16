using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionPortal : MonoBehaviour {

    private bool initialised = false;           //  Is the portal all set up?

    private Dimension m_destination;            //  Which dimension the portal leads to
    private Dimension m_origin;                 //  Which dimension the portal was made in
    private PortalState m_state;                //  The current open state of the portal
    private Transform m_transform;              //  The transform of the portal
    private PlayerController m_player;          //  A reference to the player controller
    private FloatHolder m_knifeFloat;           //  The progress of the slash animation
    private Renderer m_renderer;                //  The renderer of the portal
    [SerializeField]
    private float m_switchDistance = 0.1f;      //  How close the player has to be to activate the portal
    [SerializeField]
    private float m_effectDistance = 1.0f;      //  The distance at which the portal will begin to expand
    [HideInInspector]
    public float currentRange = 100f;           //  The current distance to the player
    private Vector3 startScale, endScale;       //  The start and end scale of the expansion
    [HideInInspector]
    public float effectFactor = 0f;
    public AnimationCurve effectCurve = AnimationCurve.EaseInOut(0.0f,0.0f,1.0f,1.0f);  //  The animation curve for the expansion
    
    [Space]
    [Space]

    //  Audio stuff
    [SerializeField]
    private AudioClip[] m_dimensionAudio;       //  The background audio for each dimension
    [SerializeField]
    private AudioClip m_effectAudio;            //  The slicing sound effect
    [SerializeField]
    private AudioClip m_windAudio;              //  The wind audio
    private AudioSource m_dimensionSource;      //  The audio source for the dimension audio
    private AudioSource m_effectSource;         //  The audio source for the slicing sound effect
    private AudioSource m_windSource;           //  The audio source for the wind sound


    //  Set up the portal
    private void InitPortal(FloatHolder knifeFloat)
    {
        //  Get various objects
        m_transform = GetComponent<Transform>();
        m_renderer = GetComponent<Renderer>();
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();
        m_effectSource = gameObject.AddComponent<AudioSource>();
        m_dimensionSource = gameObject.AddComponent<AudioSource>();
        m_windSource = gameObject.AddComponent<AudioSource>();
        m_knifeFloat = knifeFloat;
        startScale = m_transform.localScale;
        endScale = new Vector3(30f, 30f, 1f);
        m_state = PortalState.Opening;

        //  The portal is all set up
        initialised = true;
    }

    //  Open the portal
    public void OpenPortal(Dimension destination, Dimension origin, FloatHolder knifeFloat)
    {
        if (!initialised) InitPortal(knifeFloat);
        m_destination = destination;
        m_origin = origin;

        //  Change the portal to the right layer
        if(m_origin == Dimension.Default)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default_" + m_origin.ToString());
        }

        // Play the correct audio
        m_effectSource.PlayOneShot(m_effectAudio);
        m_player.isCreatingPortal = true;
        m_windSource.clip = m_windAudio;
        m_windSource.loop = true;
        m_windSource.Play();
        m_dimensionSource.clip = m_dimensionAudio[(int)destination];
        m_dimensionSource.loop = true;
        m_dimensionSource.Play();
        StartCoroutine(OpenPortalCoroutine());
    }

    private IEnumerator OpenPortalCoroutine()
    {
        float completion = 0.0f;

        //  Update the shader with the knife animation value
        while (m_state == PortalState.Opening)
        {
            completion = Mathf.Clamp01(m_knifeFloat.value);
            m_renderer.material.SetFloat("_Completion", completion);
            if (completion >= 1.0f) m_state = PortalState.Open;
            yield return new WaitForEndOfFrame();
        }

        m_player.isCreatingPortal = false;

        yield return null;
    }

    //  Close the portal
    public void Close()
    {
        StartCoroutine(ClosePortalCoroutine());
    }
    private IEnumerator ClosePortalCoroutine()
    {
        float completion = 0.0f;
        m_state = PortalState.Closing;

        Vector3 origPos = m_transform.position;

        //  Shrink the portal down
        while (m_state == PortalState.Closing)
        {
            completion += Time.deltaTime;
            m_transform.localScale = Vector3.Lerp(m_transform.localScale, Vector3.zero, completion);
            m_transform.position = Vector3.Lerp(origPos, origPos + Vector3.up, completion);

            if (completion >= 1.0f)
            {
                m_state = PortalState.Closed;
            }

            yield return new WaitForEndOfFrame();
        }

        //  Destroy the portal
        GameObject.Destroy(gameObject);
        yield return null;
    }

    private void Update()
    {
        if (!initialised) return;

        RotateToPlayer();

        currentRange = Vector2.Distance(new Vector2(m_transform.position.x, m_transform.position.z), new Vector2(m_player.cameraAnchor.position.x, m_player.cameraAnchor.position.z));

        float yDist = Mathf.Abs(m_player.transform.position.y - m_transform.position.y);

        //  If the portal is open, check if the player is walking through it
        if (m_state == PortalState.Open || m_state == PortalState.Opening)
        {
            if (currentRange <= m_effectDistance)
            {

                effectFactor = Mathf.Lerp(0, 1, 1 - currentRange);

                float t = effectCurve.Evaluate(effectFactor);

                m_transform.localScale = Vector3.Lerp(startScale, endScale, t);

                if (currentRange <= m_switchDistance && m_state == PortalState.Open && m_origin == m_player.GetDimension() && yDist <= 2f)
                {
                    m_player.SwitchDimensionImmediate(m_destination);
                    GameObject.Destroy(gameObject);
                }
            }
            else
            {
                m_transform.localScale = startScale;
            } 
        }

        //  Set the audio volume
        float vol = (1-(currentRange/5f)) * effectFactor;
        m_windSource.volume = vol;
        m_dimensionSource.volume = vol;
    }

    //  Rotate the portal to face the player
    public void RotateToPlayer()
    {
        Vector3 a = m_transform.position;
        a.y = 0.0f;
        Vector3 b = m_player.cameraAnchor.position;
        b.y = 0.0f;

        Vector3 relativePos = a - b;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        m_transform.rotation = rotation;
    }

    //  Return the state of the portal
    public PortalState GetState()
    {
        return m_state;
    }
}

//  An enum representing the open-ness of a portal
public enum PortalState
{
    Opening,
    Open,
    Closing,
    Closed
}