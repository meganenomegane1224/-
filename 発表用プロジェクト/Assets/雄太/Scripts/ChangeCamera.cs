using UnityEngine;
using System.Collections;

public class ChangeCamera : MonoBehaviour
{

	//�@���C���J����
	[SerializeField]
	private GameObject mainCamera;
	//�@�؂�ւ��鑼�̃J����
	[SerializeField]
	private GameObject otherCamera;

	// Update is called once per frame
	void Update()
	{
		//�@1�L�[����������J�����̐؂�ւ�������
		if (Input.GetKeyDown("1"))
		{
			mainCamera.SetActive(!mainCamera.activeSelf);
			otherCamera.SetActive(!otherCamera.activeSelf);
		}
	}
}
