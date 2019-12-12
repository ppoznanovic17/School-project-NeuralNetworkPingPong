using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour {

    Vector3 ballStartPosition;
    Rigidbody2D rb;
    public AudioSource blip;
    public AudioSource blop;
    float speed = 500;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        ballStartPosition = this.transform.position;
        ResetujLoptu();

	}

    // vec postojeca funkcija
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "zaOdbijanje")
            blop.Play();
        
            
       
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("s"))
        {
            ResetujLoptu();
        }
        
    }
    public void ResetujLoptu()
    {
        this.transform.position = ballStartPosition;
        rb.velocity = Vector3.zero;
        Vector3 direction = new Vector3(Random.Range(0,900), Random.Range(-100,100),0).normalized;
        rb.AddForce(direction * speed);
    }
}
