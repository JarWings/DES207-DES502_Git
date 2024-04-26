using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float h, v;
    public bool dash;
    public bool attack;

    //testing function
    public bool getHurt;

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        dash = Input.GetButtonDown("Dash");
        attack = Input.GetButtonDown("Attack");

        //test function
        getHurt = Application.isEditor && Input.GetKeyDown(KeyCode.H);
    }
}
