using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogTongue : MonoBehaviour {

    public GameObject tongue;
    public GameObject tonguePivot;
    public Transform tongueTip;
    public AudioSource audioSource;
    public AudioClip frogTongue;
	public AudioClip tongueGrab;
	public float tongueRate = 1;
    public float maxTongueLength = 10.0f;
    public string tongueButton = "Tongue";
    public bool extendTongue = false;
    private bool hasItem = false;
    private GameObject item = null;
    
	void Start () {
        TongueRetract();
	}

    void Update()
    {
        if (hasItem && (item == null || item.transform.parent != transform))
        {
            hasItem = false;
        }

        if (Input.GetButtonDown(tongueButton) == true && !extendTongue)
        {
            // item not going to the right place !!!! NEEDS FIXING !!!!
            if (hasItem)
			{
				hasItem = false;

				//fire item
				RaycastHit2D tongueBlocker = Physics2D.CircleCast(transform.position, 0.25f, GetComponent<FrogMovement>().GetFrogDirectionVector(), maxTongueLength / 2, ~(1 << gameObject.layer));

                Vector3 newItemPosition = transform.position;
                FrogDirection tongueDir = GetComponent<FrogMovement>().GetFrogDirection();
				
                if (tongueBlocker)
                {
                    switch (tongueDir)
                    {
                        case FrogDirection.UP:
                            newItemPosition.y = Mathf.Round(tongueBlocker.point.y) - 0.5f;
                            break;

                        case FrogDirection.DOWN:
                            newItemPosition.y = Mathf.Round(tongueBlocker.point.y) + 0.5f;
                            break;

                        case FrogDirection.LEFT:
                            newItemPosition.x = Mathf.Round(tongueBlocker.point.x) + 0.5f;
                            break;

                        case FrogDirection.RIGHT:
                            newItemPosition.x = Mathf.Round(tongueBlocker.point.x) - 0.5f;
                            break;
                    }
                }
                else
                {
                    switch (tongueDir)
                    {
                        case FrogDirection.UP:
                            newItemPosition.y = transform.position.y + maxTongueLength / 2;
                            break;

                        case FrogDirection.DOWN:
                            newItemPosition.y = transform.position.y - maxTongueLength / 2;
                            break;

                        case FrogDirection.LEFT:
                            newItemPosition.x = transform.position.x - maxTongueLength / 2;
                            break;

                        case FrogDirection.RIGHT:
                            newItemPosition.x = transform.position.x + maxTongueLength / 2;
                            break;
                    }
                }

				item.layer = LayerMask.NameToLayer("Default");
                item.transform.position = newItemPosition;
                item.transform.parent = null;
                item = null;
            }
            else
            {
                tongue.SetActive(true);
                audioSource.PlayOneShot(frogTongue);
                StartCoroutine(TongueExtend());
            }
        }
        else if (Input.GetButtonUp(tongueButton) == true && extendTongue)
        {
            TongueRetract();
        }

        if (extendTongue)
        {
            float tongueLength = Vector3.Distance(tongueTip.position, tonguePivot.transform.position);
            RaycastHit2D tongueBlocker = Physics2D.CircleCast(transform.position, 0.25f, GetComponent<FrogMovement>().GetFrogDirectionVector(), tongueLength, ~(1 << gameObject.layer));

            if (tongueBlocker)
            {
                if (tongueBlocker.collider.tag == "PullItem")
                {
                    hasItem = true;
					item = tongueBlocker.collider.gameObject;
					item.layer = gameObject.layer;

                    Vector3 newItemPosition = tongueBlocker.transform.position;
                    FrogDirection tongueDir = GetComponent<FrogMovement>().GetFrogDirection();

                    switch (tongueDir)
                    {
                        case FrogDirection.UP:
                            newItemPosition.y = transform.position.y + 0.5f;
                            break;

                        case FrogDirection.DOWN:
                            newItemPosition.y = transform.position.y - 0.5f;
                            break;

                        case FrogDirection.LEFT:
                            newItemPosition.x = transform.position.x - 0.5f;
                            break;

                        case FrogDirection.RIGHT:
                            newItemPosition.x = transform.position.x + 0.5f;
                            break;
                    }
                    tongueBlocker.transform.parent = transform;

                    tongueBlocker.transform.position = newItemPosition;
                }
				else if (tongueBlocker.collider.tag == "ConsumeItem")
				{
					item = tongueBlocker.collider.gameObject;

					//do stuff

					Destroy(item);
				}
                else
                {
                    float leapFrogMultiplier = 1.0f;

                    if (tongueBlocker.collider.tag == "Frog")
                    {
                        leapFrogMultiplier = 3.0f;
                    }

                    Vector3 newPosition = transform.position;
                    FrogDirection tongueDir = GetComponent<FrogMovement>().GetFrogDirection();

                    switch (tongueDir)
                    {
                        case FrogDirection.UP:
                            newPosition.y = Mathf.Round(tongueTip.position.y) + 0.5f * leapFrogMultiplier;
                            break;

                        case FrogDirection.DOWN:
                            newPosition.y = Mathf.Round(tongueTip.position.y) - 0.5f * leapFrogMultiplier;
                            break;

                        case FrogDirection.LEFT:
                            newPosition.x = Mathf.Round(tongueTip.position.x) - 0.5f * leapFrogMultiplier;
                            break;

                        case FrogDirection.RIGHT:
                            newPosition.x = Mathf.Round(tongueTip.position.x) + 0.5f * leapFrogMultiplier;
                            break;
                    }

                    transform.position = newPosition;
                }

                TongueRetract();
				audioSource.PlayOneShot(tongueGrab);
			}
        }
    }

    private void GoToTop()
    {
        Collider2D[] collidedObjects = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer topRenderer = spriteRenderer;

        foreach (Collider2D collidedObject in collidedObjects)
        {
            SpriteRenderer otherSpriteRenderer = collidedObject.GetComponent<SpriteRenderer>();

            if (otherSpriteRenderer && otherSpriteRenderer.sortingOrder > topRenderer.sortingOrder)
            {
                topRenderer = otherSpriteRenderer;
            }
        }

        if (topRenderer != spriteRenderer)
        {
            int otherSortingOrder = topRenderer.sortingOrder;
            int sortingOrder = spriteRenderer.sortingOrder;

            topRenderer.sortingOrder = sortingOrder;
            spriteRenderer.sortingOrder = otherSortingOrder;
        }
    }

    private void TongueRetract()
    {
        tonguePivot.transform.localScale = new Vector3(2, 0, 1);
        extendTongue = false;
        tongue.SetActive(false);
    }

    private IEnumerator TongueExtend()
    {
        extendTongue = true;

        while (extendTongue)
        {
            if (tonguePivot.transform.localScale.y < maxTongueLength)
            {
                tonguePivot.transform.localScale += Time.deltaTime * Vector3.up * tongueRate;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
