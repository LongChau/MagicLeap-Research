using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _detectDistance = 5f;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Rigidbody2D _rigid2D;

    [SerializeField]
    private Transform _rightDetect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rayRight = Physics2D.Raycast(_rightDetect.position, _rightDetect.right, _detectDistance);
        var rayDown = Physics2D.Raycast(_rightDetect.position, -_rightDetect.up, _detectDistance);

        Debug.DrawLine(_rightDetect.position, new Vector2(_rightDetect.position.x + _detectDistance, _rightDetect.position.y), Color.green);
        Debug.DrawLine(_rightDetect.position, new Vector2(_rightDetect.position.x, _rightDetect.position.y - _detectDistance), Color.red);

        if (rayDown.collider != null && rayDown.collider.CompareTag("Stair"))
        {
            Debug.Log("Stair");

            _rigid2D.gravityScale = 0f;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(rayDown.collider.transform.right * _speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(-rayDown.collider.transform.right * _speed * Time.deltaTime);
            }
        }
        else
        {
            _rigid2D.gravityScale = 1f;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector2.right * _speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector2.left * _speed * Time.deltaTime);
            }
        }


        //Method_1();
    }

    private void Method_1()
    {
        var rayRight = Physics2D.Raycast(_rightDetect.position, _rightDetect.right, _detectDistance);
        var rayDown = Physics2D.Raycast(_rightDetect.position, -_rightDetect.up, _detectDistance);

        if (rayDown.collider != null && rayDown.collider.CompareTag("Stair"))
        {
            _rigid2D.gravityScale = 0f;
        }
        else
        {
            _rigid2D.gravityScale = 1f;
        }
    }
}
