using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadGameButtonScript : MonoBehaviour
{
    public string scene="Test level";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(){
        DataService.Instance.LoadSceneWithLoadingScreen(scene);
    }

    public void ChangeQuit()
    {
        Application.Quit();
    }

    public void ClearSave()
    {
        DataService.Instance.ClearSave();
    }
}
