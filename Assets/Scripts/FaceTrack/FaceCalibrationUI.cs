using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FaceCalibrationUI : MonoBehaviour
{
    public ARKitFaceSolver solver;
    public Slider slider;

    public float calibrationTime = 3f;
    public string nextSceneName = "GameScene";

    float timer;

    void Start()
    {
        slider.value = 0f;
        solver.calibrarNeutral = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        slider.value = timer / calibrationTime;

        if (slider.value >= 1f)
        {
            FinishCalibration();
        }
    }

    void FinishCalibration()
    {
        solver.calibrarNeutral = false;

        PlayerPrefs.SetFloat("neutralJaw", solver.neutralJaw);
        PlayerPrefs.SetFloat("neutralMouthWidth", solver.neutralMouthWidth);
        PlayerPrefs.SetFloat("neutralBrowY", solver.neutralBrowY);
        PlayerPrefs.SetFloat("eyeOpenNeutralL", solver.eyeOpenNeutralL);
        PlayerPrefs.SetFloat("eyeOpenNeutralR", solver.eyeOpenNeutralR);
        PlayerPrefs.SetFloat("neutralYaw", solver.neutralYaw);

        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneName);
    }
}