using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class roteCamera : MonoBehaviour
{
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private CinemachineBrain cinemachineBrain;
    [SerializeField]
    private DragHandler _lookController;
    [SerializeField]
    private Transform targetTransform;
    /// <summary> カメラ速度（°/px） </summary>
    [HideInInspector]
    public Vector2 _angularPerPixel;
    /// <summary> カメラ操作として前フレームにタッチしたキャンバス上の座標 </summary>
    private Vector2 _lookPointerPosPre;
    private Vector2 _movePointerPosBegin;
    private Vector3 _moveVector;

    /// <summary> 起動時 </summary>
    private void Awake()
    {
        _lookController.OnBeginDragEvent += OnBeginDragLook;
        _lookController.OnDragEvent += OnDragLook;

        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            MainCamera.transform.position = new Vector3(0,-1.78f , -5.63f);
            MainCamera.transform.rotation = Quaternion.Euler(-34 , 0 , 0);
            cinemachineBrain.enabled = false;
        }
        else
        {
            cinemachineBrain.enabled = true;
        }
    }

    private void Update() 
    {
        transform.position = targetTransform.position;
    }

    /// 移動操作
    #region Move

    /// <summary> ドラッグ操作開始（移動用） </summary>
    private void OnBeginDragMove(PointerEventData eventData)
    {
        // タッチ開始位置を保持
        _movePointerPosBegin = eventData.position;
    }

    /// <summary> ドラッグ操作中（移動用） </summary>
    private void OnDragMove(PointerEventData eventData)
    {
        // タッチ開始位置からのスワイプ量を移動ベクトルにする
        var vector = eventData.position - _movePointerPosBegin;
        _moveVector = new Vector3(vector.x, 0f, vector.y);
    }

    /// <summary> ドラッグ操作終了（移動用） </summary>
    private void OnEndDragMove(PointerEventData eventData)
    {
        // 移動ベクトルを解消
        _moveVector = Vector3.zero;
    }
     #endregion

    /// カメラ操作
    #region Look
    /// <summary> ドラッグ操作開始（カメラ用） </summary>
    private void OnBeginDragLook(PointerEventData eventData)
    {
        _lookPointerPosPre = _lookController.GetPositionOnCanvas(eventData.position);
    }

    /// <summary> ドラッグ操作中（カメラ用） </summary>
    private void OnDragLook(PointerEventData eventData)
    {
        var pointerPosOnCanvas = _lookController.GetPositionOnCanvas(eventData.position);
        // キャンバス上で前フレームから何px操作したかを計算
        var vector = pointerPosOnCanvas - _lookPointerPosPre;
        // 操作量に応じてカメラを回転
        LookRotate(new Vector2(-vector.y, vector.x));
        _lookPointerPosPre = pointerPosOnCanvas;
    }

    [SerializeField]
    private Vector2 n;
    private void LookRotate(Vector2 angles)
    {
        Vector2 deltaAngles = angles * _angularPerPixel;
        _camera.transform.localEulerAngles = new Vector3(Mathf.Clamp(deltaAngles.x + _camera.transform.localEulerAngles.x, n.x , n.y), _camera.transform.localEulerAngles.y);
        transform.eulerAngles += new Vector3(0f, deltaAngles.y);
    }
    #endregion
}