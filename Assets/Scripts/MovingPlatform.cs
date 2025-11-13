using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance; // 이동 거리
    public float moveSpeed; // 이동 속도

    private Vector3 startPos; 
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        float offset = Mathf.Sin(moveSpeed * Time.time) * moveDistance; // Sin = 사인 함수를 계산해주는 Unity 함수
                                                                        // 반환값 -1 ~ 1 
                                                                        // Sin(Time.time) 시간이 지나면서 -1 ~ 1을 왕복
                                                                        // moveDistance의 값 만큼 -2 ~ 2, -3 ~ 3 값을 왕복
                                                                        // moveSpeed는 Sin 함수의 속도, 주기가 빨라진다.
        
        Vector3 targetPos = startPos + transform.right * offset; // 시작 위치 + 방향 벡터 * 이동거리

        // _rigidbody.velocity = (targetPos - _rigidbody.position) / Time.fixedDeltaTime;
        // targetPos - _rigidbody.position 현재 위치 - 목표 위치 = 목표까지 남은 거리

        _rigidbody.MovePosition(targetPos);
        // rigidbody를 지정한 위치로 이동시키는 함수
        // kinematic 일때 외부 물리 영향 없이 이동이 가능
        //
    }
}
