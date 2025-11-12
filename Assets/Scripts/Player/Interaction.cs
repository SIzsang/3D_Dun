using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{

    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance; // 거리
    public LayerMask layerMask; // 어떤 레이어가 달려있는 게임 오브젝트를 추출할지

    public GameObject curInteractGameObject; // 인터랙션 성공시 가지고 있는 게임 오브젝트 정보
    private IInteractable curInteractable; // 캐싱하는 자료가 이 두 변수에 담겨있다.

    public TextMeshProUGUI promptText; // 나중에 UI 분리를 해서 어떻게하면 직접적으로 사용 할 수 있을지 리팩토링
    private Camera camera;
    
    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask)) // 충돌이 된 물체가 있다면 hit에 정보를 넘겨준다. / 길이 / 레이어 마스크
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
