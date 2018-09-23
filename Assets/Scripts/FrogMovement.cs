using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FrogDirection {UP, DOWN, LEFT, RIGHT}

public class FrogMovement : MonoBehaviour {

    public float cooldownTime = 0.5f;
    public AudioClip ribbit;
    public string moveHorizontal = "Horizontal";
    public string moveVertical = "Vertical";
    public float deadzone = 0.27f;
    public GameObject thisFrog;
    public GameObject otherFrog;

    private Vector2 axis = new Vector2(0, 0);
    private Vector2 newPosition;
    private bool cooldown = false;
    private Animator anim;
    private FrogDirection frogDir = FrogDirection.DOWN;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        axis.x = Input.GetAxis(moveHorizontal);
        axis.y = Input.GetAxis(moveVertical);
        newPosition = transform.position;

        if (axis.y > deadzone)
        {
            FrogMove(0, 1);
        }
        else if (axis.y < -deadzone)
        {
            FrogMove(0, -1);
        }
        else if(axis.x < -deadzone)
        {
            FrogMove(-1, 0);
        }
        else if(axis.x > deadzone)
        {
            FrogMove(1, 0);
        }

        if (Physics2D.OverlapPoint(newPosition) == false && FrogCanMove(newPosition, otherFrog.transform.position))
        {
            transform.position = newPosition;
        }
    }

    public FrogDirection GetFrogDirection()
    {
        return frogDir;
    }

    public Vector2 GetFrogDirectionVector()
    {
        Vector2 frogDirectionVector = Vector2.zero;
        
        switch (frogDir)
        {
            case FrogDirection.UP:
                frogDirectionVector = Vector2.up;
                break;

            case FrogDirection.DOWN:
                frogDirectionVector = Vector2.down;
                break;

            case FrogDirection.LEFT:
                frogDirectionVector = Vector2.left;
                break;

            case FrogDirection.RIGHT:
                frogDirectionVector = Vector2.right;
                break;
        }

        return frogDirectionVector;
    }

    private void FrogMove(int x, int y)
    {
        if (!cooldown && !GetComponent<FrogTongue>().extendTongue)
        {
            StartCoroutine(MovementCooldown());
            newPosition = newPosition + new Vector2(x, y);
            anim.SetBool("isMoving", true);

            float rotation = 0;

            if (x == 1)
            {
                rotation = 90;
                frogDir = FrogDirection.RIGHT;
            }
            else if (x == -1)
            {
                rotation = -90;
                frogDir = FrogDirection.LEFT;
            }

            if (y == 1)
            {
                rotation = 180;
                frogDir = FrogDirection.UP;
            }
            else if (y == -1)
            {
                rotation = 0;
                frogDir = FrogDirection.DOWN;
            }

            transform.eulerAngles = new Vector3(0, 0, rotation);

            GetComponent<AudioSource>().PlayOneShot(ribbit);
        }
    }

    public bool FrogCanMove(Vector3 frog1, Vector3 frog2)
    {
        //if camera zooms, this will need re-thinking as frogs could get caught off edge of visible screen
        bool canMove = true;
        float xApart = Vector3.Distance(new Vector3(frog1.x, 0, 0), new Vector3(frog2.x, 0, 0));
        float yApart = Vector3.Distance(new Vector3(0, frog1.y, 0), new Vector3(0, frog2.y, 0));

        if (xApart > Camera.main.orthographicSize * 3 || yApart > Camera.main.orthographicSize * 2)
        {
            canMove = false;
        }
        return canMove;
    }

    private IEnumerator MovementCooldown()
    {
        cooldown = true;
        while (cooldown)
        {
            yield return new WaitForSeconds(cooldownTime);
            cooldown = false;
            anim.SetBool("isMoving", false);
        }
    }
}
