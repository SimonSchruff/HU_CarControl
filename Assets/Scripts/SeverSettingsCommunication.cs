using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SeverSettingsCommunication : MonoBehaviour
{
    [Header("ToAdjust")]
    [SerializeField] string settingsName;
    [Space]
    [Header("SetUpReferences")]
    [SerializeField] InputField nameToSave;
    [SerializeField] Button button;
    int tempScore = 10;
    string getSettingsURL = "https://carcontroldatabase.000webhostapp.com/PHP/LoadSettingsFromUnity.php";
    string postSettingsURL = "https://carcontroldatabase.000webhostapp.com/PHP/LoadSettingsFromUnity.php";

    public void SaveDataToDatabase ()
    {
        StartCoroutine(SaveData());
    }

    IEnumerator SaveData ()
    {
        //Post Data With Request!
        //      List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //      formData.Add(new MultipartFormDataSection("Key", "Value"));
        //      UnityWebRequest webRequest = UnityWebRequest.Post(postSettingsURL, formData);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", settingsName));
        UnityWebRequest getSettingsRequest = UnityWebRequest.Post(postSettingsURL, formData);


    //    List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
    //    formData.Add(new MultipartFormDataSection("name" ,settingsName));

   //     UnityWebRequest getSettingsRequest = UnityWebRequest.Get(getSettingsURL);   //Create Web Request and getData
        yield return getSettingsRequest.SendWebRequest();
        
        if(getSettingsRequest.isHttpError || getSettingsRequest.isNetworkError)
        {
            Debug.Log("Error: " + getSettingsRequest.error);
        }
        else
        {
            Debug.Log(getSettingsRequest.downloadHandler.text);
        }
    }
}
