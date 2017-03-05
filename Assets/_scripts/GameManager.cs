using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class GameManager : MonoBehaviour {

	public float GuideTime;
	public RiderController player;
	public Animator animator;
	public bool AutoGenerateHorse;
	public GameObject[] instanableHorse;
	public Text ScoreTxt;
	public drawcircle circle;
	public Text BestScore;
	public Text ScoreTxt2;


	private bool CouldStart = false;
	private bool CouldRestart = false;
	private int Score;
	private int GameoverHash = Animator.StringToHash ("GameOver");
	private int Gameover2Hash = Animator.StringToHash ("GameOver2");

	// Use this for initialization
	void Start () {
		StartCoroutine (GameLoop ());
	}
	
	// Update is called once per frame
	void Update () {
		UpdateScore ();
	}

	IEnumerator GameLoop(){

		// give player a horse
		RDHorseController horse =  AddHorse (player.transform.position);
		player.GetComponent<RiderController> ().RideHorse (horse, false);

		// Waiting for click...
		yield return StartCoroutine (RoundStarting());

		if (AutoGenerateHorse) {
			InvokeRepeating ("GenerateHorse", 0, 3f);
		}

		player.horse.GetComponent<RDHorseController> ().Run = true;

		// Guide user to control the player...
		yield return StartCoroutine (RoundGuiding());

		// Game playing...
		yield return StartCoroutine(RoundPlaying());

		// set timescale normal
		Time.timeScale = 1;

		//gameover, do some dirty work...
		GameOver();

		// Game end, waiting for click the restart button...
		yield return StartCoroutine(RoundEnding());

		// Restart button pressed, do sone clean up and reload the scene...
		Restart();

	}

	#region GameLoop Functions

	IEnumerator RoundStarting(){
		while (!CouldStart){
			yield return null;
		}
	}

	IEnumerator RoundGuiding(){
		animator.SetBool (Animator.StringToHash ("Guiding"), true);
		yield return new WaitForSeconds(GuideTime);
	}

	IEnumerator RoundPlaying(){
		animator.SetBool (Animator.StringToHash ("Playing"), true);
		while (!player.Dead){
			yield return null;
		}
	}

	IEnumerator RoundEnding(){
		PlayGameOverAnim ();
		while (!CouldRestart)
			yield return null;
	}

	#endregion


	public void OnStartPanelClick(){
		CouldStart = true;
	}

	public void OnRestartClick(){
		CouldRestart = true;
	}

	void GenerateHorse(){
		int count = Random.Range (1,4);
		for (int i = 0; i < count; i++){
			Vector3 PlayerPos = player.transform.position;
			PlayerPos.y = 0;
			PlayerPos.x += Random.Range (-8, 8);
			PlayerPos.z += 20 + Random.Range (5, 10);
			RDHorseController horse = AddHorse (PlayerPos);
			horse.Run = true;
		}
	}

	RDHorseController AddHorse(Vector3 pos){
		int t = Random.Range (0, instanableHorse.Length);
		//t = 4;
		GameObject newHorse = Instantiate (instanableHorse [t]);
		RDHorseController horse = newHorse.GetComponent<RDHorseController> ();
		newHorse.transform.position += pos;
		horse.circle = circle;
		return horse;
	}

	void UpdateScore(){
		if (!player.Dead){
			ScoreTxt.text = string.Format ("{0}m",Mathf.Floor(player.gameObject.transform.position.z));
		}
	}

	void GameOver(){
		Score = (int)Mathf.Floor (player.gameObject.transform.position.z);
		BestScore.text = GetHistoryHigh ().ToString();
		ScoreTxt2.text = Score.ToString();
		SaveBestScore ();
	}

	void Restart(){
		SceneManager.LoadScene("riderun");
	}

	void PlayGameOverAnim(){
		// TODO: lua controll
		animator.SetBool (GameoverHash, true);
	}

	void SaveBestScore(){
		int best = GetHistoryHigh ();
		if (Score > best){
			PlayerPrefs.SetInt ("Score", Score);
		}
	}

	int GetHistoryHigh(){
		return PlayerPrefs.GetInt ("Score");
	}
}
