using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNumber;
    [SerializeField] private float shakingAfterSecond;

    private Animator animator;
    [SerializeField]  private MeshRenderer meshRenderer;
    private float time;
    private bool timerOn = false;
    private bool shakingIsOk=true;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        PlayerTouch.onShakingStatus += SetShakingStatus;// on pause disable the clue.
    }
    private void OnDestroy()
    {
        PlayerTouch.onShakingStatus -= SetShakingStatus;
    }
    private void Update()
    {
        if (timerOn && shakingIsOk)
        {
            time += Time.deltaTime;

            float seconds = Mathf.FloorToInt(time % 60);

            if (seconds == shakingAfterSecond)
                animator.SetBool("BallShaking", true);
        }
    }
    public TextMeshProUGUI GetNumber()
    {
        return textNumber;
    }
    public void SetNumber(int num)
    {
        textNumber.text = num.ToString();
    }
    public void SetMaterial(Material _material)
    {
        Material[] newMaterials = new Material[] { _material };
        meshRenderer.materials = newMaterials;
    }
    public void SetTimerOn()
    {
        time = 0;
        timerOn = true;
    }
    public void SetTimerOff()
    {
        timerOn = false;
    }
    private void SetShakingStatus(bool status)
    {
        shakingIsOk = status;
    }
}
