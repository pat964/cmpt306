using UnityEngine;
using System.Collections;

public class TutorialCamera : MonoBehaviour {
	Vector3 cameraPos;
	GameObject overlayView;
	GameObject overlayView1;
	GameObject overlayView2;
	GameObject battleView;
	GameObject battleView1;
	GameObject battleView2;
	GameObject handView;
	GameObject handView1;
	GameObject handView2;
	GameObject handView3;
	GameObject gameView;
	GameObject gameView1;
	GameObject gameView2;
	GameObject woundView;
	GameObject woundView1;
	GameObject woundView2;
	GameObject actionsView;
	GameObject actionsView1;
	int cameraSelect;

	// Use this for initialization
	void Start () {
		cameraSelect = 1;
		overlayView = GameObject.Find ("Background - Overlay");
		overlayView1 = GameObject.Find ("Background - Overlay (1)");
		overlayView2 = GameObject.Find ("Background - Overlay (2)");
		battleView = GameObject.Find ("Background - Battle camera");
		battleView1 = GameObject.Find ("Background - Battle camera (1)");
		battleView2 = GameObject.Find ("Background - Battle camera (2)");
		handView = GameObject.Find ("Background - hand");
		handView1 = GameObject.Find ("Background - hand (1)");
		handView2 = GameObject.Find ("Background - hand (2)");
		handView3 = GameObject.Find ("Background - hand (3)");
		gameView = GameObject.Find ("Background - General Game");
		gameView1 = GameObject.Find ("Background - General Game (1)");
		gameView2 = GameObject.Find ("Background - General Game (2)");
		woundView = GameObject.Find ("Wound Background");
		woundView1 = GameObject.Find ("Wound Background (1)");
		woundView2 = GameObject.Find ("Wound Background (2)");
		actionsView = GameObject.Find ("Actions background");
		actionsView1= GameObject.Find ("Actions background (1)");
		cameraPos = new Vector3 (0, 0, -10);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("b")) {
			if (cameraSelect > 1){
				cameraSelect--;
			}
			else{
				cameraSelect = 18;
			}
			if (cameraSelect == 1) {
				transform.position = gameView.transform.position + cameraPos;

			} else if (cameraSelect == 2) {
				transform.position = gameView1.transform.position + cameraPos;
				
			} else if (cameraSelect == 3) {
				transform.position = gameView2.transform.position + cameraPos;
				
			} else if (cameraSelect == 4) {
				transform.position = overlayView.transform.position + cameraPos;
				
			} else if (cameraSelect == 5) {
				transform.position = overlayView1.transform.position + cameraPos;
				
			} else if (cameraSelect == 6) {
				transform.position = overlayView2.transform.position + cameraPos;
				
			} else if (cameraSelect == 7) {
				transform.position = battleView.transform.position + cameraPos;
				
			} else if (cameraSelect == 8) {
				transform.position = battleView1.transform.position + cameraPos;
				
			} else if (cameraSelect == 9) {
				transform.position = battleView2.transform.position + cameraPos;
				
			} else if (cameraSelect == 10) {
				transform.position = handView.transform.position + cameraPos;
				
			} else if (cameraSelect == 11) {
				transform.position = handView1.transform.position + cameraPos;
				
			} else if (cameraSelect == 12) {
				transform.position = handView2.transform.position + cameraPos;
				
			} else if (cameraSelect == 13) {
				transform.position = handView3.transform.position + cameraPos;
				
			} else if (cameraSelect == 14) {
				transform.position = woundView.transform.position + cameraPos;
				
			} else if (cameraSelect == 15) {
				transform.position = woundView1.transform.position + cameraPos;
				
			} else if (cameraSelect == 16) {
				transform.position = woundView2.transform.position + cameraPos;
				
			} else if (cameraSelect == 17) {
				transform.position = actionsView.transform.position + cameraPos;
			}
			else if (cameraSelect == 18) {
				transform.position = actionsView1.transform.position + cameraPos;
			}
		}

			if (Input.GetKeyDown ("space")) {
				if (cameraSelect < 18){
					cameraSelect++;
				}
				else{
					cameraSelect = 1;
				}
				if (cameraSelect == 1){
					transform.position = gameView.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 2){
					transform.position = gameView1.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 3){
					transform.position = gameView2.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 4) {
					transform.position = overlayView.transform.position + cameraPos;
					
				} 
				else if (cameraSelect == 5) {
					transform.position = overlayView1.transform.position + cameraPos;
					
				} 
				else if (cameraSelect == 6) {
					transform.position = overlayView2.transform.position + cameraPos;
					
				} 
				else if (cameraSelect == 7) {
					transform.position = battleView.transform.position + cameraPos;
					
				} 
				else if (cameraSelect == 8) {
					transform.position = battleView1.transform.position + cameraPos;
					
				} 
				else if (cameraSelect == 9) {
					transform.position = battleView2.transform.position + cameraPos;
					
				} 
				else if (cameraSelect == 10) {
					transform.position = handView.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 11) {
					transform.position = handView1.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 12) {
					transform.position = handView2.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 13) {
					transform.position = handView3.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 14){
					transform.position = woundView.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 15){
					transform.position = woundView1.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 16){
					transform.position = woundView2.transform.position + cameraPos;
					
				}
				else if (cameraSelect == 17){
					transform.position = actionsView.transform.position + cameraPos;
				}
				else if (cameraSelect == 18) {
					transform.position = actionsView1.transform.position + cameraPos;
				}
				
				
			}
		}
	}

