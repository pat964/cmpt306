using UnityEngine;
using System.Collections;

public class TutorialCamera : MonoBehaviour {
	Vector3 cameraPos;
	GameObject overlayView;
	GameObject battleView;
	GameObject handView;
	GameObject gameView;
	GameObject woundView;
	GameObject actionsView;
	int cameraSelect;

	// Use this for initialization
	void Start () {
		cameraSelect = 2;
		overlayView = GameObject.Find ("Background - Overlay");
		battleView = GameObject.Find ("Background - Battle camera");
		handView = GameObject.Find ("Background - hand");
		gameView = GameObject.Find ("Background - General Game");
		woundView = GameObject.Find ("Wound Background");
		actionsView = GameObject.Find ("Actions background");
		cameraPos = new Vector3 (0, 0, -10);

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			if (cameraSelect == 2) {
				transform.position = overlayView.transform.position + cameraPos;
				cameraSelect++;
			} 
			else if (cameraSelect == 3) {
				transform.position = battleView.transform.position + cameraPos;
				cameraSelect++;
			} 
			else if (cameraSelect == 4) {
				transform.position = handView.transform.position + cameraPos;
				cameraSelect++;
			}
			else if (cameraSelect == 5){
				transform.position = woundView.transform.position + cameraPos;
				cameraSelect++;
			}
			else if (cameraSelect == 6){
				transform.position = actionsView.transform.position + cameraPos;
				cameraSelect = 1;
			}
			else if (cameraSelect == 1){
				transform.position = gameView.transform.position + cameraPos;
				cameraSelect++;
			}

		}
	}
}
